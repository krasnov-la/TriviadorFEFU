using Game;
using Microsoft.AspNetCore.SignalR;

public sealed class GameHub : Hub
{
    public async Task Echo(string message)
    {
        await Clients.All.SendAsync("receiveMethod", message + " modified");
    }
}