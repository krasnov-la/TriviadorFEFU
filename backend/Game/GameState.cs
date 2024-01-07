using System.Text.Json;

namespace Game;
public class GameState
{
    public IEnumerable<AreaState> Areas {get;} = JsonSerializer.Deserialize<IEnumerable<AreaState>>(File.ReadAllText("Game/areas.json")) ?? new List<AreaState>();
    public Dictionary<string, PlayerState> Players {get;}
    public IEnumerable<string> Order {get; private set;} = new List<string>();
    public uint Turn {get; set;} = 0;
    public GameState(params string[] logins)
    {
        Players = logins.Select(l => new PlayerState(){
            Login = l
        }).ToDictionary(ps => ps.Login);

        FillOrder(logins);
    }

    private void FillOrder(params string[] logins)
    {
        int playerCount = logins.Length;
        for (int i = 0; i < playerCount; i++)
            Order = Order.Concat(
                logins.Select((login, index) 
                    => logins[(index + i) % playerCount] 
                )
            ).ToList();
    }
}