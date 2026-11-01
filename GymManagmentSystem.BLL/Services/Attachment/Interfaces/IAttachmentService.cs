using GymManagmentSystem.BLL.Common;
using static GymManagmentSystem.BLL.Services.Attachment.Classes.AttachmentService;

namespace GymManagmentSystem.BLL.Services.Attachment.Interface;
public interface IAttachmentService
{
    Task<Result<string?>> UploadAsync(Stream fileStream, string fileName, string FolderName);

    Result DeleteAsync(string fileName,  string FolderName);

    Result<FileResponse> GetFile(string fileName, string folderName);
}
