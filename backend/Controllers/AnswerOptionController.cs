using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnswerOptionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnswerOptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AnswerOption>> Get()
        {
            return _unitOfWork.AnswerRepo.All().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<AnswerOption> Get(Guid id)
        {
            var answerOption = _unitOfWork.AnswerRepo.First(x => x.Id == id);

            if (answerOption == null)
            {
                return NotFound();
            }

            return Ok(answerOption);
        }

        [HttpPost]
        public ActionResult<AnswerOption> Post([FromBody] AnswerOptionRequest request)
        {
            // Валидация и обработка запроса
            if (ModelState.IsValid)
            {
                var answerOption = new AnswerOption
                {
                    Text = request.Text,
                    // Маппинг остальных свойств
                };

                _unitOfWork.AnswerRepo.Add(answerOption);
                _unitOfWork.Save();

                return Ok(answerOption);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] AnswerOptionRequest request)
        {
            var existingAnswerOption = _unitOfWork.AnswerRepo.First(x => x.Id == id);

            if (existingAnswerOption == null)
            {
                return NotFound();
            }

            // Обновление свойств существующего объекта
            existingAnswerOption.Text = request.Text;

            _unitOfWork.AnswerRepo.Update(existingAnswerOption);
            _unitOfWork.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var answerOption = _unitOfWork.AnswerRepo.First(x => x.Id == id);
            if (answerOption == null)
            {
                return NotFound();
            }

            _unitOfWork.AnswerRepo.Remove(answerOption);
            _unitOfWork.Save();

            return NoContent();
        }
    }

    public class AnswerOptionRequest
    {
        public required string Text {get; set;}
        public bool Correct {get; set;}

    }
    
}
