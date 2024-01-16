namespace Services;

public class FileHandler : IFileHandler
{
    readonly string _mediaPath = Path.Combine(Directory.GetCurrentDirectory(), "media");
    public void Delete(string? fileName)
    {
        if (fileName == null) return;
        string fullPath = Path.Combine(_mediaPath, fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public string? Save(IFormFile? file)
    {
        if (file is null) return null;
        string newFilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        string fullPath = Path.Combine(_mediaPath, newFilename);

        using (var fStream = new FileStream(fullPath, FileMode.Create))
        {
            file.CopyTo(fStream);
        }

        return newFilename;
    }
}