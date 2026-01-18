using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices
{
    public class LocalStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            // Tạo tên file ngẫu nhiên để tránh trùng
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Lấy đường dẫn gốc thư mục wwwroot
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", folderName);

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            // Lưu file vật lý
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn URL (để lưu vào DB)
            return $"/uploads/{folderName}/{fileName}";
        }
    }
}
