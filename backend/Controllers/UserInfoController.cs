using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using System;

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
            };

            return Ok(userDto);
        }
    }
}
