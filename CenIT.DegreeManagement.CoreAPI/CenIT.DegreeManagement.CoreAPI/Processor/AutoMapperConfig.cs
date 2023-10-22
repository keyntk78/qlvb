﻿using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Models.Sys.Truong;

namespace CenIT.DegreeManagement.CoreAPI.Processor
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<HocSinhImportViewModel, HocSinhModel>();
            CreateMap<CauHinhPhoiGocInputModel, CauHinhPhoiGocModel>();
       
            //CreateMap<SoGocModel, SoGocViewModel>();
            CreateMap<DanhMucTotNghiepModel, DanhMucTotNghiepViewModel>();
            CreateMap<SoGocModel, ThongTinHocSinhInVanBangViewModel>();
            CreateMap<SoGocModel, ThongTinDanhSachHocSinhInVanBangViewModel>();
            CreateMap<TinTucViewModel, TinTucListModel>();

            //truong
            CreateMap<TruongModel, TruongDTO>();
            CreateMap<HocSinhViewModel, HocSinhDTO>();
            CreateMap<HocSinhInBangModel, HocSinhInBangDTO>();



        }
    }
}
