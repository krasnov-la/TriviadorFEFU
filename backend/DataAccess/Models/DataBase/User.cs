using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class User
{
    public Guid Id {get; init;} = Guid.NewGuid();
    [MaxLength(256)]
    public required string Name {get; set;}
    public required string Password {get; set;}
    public bool Admin {get; set;} = false;
    [MaxLength(256)]
    public string? School {get; set;} = null;
    public int Rating {get; set;} = 0;
}