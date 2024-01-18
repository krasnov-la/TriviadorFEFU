namespace DataAccess.Models;
public class RegistrationRequest
{
    public required string Login {get; set;}
    public required string DisplayName {get; set;}
    public required string Password {get; set;}
    public string? School {get; set;}
}