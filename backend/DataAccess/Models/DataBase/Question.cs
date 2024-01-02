namespace DataAccess.Models;
public class Question
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public required string Text {get; set;}
    public List<AnswerOption> Options {get; set;} = new();
}