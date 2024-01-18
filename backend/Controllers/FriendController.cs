using Auth;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class FriendController : ControllerBase
{
    IUnitOfWork _unit;
    public FriendController(IUnitOfWork unit)
    {
        _unit = unit;
    }

    [AuthFilter("Default")]
    [HttpGet("Add/{id}")]
    public IActionResult Add(Guid id)
    {
        var user = _unit.UserRepo.First(u => u.Login == User.Identity.Name);
        var friend = _unit.UserRepo.First(u => u.Id == id);

        if (user is null || friend is null) return BadRequest();

        _unit.FriendRepo.Add(new FriendRelation()
        {
            PersonId = user.Id,
            FriendId = id
        });

        _unit.Save();
        return Ok();
    }

    [AuthFilter("Default")]
    [HttpGet("")]
    public IActionResult Get()
    {
        var user = _unit.UserRepo.First(u => u.Login == User.Identity.Name);
        if (user is null) return BadRequest();

        return Ok(_unit.FriendRepo.Where(r => r.PersonId == user.Id).Select(r => r.FriendId));
    }
}