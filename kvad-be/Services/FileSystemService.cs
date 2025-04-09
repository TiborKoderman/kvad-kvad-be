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
        var filePath = Path.Combine(_configuration["FileStorage:BasePath"], fileName);
        return filePath;
    }
}