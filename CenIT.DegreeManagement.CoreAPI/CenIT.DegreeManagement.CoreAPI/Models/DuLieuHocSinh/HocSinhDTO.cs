﻿using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;

namespace CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh
{
    public class HocSinhDTO : BaseModel
    {
        public string? Id { get; set; }
        public string HoTen { get; set; } = null!;
        public int STT { get; set; }
        public string CCCD { get; set; } = null!;
        public DateTime NgaySinh { get; set; } = new DateTime();
        public string NoiSinh { get; set; } = null!;
        public bool GioiTinh { get; set; } = true;
        public string DanToc { get; set; } = null!;
        public string HanhKiem { get; set; } = null!;
        public string HocLuc { get; set; } = null!;
        public string KetQua { get; set; } = string.Empty;
        public string XepLoai { get; set; } = null!;
        public string GhiChu { get; set; } = string.Empty;
        public string DienXTN { get; set; } = string.Empty;
        public double DiemXTN { get; set; } = 0;
        public double DiemTB12 { get; set; } = 0;
        public double DiemKK { get; set; } = 0;
        public string? MaDKThi { get; set; } = string.Empty;
        public string? TenDKThi { get; set; } = string.Empty;
        public string? MaHD { get; set; } = string.Empty;
        public string? HoiDong { get; set; } = string.Empty;
        public TrangThaiHocSinhEnum TrangThai { get; set; }
        public string IdDanhMucTotNghiep { get; set; } = null!;
        public string IdTruong { get; set; } = null!;
        public string IdKhoaThi { get; set; } = null!;
        public string SoHieuVanBang { get; set; } = string.Empty;
        public string SoVaoSoCapBang { get; set; } = string.Empty;
        public string SoVaoSoBanSao { get; set; } = string.Empty;
        public List<KetQuaHocTapModel> KetQuaHocTaps { get; set; }
        public string? DiaChi { get; set; } = string.Empty;
        public string? Lop { get; set; } = string.Empty;
        public string? IdSoGoc { get; set; } = string.Empty;
        public string? IdPhoiGoc { get; set; } = string.Empty;
        public string? IdPhoiBanSao { get; set; } = string.Empty;
        public string? IdSoCapBanSao { get; set; } = string.Empty;
        public string? IdSoCapPhatBang { get; set; } = string.Empty;
        public int SoLanIn { get; set; } = 0;
        public ThongTinPhatBangModel ThongTinPhatBang { get; set; } = new ThongTinPhatBangModel();
        public string NamThi { get; set; } = string.Empty;
        public DateTime KhoaThi { get; set; }
        public string MaHinhThucDaoTao { get; set; }
        public SoGocModel SoGoc { get; set; } = new SoGocModel();
        public DanhMucTotNghiepModel DanhMucTotNghiep { get; set; } = new DanhMucTotNghiepModel();
        public TruongModel Truong { get; set; } = new TruongModel();
    }
}
