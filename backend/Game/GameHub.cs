using Game;
using Microsoft.AspNetCore.SignalR;

public sealed class GameHub : Hub
{
    private readonly Dictionary<Guid, GameState> _games = new();

    public void Connect(Guid gameId)
    {
        Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
    }
}