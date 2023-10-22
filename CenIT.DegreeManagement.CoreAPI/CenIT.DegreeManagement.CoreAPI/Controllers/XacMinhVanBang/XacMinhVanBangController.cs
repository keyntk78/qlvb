using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Controllers.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.XacMinhVanBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class XacMinhVanBangController : BaseAppController
    {

        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private XacMinhVanBangCL _cacheXacMinhVB;
        private TruongCL _truongCL;


        private ILogger<XacMinhVanBangController> _logger;
        private readonly IFileService _fileService;

        public XacMinhVanBangController(ICacheService cacheService, IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<XacMinhVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheXacMinhVB = new XacMinhVanBangCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);  
        }

        [HttpGet("GetSearchHocSinhXacMinhVanBang")]
        [AllowAnonymous]
        public IActionResult GetSearchHocSinhXacMinhVanBang([FromQuery] HocSinhSearchXacMinhVBModel model)
        {
            int total;

            var data = _cacheLayer.GetSearchHocSinhXacMinhVB(out total, model);

            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetByCccd")]
        [AllowAnonymous]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()), cccd);
        }

        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] XacMinhVangBangInputModel model)
        {

            if (model.FileYeuCau != null)
            {
                string folderName = "DonYeuCau";
                var fileResult = _fileService.SaveFile(model.FileYeuCau, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileYeuCau = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var data = await _cacheXacMinhVB.Create(model);
            if (data == (int)XacMinhVanBangEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("Thêm lịch sử xác minh thất bại"));
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage("Thêm lịch sử xác minh thành công"));

        }

        [HttpPost("CreateList")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateList([FromForm] XacMinhVangBangListInputModel model)
        {
            
            if (model.FileYeuCau != null)
            {
                string folderName = "DonYeuCau";
                var fileResult = _fileService.SaveFile(model.FileYeuCau, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileYeuCau = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var data = await _cacheXacMinhVB.CreateList(model);

            if (data == (int)XacMinhVanBangEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("Thêm lịch sử xác minh thất bại"));
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage("Thêm lịch sử xác minh thành công"));
        }

        [HttpGet("CauHinhXacMinhVB")]
        [AllowAnonymous]
        public IActionResult CauHinhXacMinhVB(string idDonVi)
        {
            
           var cauHinh = _truongCL.GetCauHinhXacMinhByIdDonVi(idDonVi);
            if(cauHinh == null) return ResponseHelper.NotFound("Không tìm thấy cấu hình");

            var model = new CauHinhXacMinhVanBangModel()
            {
                NguoiKyBang = cauHinh.NguoiKyBang,
                CoQuanCapBang = cauHinh.CoQuanCapBang,
                DiaPhuongCapBang = cauHinh.DiaPhuongCapBang,
                UyBanNhanDan = cauHinh.UyBanNhanDan
            };
            return ResponseHelper.Ok(model);
        }


        [HttpGet("GetLichSuPhatMinhVanbang")]
        [AllowAnonymous]
        public IActionResult GetLichSuPhatMinhVanbang([FromQuery]LichSuXacMinhVanBangSearchModel modelSearch)
        {
            var result = _cacheXacMinhVB.GetSearchLichSuXacMinh(modelSearch);

            return Ok(ResponseHelper.ResultJson(result));
        }


        [HttpGet("GetLichSuXacMinhById")]
        [AllowAnonymous]
        public IActionResult GetLichSuXacMinhById(string id)
        {
            if (id == null) return ResponseHelper.NotFound("Lần xác mình không tồn tại");
            var result = _cacheXacMinhVB.GetLichSuXacMinhById(id);

            return result != null ? ResponseHelper.Ok(result)
              : ResponseHelper.NotFound(_localizer.GetAddErrorMessage("Lần xác mình không tồn tại"));
        }
    }
}
