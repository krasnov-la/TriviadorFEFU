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
            "»ÃŒ",
            "œ»ÿ",
            "œ»",
            "»Õ“ËœÃ",
            "¬»-ÿ–Ã»",
            "»ÃŒ",
            "ÿÃËÕ∆",
            "ÿ›Ã",
            "ﬁÿ",
            "ÿ»√Õ",
            "ÿœ"
        };

        [HttpGet("schools")]
        public IActionResult GetAllSchools()
        {
            return Ok(_schools);
        }     
    }
}