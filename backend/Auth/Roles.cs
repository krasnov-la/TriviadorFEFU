namespace Auth;

public static class Roles
{
    //Role name should not exceed 32 characters.
    public static string Default {get;} = "Default";
    public static string Admin {get;} = "Admin";

    static readonly Dictionary<string, int> ValidationRules = new(new List<KeyValuePair<string, int>>(){
        new("Default", 0),
        new("Admin", 1)
    });

    public static bool Validate(string expected, string? actual)
    {
        if (actual is null || !ValidationRules.ContainsKey(actual)) return false;
        return ValidationRules[actual] >= ValidationRules[expected];
    }
}