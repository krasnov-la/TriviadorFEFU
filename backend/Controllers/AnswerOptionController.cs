using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerOptionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnswerOptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/AnswerOptions
        [HttpGet]
        public ActionResult<IEnumerable<AnswerOption>> GetAnswerOptions()
        {
            return _unitOfWork.AnswerRepo.All().ToList();
        }

        // GET: api/AnswerOptions/5
        [HttpGet("{id}")]
        public ActionResult<AnswerOption> GetAnswerOption(Guid id)
        {
            var answerOption = _unitOfWork.AnswerRepo.First(x => x.Id == id);

            if (answerOption == null)
            {
                return NotFound();
            }

            return answerOption;
        }

        // POST: api/AnswerOptions
        [HttpPost]
        public ActionResult<AnswerOption> PostAnswerOption(AnswerOption answerOption)
        {
            _unitOfWork.AnswerRepo.Add(answerOption);
            _unitOfWork.Save();

            return CreatedAtAction("GetAnswerOption", new { id = answerOption.Id }, answerOption);
        }

        // PUT: api/AnswerOptions/5
        [HttpPut("{id}")]
        public IActionResult PutAnswerOption(Guid id, AnswerOption answerOption)
        {
            if (id != answerOption.Id)
            {
                return BadRequest();
            }

            _unitOfWork.AnswerRepo.Update(answerOption);
            _unitOfWork.Save();

            return NoContent();
        }

        // DELETE: api/AnswerOptions/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAnswerOption(Guid id)
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
}
