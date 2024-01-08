namespace Game;
public interface IGameClient
{
    Task ConnectToLobby(IEnumerable<string> logins);
    Task LobbyNotFound();
    Task GameStart();
}