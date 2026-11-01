using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Attachment.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagmentSystem.BLL.Services.Attachment.Classes;

public class AttachmentService : IAttachmentService
{
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MG
    private readonly ILogger<AttachmentService> _logger;
    private readonly IWebHostEnvironment _env;
    private static readonly HashSet<string> _allowedExtentions =
        new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpeg", ".jpg", ".gif" };
    public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task<Result<string?>> UploadAsync(Stream fileStream, string fileName, string FolderName)
    {
        if (fileStream == null || !fileStream.CanRead)
        {
            _logger.LogWarning("File Rejected : Stream is null or not readable. FileName: {FileName}", fileName);
            return Result<string?>.Fail("File Not Found");
        }

        if (!fileStream.CanSeek)
        {
            _logger.LogWarning("File Rejected : Stream does not support seeking. FileName: {FileName}", fileName);
            return Result<string?>.Fail("Invalid File Stream");
        }

        if (fileStream.Length == 0)
        {
            _logger.LogWarning("File Rejected : File is empty. FileName: {FileName}", fileName);
            return Result<string?>.Fail("File Is Empty");
        }

        if (fileStream.Length > _maxFileSize)
        {
            _logger.LogWarning("File Rejected : File Too Large {Length} Bytes (Max {Max}). FileName: {FileName}",
                fileStream.Length, _maxFileSize, fileName);
            return Result<string?>.Fail("File Too Large (Max 5MB)");
        }

        var extention = Path.GetExtension(fileName)?.Trim();

        if (string.IsNullOrEmpty(extention) || !_allowedExtentions.Contains(extention))
        {
            _logger.LogWarning("File Rejected : Extension Not Allowed '{Extention}'. FileName: {FileName}",
                extention, fileName);
            return Result<string?>.Fail("File Extension Not Allowed");
        }

        var uploadedFolder = Path.Combine(_env.ContentRootPath, FolderName);
        var storedFileName = $"{Guid.NewGuid()}{extention}";
        var filePath = Path.Combine(uploadedFolder, storedFileName);

        try
        {
            Directory.CreateDirectory(uploadedFolder);

            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(fs);

            _logger.LogInformation("File Uploaded Successfully. Original: {FileName}, Stored: {StoredFileName}, Folder: {FolderName}, Size: {Length} Bytes",
                fileName, storedFileName, FolderName, fileStream.Length);

            return Result<string?>.OK(storedFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed To Upload File {FileName} To Folder {FolderName}", fileName, FolderName);
            return Result<string?>.Fail("Internal Server Error");
        }
    }

    public Result DeleteAsync(string fileName, string FolderName)
    {
        var fullPath = Path.Combine(_env.ContentRootPath, FolderName, fileName);

        try
        {
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File {FileName} was not found in folder {FolderName}", fileName, FolderName);
                return Result.Fail($"File '{fileName}' was not found in '{FolderName}'.");
            }

            File.Delete(fullPath);

            _logger.LogInformation("File {FileName} was deleted successfully from folder {FolderName}", fileName, FolderName);
            return Result.OK();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FileName} from folder {FolderName}", fileName, FolderName);
            return Result.Fail("An error occurred while deleting the file.");
        }
    }

    public record FileResponse(Stream Stream, string ContentType);
    public Result<FileResponse> GetFile(string fileName, string folderName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
        {
            _logger.LogWarning("File name or folder name is empty");
            return Result<FileResponse>.Fail("File name and folder name are required.");
        }

        var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);

        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("File {FileName} was not found in folder {FolderName}", fileName, folderName);
            return Result<FileResponse>.Fail($"File '{fileName}' was not found in '{folderName}'.");
        }

        try
        {
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            var extension = Path.GetExtension(fullPath).ToLowerInvariant();

            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            _logger.LogInformation("File {FileName} was read successfully from folder {FolderName}", fileName, folderName);
            return Result<FileResponse>.OK(new FileResponse(stream, contentType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read file {FileName} from folder {FolderName}", fileName, folderName);
            return Result<FileResponse>.Fail("An error occurred while reading the file.");
        }
    }
}
