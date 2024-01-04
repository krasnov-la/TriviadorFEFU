using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Text { get; set; }

        public List<AnswerOption> Options { get; set; } = new();
    }
}