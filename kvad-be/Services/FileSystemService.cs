public class FileSystemService
{
    private readonly ILogger<FileSystemService> _logger;
    private readonly IConfiguration _configuration;

    public FileSystemService(ILogger<FileSystemService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public string GetFilePath(string fileName)
    {
        var basePath = _configuration["FileStorage:BasePath"];
        if (string.IsNullOrEmpty(basePath))
        {
            throw new InvalidOperationException("Base path configuration is missing or null.");
        }
        var filePath = Path.Combine(basePath, fileName);
        return filePath;
    }
}