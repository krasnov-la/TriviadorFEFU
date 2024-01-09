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
    static Dictionary<string, Guid> _players = new();
    static Dictionary<string, int> _expandChoises = new();

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

        _lobbies[owner].Add(Context.UserIdentifier);
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
        if (game.Turn == 3 || game.Turn == 21) game.Phase++;
        
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
            case GamePhase.Duel: 
                 Clients.Users(game.Players.Keys)
                .StartTurnDuel(user);
                break;
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
        await Clients.Users(game.Players.Keys).Obtain(user, areaId);
        await Clients.Users(game.Players.Keys).EndTurn();

        StartTurn(gameId);
    }

    public async Task ChooseExpand(int areaId)
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
        
        _expandChoises[user] = areaId;
        await Clients.Users(game.Players.Keys).ExpandChoise(user, areaId);
        if (_expandChoises.Count == game.Players.Count)
            AskQuestion(gameId);
        
        StartTurn(gameId);
    }

    async Task AskQuestion(Guid gameId)
    {
        //generate random question guid
        var guid = Guid.Empty;
        var group = _games[gameId].Players.Keys;

        Dictionary<string, Task<bool>> answers = new();
        foreach (var user in group)
            answers.Add(user, Clients.User(user).AskQuestion(guid));

        await Task.WhenAll(answers.Values);

        await Clients.Users(group).ExpandChoisesDrop();

        foreach (var pair in answers)
            if (pair.Value.Result)
                await Clients.Users(group).Obtain(pair.Key, _expandChoises[pair.Key]);
        
        _expandChoises.Clear();
    } 
}