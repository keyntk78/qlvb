using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh
{
    public class ChinhSuaVanBangModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? HoTen { get; set; }
        public string? CCCD { get; set; }
        public DateTime? NgaySinh { get; set; } = new DateTime();
        public string? NoiSinh { get; set; }
        public bool? GioiTinh { get; set; }
        public string? DanToc { get; set; }
        public TrangThaiLichSuVanBangEnum TrangThai { get; set; }
        public string? LyDo { get; set; }
        public string? PathFileDonDeNghi { get; set; }
        public string? PathFileVanBang { get; set; }
        public string? PathFileGiayKhaiSinh { get; set; }
        public string? PathFileTrichLuc { get; set; }
        public string? PathFileCCCD { get; set; }

        public string? HoTenCu { get; set; }
        public string? CCCDCu { get; set; }
        public DateTime? NgaySinhCu { get; set; } = new DateTime();
        public string? NoiSinhCu { get; set; }
        public bool? GioiTinhCu { get; set; }
        public string? DanTocCu { get; set; }
    }

}
