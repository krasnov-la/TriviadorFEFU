using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserInfoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{login}")]
        public IActionResult GetUser(string login)
        {

            var user = _unitOfWork.UserRepo.First(u => u.Login == login);

            if (user == null)
            {
                return NotFound($"User with Id {login} not found");
            }

            var userDto = new
            {
                Login = user.Login,
                DisplayName = user.DisplayName,
                ImgPath = user.ImgPath,
                School = user.School,
            };

            return Ok(userDto);
        }

        [HttpGet("GetMyself")]
        [Authorize]
        public IActionResult GetMyself()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return Unauthorized();
            var login = claimsIdentity.Name;
            if (login == null) return Unauthorized();

            var user = _unitOfWork.UserRepo.First(user => user.Login == login);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new
            {
                Login = user.Login,
                DisplayName = user.DisplayName,
                ImgPath = user.ImgPath,
                School = user.School,
            };

            return Ok(userDto);
        }
    }
}
