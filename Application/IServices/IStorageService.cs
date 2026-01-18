using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IStorageService
    {
        // Hàm nhận file và trả về đường dẫn URL (string)
        Task<string> SaveFileAsync(IFormFile file, string folderName);
    }
}
