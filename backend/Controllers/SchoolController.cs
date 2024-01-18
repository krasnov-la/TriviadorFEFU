using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Controller
{
    [ApiController]
    [Route("[controller]")]
    public class SchoolController : ControllerBase
    {
        private readonly List<string> _schools = new List<string>
        {
            "ИМО",
            "ПИШ",
            "ПИ",
            "ИНТиПМ",
            "ВИ-ШРМИ",
            "ИМО",
            "ШМиНЖ",
            "ШЭМ",
            "ЮШ",
            "ШИГН",
            "ШП"
        };

        [HttpGet("schools")]
        public IActionResult GetAllSchools()
        {
            return Ok(_schools);
        }     
    }
}