using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AnswerOptionController : ControllerBase
	{
		private readonly AppDbContext _context;

		public AnswerOptionController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AnswerOption>>> GetAnswerOptions()
		{
			return await _context.Answers.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AnswerOption>> GetAnswerOption(Guid id)
		{
			var answerOption = await _context.Answers.FindAsync(id);

			if (answerOption == null)
			{
				return NotFound();
			}

			return answerOption;
		}

		[HttpPost]
		public async Task<ActionResult<AnswerOption>> PostAnswerOption(AnswerOption answerOption)
		{
			_context.Answers.Add(answerOption);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetAnswerOption", new { id = answerOption.Id }, answerOption);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutAnswerOption(Guid id, AnswerOption answerOption)
		{
			if (id != answerOption.Id)
			{
				return BadRequest();
			}

			_context.Entry(answerOption).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!AnswerOptionExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAnswerOption(Guid id)
		{
			var answerOption = await _context.Answers.FindAsync(id);
			if (answerOption == null)
			{
				return NotFound();
			}

			_context.Answers.Remove(answerOption);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool AnswerOptionExists(Guid id)
		{
			return _context.Answers.Any(e => e.Id == id);
		}
	}
}
