using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang
{
    public class ChinhSuaVanBangInputModel
    {
        public string IdHocSinh { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public DateTime NgaySinh { get; set; } = new DateTime();
        public string NoiSinh { get; set; }
        public bool GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string? PathFileVanBan { get; set; }
        public IFormFile FileVanBan { get; set; }
        public string NguoiThucHien { get; set; }
        public string LyDo { get; set; }

    }
}
