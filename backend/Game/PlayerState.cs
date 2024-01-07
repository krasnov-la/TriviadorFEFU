namespace Game;
public class PlayerState
{
    public required string Login {get; init;}
    public int Score {get; set;} = 0;
    public IEnumerable<int> Areas {get; set;} = new List<int>();
}