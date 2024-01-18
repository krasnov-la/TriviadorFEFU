using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderBoardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaderBoardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Получить рейтинг школы (сумма учеников каждой школы)
        [HttpGet("ratings")]
        public IActionResult GetSchoolRatings()
        {
            var schoolRatings = _unitOfWork.UserRepo
                .All()
                .Where(u => !string.IsNullOrEmpty(u.School))
                .GroupBy(u => u.School)
                .Select(g => new { School = g.Key, TotalRating = g.Sum(u => u.Rating) })
                .OrderByDescending(r => r.TotalRating)
                .ToList();

            return Ok(schoolRatings);
        }


        // Получить N лучших учеников по школе
        [HttpGet("top-students-by-school")]
        public IActionResult GetTopStudentsBySchool(int n, string school)
        {
            var topStudentsBySchool = _unitOfWork.UserRepo
                .All()
                .Where(u => !string.IsNullOrEmpty(u.School) && u.School.Equals(school, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(u => u.Rating)
                .Take(n)
                .ToList();

            return Ok(topStudentsBySchool);
        }

        // Получить N лучших учеников без учета школы
        [HttpGet("top-students")]
        public IActionResult GetTopStudentsOverall(int n)
        {
            var topStudentsOverall = _unitOfWork.UserRepo
                .All()
                .OrderByDescending(u => u.Rating)
                .Take(n)
                .ToList();

            return Ok(topStudentsOverall);
        }
    }
}
