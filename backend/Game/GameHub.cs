using System.Text.RegularExpressions;
using Game;
using Microsoft.AspNetCore.SignalR;
using Auth;

[AuthFilter("Default")]
public sealed class GameHub : Hub<IGameClient>
{
    public Dictionary<string, List<string>> Lobbies {get;} = new Dictionary<string, List<string>>();
    public Dictionary<Guid, GameState> Games {get;} = new Dictionary<Guid, GameState>();
    async Task CreateLobby(string owner)
    {
        Lobbies[owner] = new List<string>(){ owner };
        await Clients.Caller.ConnectToLobby(Lobbies[owner]);
    }

    async Task JoinLobby(string owner)
    {
        if (!Lobbies.ContainsKey(owner))
        {
            await Clients.Caller.LobbyNotFound();
            return;
        }

        Lobbies[owner].Add(Context.User.Identity.Name);
        await Clients.Caller.ConnectToLobby(Lobbies[owner]);

        if (Lobbies[owner].Count == 3)
            await Clients.Users(Lobbies[owner]).GameStart();
    }
}