using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LinkUpAPI.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
        Task<bool> DeleteFileAsync(string fileUrl);
        string GetBlobSasUri(string blobName, int expirationInMinutes = 60);
    }
}