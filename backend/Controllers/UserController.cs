using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repository;
using Auth;
using Services;
using System.Net;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IFileHandler _fileHandler;

        public UserController(IUnitOfWork unitOfWork, IFileHandler fileHandler)
        {
            _unit = unitOfWork;
            _fileHandler = fileHandler;
        }

        [HttpGet("{login}")]
        public IActionResult GetUser(string login)
        {
            var user = _unit.UserRepo.First(u => u.Login == login);

            if (user == null) return NotFound($"User not found");

            var userDto = new
            {
                Login = user.Login,
                DisplayName = user.DisplayName
            };

            return Ok(userDto);
        }

        [HttpGet("image/{login}")]
        public HttpResponseMessage GetImage(string login)
        {
            string? imgName = _unit.UserRepo.First(u => u.Login == login)?.ImgPath;
            if (imgName is null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var dataBytes = System.IO.File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "media"));
            var dataStream = new MemoryStream(dataBytes);

            HttpResponseMessage message = new(HttpStatusCode.OK);
            message.Content = new StreamContent(dataStream);
            message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            message.Content.Headers.ContentDisposition.FileName = login + "_image";
            message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return message;
        }

        [AuthFilter("Default")]
        [HttpPost("{login}")]
        public IActionResult UpdateUser([FromForm] UserUpdateForm form, IFormFile? file)
        {
            User? user = _unit.UserRepo.First(u => u.Login == User.Identity.Name);
            if (user is null) return BadRequest("User deleted");

            if (file is not null)
            {
                _fileHandler.Delete(user.ImgPath);
                user.ImgPath = _fileHandler.Save(file);
            }

            user.DisplayName = form.DisplayName;
            user.School = form.School;

            _unit.UserRepo.Update(user);
            _unit.Save();

            return Ok();
        }
    }
}
