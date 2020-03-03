
using Microsoft.AspNetCore.Http;

namespace StudyMATEUpload.Services
{
    public interface IImageService
    {
        bool Create(IFormFile file, out string path);
        void Delete(string ImageUrl);
        bool Edit(IFormFile file, string imageUrl, out string path);
    }
}
