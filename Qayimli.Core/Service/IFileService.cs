using Microsoft.AspNetCore.Http;
using Qayimli.Core.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Service
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, string folderName);
        Task<string> SaveBase64FileAsync(string base64String, string[] allowedFileExtensions, string folderName);
        void DeleteFile(string fileNameWithExtension, string folderName);
    }
}
