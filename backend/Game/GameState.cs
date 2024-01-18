using System.Text.Json;

namespace Game;

public enum GamePhase
    {
        Init,
        Expand,
        Duel
    }

public class GameState
{
    public IEnumerable<AreaState> Areas {get;} = JsonSerializer.Deserialize<IEnumerable<AreaState>>(File.ReadAllText("Game/areas.json")) ?? new List<AreaState>();
    public Dictionary<string, PlayerState> Players {get;}
    public List<string> Order {get; private set;} = new List<string>();
    public int Turn {get; set;} = -1;
    public string ExpectedPlayer {get; set;} = "";
    public GamePhase Phase {get; set;} = GamePhase.Init;
    public GameState(List<string> logins)
    {
        Players = logins.Select(l => new PlayerState(){
            Login = l
        }).ToDictionary(ps => ps.Login);

        FillOrder(logins);
    }

    private void FillOrder(List<string> logins)
    {
        //TODO: change to 2 (4 mb)
        for (int i = 0; i < 2; i++)
            Order = Order.Concat(
                logins.Select((login, index) 
                //TODO: change to 3
                    => logins[(index + i) % 2] 
                )
            ).ToList();
    }
}