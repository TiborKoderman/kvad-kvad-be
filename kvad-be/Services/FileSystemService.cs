public class FileSystemService(IConfiguration configuration)
{
    public string GetFilePath(string fileName)
    {
        var basePath = configuration["FileStorage:BasePath"];
        if (string.IsNullOrEmpty(basePath))
        {
            throw new InvalidOperationException("Base path configuration is missing or null.");
        }
        var filePath = Path.Combine(basePath, fileName);
        return filePath;
    }
}