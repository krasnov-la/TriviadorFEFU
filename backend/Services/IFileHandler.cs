namespace Services;

public interface IFileHandler
{
    string? Save(IFormFile? file);
    void Delete(string? path);
}
