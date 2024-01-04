using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class AnswerOption
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Text { get; set; }

        public bool Correct { get; set; }

        [ForeignKey("Question")]
        public Guid QuestionId { get; set; }

        public Question? Question { get; set; }
    }
}
