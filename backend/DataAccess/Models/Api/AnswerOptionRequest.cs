namespace DataAccess.Models;
ublic class AnswerOptionRequest
{
    public Guid QuestionId { get; set; }
    public required string Text { get; set; }
    public bool Correct { get; set; }

}