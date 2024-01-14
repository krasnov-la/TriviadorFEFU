using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QAEndpointController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QAEndpointController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public IActionResult GetQuestionsOptions(Guid id)
        {
            var question = _unitOfWork.QuestionRepo.First(x => x.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var answers = _unitOfWork.AnswerRepo.Where(a => a.QuestionId == id).ToList();

            var questionResponse = new
            {
                Text = question.Text
            };

            var answersResponse = answers.Select(answer => new
            {
                Text = answer.Text,
                Correct = answer.Correct
            }).ToList();

            return Ok(new { Question = questionResponse, Options = answersResponse });
        }
    }
}
