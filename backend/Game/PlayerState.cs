namespace Game;
public class PlayerState
{
    public required string Login {get; init;}
    public int Score {get; set;} = 0;
    public IEnumerable<int> Areas {get; set;} = new List<int>();
    public int MainArea {get; set;}
    public int HP {get; set;} = 3;
}