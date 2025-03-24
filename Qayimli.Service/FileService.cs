using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Qayimli.Core.Service;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Qayimli.Service
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions,string folderName)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            var wwwrootPath = _environment.WebRootPath;
            var path = Path.Combine(wwwrootPath, folderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check the allowed extensions
            var ext = Path.GetExtension(imageFile.FileName);
            if (!allowedFileExtensions.Contains(ext))
            {
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return fileName;
        }

        public async Task<string> SaveBase64FileAsync(string base64String, string[] allowedFileExtensions, string folderName)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            // Extract file extension from the base64 string
            var base64Data = base64String.Split(',')[1]; // Get the part after the comma
            var ext = base64String.Substring(5, base64String.IndexOf(';') - 5).Split('/')[1]; // e.g., "jpeg"

            if (!allowedFileExtensions.Contains($".{ext}"))
            {
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
            }

            var wwwrootPath = _environment.WebRootPath;
            var path = Path.Combine(wwwrootPath, folderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"{Guid.NewGuid()}.{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);

            var bytes = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(fileNameWithPath, bytes);

            return fileName;
        }

        public void DeleteFile(string fileNameWithExtension, string folderName)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            var wwwrootPath = _environment.WebRootPath;
            var path = Path.Combine(wwwrootPath, folderName, fileNameWithExtension);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Invalid file path");
            }
            File.Delete(path);
        }
    }
}
