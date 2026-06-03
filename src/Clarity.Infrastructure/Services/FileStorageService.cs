using Clarity.Application.Common.Interfaces;

namespace Clarity.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(string basePath = "storage/documents")
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_basePath, uniqueName);

        using var outputStream = File.Create(filePath);
        await fileStream.CopyToAsync(outputStream, cancellationToken);

        return uniqueName;
    }

    public Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, storagePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Document not found in storage.", filePath);

        Stream stream = File.OpenRead(filePath);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, storagePath);
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }
}
