using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

public class IdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}