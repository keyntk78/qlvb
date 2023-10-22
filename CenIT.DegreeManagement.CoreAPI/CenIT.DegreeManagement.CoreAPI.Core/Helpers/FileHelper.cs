using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public static class FileHelper
    {
        public static string GetUniqueFileName(string extension)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string uniqueFileName = string.Concat(timestamp, extension);

            return uniqueFileName;
        }

        public static Tuple<int, string> CheckValidFileExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new Tuple<int, string>(-1, "File không được để trống.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                return new Tuple<int, string>(-1, "Định dạng file không hợp lệ. Vui lòng chỉ chấp nhận file .xlsx.");
            }

            const int maxSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxSize)
            {
                return new Tuple<int, string>(-1, "Định dạng file không hợp lệ. Vui lòng chỉ chấp nhận file .xlsx.");
            }

            return new Tuple<int, string>(1, "");
        }
    }
}
