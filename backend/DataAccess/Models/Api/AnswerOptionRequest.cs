namespace DataAccess.Models;
public class AnswerOptionRequest
{
    public Guid QuestionId { get; set; }
    public required string Text { get; set; }
    public bool Correct { get; set; }

}