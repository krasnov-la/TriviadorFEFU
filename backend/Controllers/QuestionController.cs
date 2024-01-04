using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Question>> GetQuestions()
        {
            return _unitOfWork.QuestionRepo.All().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Question> GetQuestion(Guid id)
        {
            var question = _unitOfWork.QuestionRepo.First(x => x.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        [HttpPost]
        public ActionResult<Question> PostQuestion(Question question)
        {
            _unitOfWork.QuestionRepo.Add(question);
            _unitOfWork.Save();

            return CreatedAtAction("GetQuestion", new { id = question.Id }, question);
        }

        [HttpPut("{id}")]
        public IActionResult PutQuestion(Guid id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            _unitOfWork.QuestionRepo.Update(question);
            _unitOfWork.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuestion(Guid id)
        {
            var question = _unitOfWork.QuestionRepo.First(x => x.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            _unitOfWork.QuestionRepo.Remove(question);
            _unitOfWork.Save();

            return NoContent();
        }
    }
}
