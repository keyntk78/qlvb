﻿using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Bussiness.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.QuanLySo
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoCapBanSaoController : BaseAppController
    {
        private SoCapBanSaoCL _cacheLayer;
        private TruongCL _truongCL;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;

        private ILogger<SoCapBanSaoController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;

        private readonly string _nameController = "SoBanSao";
        public SoCapBanSaoController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<SoCapBanSaoController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new SoCapBanSaoCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách học sinh theo sổ cấp bản sao
        /// API: /api/SoCapBanSao/GetHocSinhTheoCapBanSao
        /// </summary>
        /// <param name="paramModel"></param>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetHocSinhTheoCapBanSao")]
        public IActionResult GetHocSinhTheoCapBanSao(string idTruong, string idDanhMucTotNghiep, [FromQuery] SearchParamModel paramModel)
        {
            var truong = _truongCL.GetById(idTruong);
            var dmtn = _danhMucTotNghiepCL.GetById(idDanhMucTotNghiep);
            string soBanSao = "";
            if (truong == null || dmtn == null)
            {
                return Ok(ResponseHelper.ResultJson(soBanSao));
            }

            soBanSao = _cacheLayer.GetHocSinhTheoCapBanSao(truong, dmtn, paramModel);

            return Ok(ResponseHelper.ResultJson(soBanSao));
        }
    }
}
