using System.Text.RegularExpressions;
using Auth;
using DataAccess.Repository;
using Game;
using Microsoft.AspNetCore.SignalR;
namespace Game;

[AuthFilter("Default")]
public sealed class GameHub : Hub<IGameClient>
{
    IUnitOfWork _unit;

    public GameHub(IUnitOfWork unit)
    {
        _unit = unit;
    }

    static Dictionary<string, List<string>> _lobbies = new();
    static Dictionary<Guid, GameState> _games = new();
    static Dictionary<string, Guid> _players = new();
    static Dictionary<string, int?> _expandChoises = new();

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

        string caller = Context.UserIdentifier;
        _lobbies[owner].Add(caller);
        await Clients.Users(_lobbies[owner]).UpdateLobby(caller);
        await Clients.Caller.JoinLobby(_lobbies[owner]);

        if (_lobbies[owner].Count == 3)
            await GameStart(owner);
    }

    async Task GameStart(string owner)
    {
        var gameId = Guid.NewGuid();
        foreach (var login in _lobbies[owner])
            _players[login] = gameId;
        
        _games[gameId] = new GameState(_lobbies[owner]);
        await Clients.Users(_lobbies[owner]).GameStart();
        StartTurn(gameId);
    }

    void StartTurn(Guid gameId)
    {
        var game = _games[gameId];
        game.Turn++;
        var user = game.Order[game.Turn % 9];
        game.ExpectedPlayer = user;
        if (game.Turn == 3) game.Phase++;

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

    async Task AskQuestion(Guid gameId)
    {

        IEnumerable<Guid> questionIds = _unit.QuestionRepo.All().Select(q => q.Id);
        var guid = questionIds.ElementAt(new Random(DateTime.Now.Microsecond).Next(questionIds.Count()));
        var group = _games[gameId].Players.Keys;

        Dictionary<string, Task<bool>> answers = new();
        foreach (var user in group)
            answers.Add(user, Clients.User(user).AskQuestion(guid));

        await Task.WhenAll(answers.Values);

        await Clients.Users(group).ExpandChoisesDrop();

        foreach (var pair in answers)
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