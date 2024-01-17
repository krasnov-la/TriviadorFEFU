using System.Text.RegularExpressions;
using Auth;
using Game;
using Microsoft.AspNetCore.SignalR;
namespace Game;

[AuthFilter("Default")]
public sealed class GameHub : Hub<IGameClient>
{
    static Dictionary<string, List<string>> _lobbies = new();
    static Dictionary<Guid, GameState> _games = new();
    static Dictionary<string, string> _lastConnection = new();
    static Dictionary<string, Guid> _players = new();
    static Dictionary<string, int?> _expandChoises = new();
    static Dictionary<Guid, Dictionary<string, Task<bool>>> _answers = new();
    static Dictionary<Guid, Dictionary<string, bool>> _playersAnswers = new();

    public override async Task OnConnectedAsync()
    {
        string user = Context.UserIdentifier;
        _lastConnection[user] = Context.ConnectionId;
        return;
    }
    public async Task CreateLobby()
    {
        var owner = Context.UserIdentifier;
        if (_lobbies.ContainsKey(owner))
            await Clients.Users(_lobbies[owner]).LobbyTerminated();
        
        _lobbies[owner] = new List<string>(){ owner };
        await Clients.Caller.JoinLobby(_lobbies[owner]);
    }

    public async Task JoinLobby(string owner)
    {
        if (!_lobbies.ContainsKey(owner))
        {
            await Clients.Caller.LobbyNotFound();
            return;
        }

        string callerLogin = Context.UserIdentifier;
        _lobbies[owner].Add(callerLogin);
        await Clients.Users(_lobbies[owner]).UpdateLobby(callerLogin);
        await Clients.Caller.JoinLobby(_lobbies[owner]);

        //TODO: change to 4
        if (_lobbies[owner].Count == 2)
            await GameStart(owner);
    }

    async Task GameStart(string owner)
    {
        var gameId = Guid.NewGuid();
        foreach (var login in _lobbies[owner])
            _players[login] = gameId;
        
        _games[gameId] = new GameState(_lobbies[owner]);
        _answers[gameId] = new Dictionary<string, Task<bool>>();
        _playersAnswers[gameId] = new Dictionary<string, bool>();
        await Clients.Users(_lobbies[owner]).GameStart(gameId);
        StartTurn(gameId);
    }

    void StartTurn(Guid gameId)
    {
        var game = _games[gameId];
        game.Turn++;
        //TODO: change to 9
        var user = game.Order[game.Turn % 4];
        game.ExpectedPlayer = user;
        //TODO: change to 3;
        if (game.Turn == 2) game.Phase++;

        if (20 - game.Players.Select(p => p.Value.Areas.Count()).Sum() < 3)
        {
            Clients.Users(game.Players.Keys).GameEnd();
            return;
        }
        
        switch (game.Phase)
        {
            case GamePhase.Init:
                Clients.Users(game.Players.Keys)
                .StartTurnInit(user);
                break;
            case GamePhase.Expand: 
                Clients.Users(game.Players.Keys)
                .StartTurnExpand(user);
                break;
            // case GamePhase.Duel: 
            //      Clients.Users(game.Players.Keys)
            //     .StartTurnDuel(user);
            //     break;
        }
    }

    public async Task ChooseInit(int areaId)
    {
        var user = Context.UserIdentifier;
        var gameId = _players[user];
        var game = _games[gameId];
        if (game.ExpectedPlayer != user)
        {
            await Clients.Caller.WrongOrderMove(game.ExpectedPlayer, user);
            return;
        }
        //TODO: check if move is valid

        game.Players[user].MainArea = areaId;
        
        game.Players[user].Areas = game.Players[user].Areas.Append(areaId);
        AddScore(game.Players[user], 200) ;
        await Clients.Users(game.Players.Keys).Obtain(user, areaId);
        await Clients.Users(game.Players.Keys).EndTurn();

        StartTurn(gameId);
    }

    public async Task ChooseExpand(int? areaId)
    {
        var user = Context.UserIdentifier;
        var gameId = _players[user];
        var game = _games[gameId];

        if (areaId is null)
        {
            StartTurn(gameId);
            return;
        }

        if (game.ExpectedPlayer != user)
        {
            await Clients.Caller.WrongOrderMove(game.ExpectedPlayer, user);
            return;
        }
        //TODO: check if move is valid
        
        _expandChoises[user] = areaId;
        await Clients.Users(game.Players.Keys).ExpandChoise(user, areaId);
        if (_expandChoises.Count == game.Players.Count)
            await AskQuestion(gameId);
        
        StartTurn(gameId);
    }

    public async Task AnswerQuestion(Guid gameId, string login, bool isCorrect) {
        _playersAnswers[gameId][login] = isCorrect;
        _answers[gameId][login].Start();
    }

    async Task AskQuestion(Guid gameId)
    {
        //generate random question guid
        var guid = new Guid("6d8148f0-6b80-4fb4-99ab-7bc00935f5d2");

        var group = _games[gameId].Players.Keys;

        foreach (var user in group) {
            _answers[gameId].Add(user, new Task<bool>(() => { 
                return _playersAnswers[gameId][user];
            }));
            Clients.Client(_lastConnection[user]).AskQuestion(guid);
        }

        await Task.WhenAll(_answers[gameId].Values);

        await Clients.Users(group).ExpandChoisesDrop();

        foreach (var pair in _answers[gameId])
            if (pair.Value.Result)
            {
                var choice = _expandChoises[pair.Key];
                var player = _games[gameId].Players[pair.Key];
                if (choice is null)
                {
                    AddScore(player, 100);
                    return;
                }
                AddScore(player, 200);
                await Clients.Users(group).Obtain(pair.Key, (int)choice);
            }
        
        _expandChoises.Clear();
    }

    void AddScore(PlayerState player, int add)
    {
        lock (player)
            player.Score += add;
    }
}