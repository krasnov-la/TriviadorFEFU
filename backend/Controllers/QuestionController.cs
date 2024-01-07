using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Question>> Get()
        {
            return _unitOfWork.QuestionRepo.All().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Question> Get(Guid id)
        {
            var question = _unitOfWork.QuestionRepo.First(x => x.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        [HttpPost]
        public ActionResult<Question> Post([FromBody] QuestionRequest request)
        {
            if (ModelState.IsValid)
            {
                var question = new Question
                {
                    Text = request.Text,
                };

                _unitOfWork.QuestionRepo.Add(question);
                _unitOfWork.Save();

                return Ok(question);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] QuestionRequest request)
        {
            var existingQuestion = _unitOfWork.QuestionRepo.First(x => x.Id == id);

            if (existingQuestion == null)
            {
                return NotFound();
            }

            existingQuestion.Text = request.Text;

            _unitOfWork.QuestionRepo.Update(existingQuestion);
            _unitOfWork.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
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
