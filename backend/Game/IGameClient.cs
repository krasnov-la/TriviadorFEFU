namespace Game;
public interface IGameClient
{
    public Task LobbyTerminated();
    public Task JoinLobby(IEnumerable<string> logins);
    public Task LobbyNotFound();
    public Task GameStart();
    public Task StartTurnInit(string login);
    public Task StartTurnExpand(string login);
    public Task StartTurnDuel(string login);
    public Task Obtain(string login, int areaId);
    public Task ExpandChoise(string login, int areaId);
    public Task ExpandChoisesDrop();
    public Task EndTurn();
    public Task<bool> AskQuestion(Guid questionId);
    public Task WrongOrderMove(string expected, string actual);
}