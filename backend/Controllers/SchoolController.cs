using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Controller
{
    [ApiController]
    [Route("[controller]")]
    public class SchoolController : Controller
    {
        private readonly List<string> _schools = new List<string>
        {
            "���",
            "���",
            "��",
            "������",
            "��-����",
            "���",
            "�����",
            "���",
            "��",
            "����",
            "��"
        };

        [HttpGet("schools")]
        public IActionResult GetAllSchools()
        {
            return Ok(_schools);
        }     
    }
}