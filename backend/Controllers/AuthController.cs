using DataAccess.Repository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    IUnitOfWork _unit; 
    public AuthController(IUnitOfWork unit)
    {
        _unit = unit;
    }
    [HttpGet("Test")]
    public IActionResult Test()
    {
        _unit.UserRepo.Add(new User(){
            Name = "Dwarf",
            Password = "fojfoejf"
        });

        _unit.Save();
        return Ok("nwifhuhfueh");
    }
}