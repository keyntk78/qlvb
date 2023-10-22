using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.QuanLySo
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SoGocController : BaseAppController
    {
        private SoGocCL _cacheLayer;
        private TruongCL _truongCL;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;



        private ILogger<SoGocController> _logger;
        private readonly ShareResource _localizer;
        private readonly IMapper _mapper;

        private readonly string _nameController = "SoGoc";
        public SoGocController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<SoGocController> logger, IMapper mapper) : base(cacheService, configuration)
        {
            _cacheLayer = new SoGocCL(cacheService, configuration); 
            _truongCL = new TruongCL(cacheService, configuration);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách học sinh theo sổ gốc
        /// API: /api/SoGoc/GetHocSinhTheoSoGoc
        /// </summary>
        /// <param name="paramModel"></param>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetHocSinhTheoSoGoc")]
        [AllowAnonymous]
        public IActionResult GetHocSinhTheoSoGoc([FromQuery] string idTruong, [FromQuery] string idDanhMucTotNghiep, [FromQuery] SearchParamModel paramModel)
        {
            var truong = _truongCL.GetById(idTruong);
            var dmtn = _danhMucTotNghiepCL.GetById(idDanhMucTotNghiep);
            string soGoc = "";
            if(truong == null || dmtn == null)
            {
                return Ok(ResponseHelper.ResultJson(soGoc));
            }

            soGoc = _cacheLayer.GetHocSinhTheoSoGoc(truong, dmtn, paramModel);

            return Ok(ResponseHelper.ResultJson(soGoc));
        }

        [HttpGet("GetbyIdDanhMucTotNghiep")]
        public IActionResult GetbyIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var soGoc = _cacheLayer.GetbyIdDanhMucTotNghiep(idDanhMucTotNghiep);
            return soGoc != null ? ResponseHelper.Ok(soGoc)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
        }

    }
}
