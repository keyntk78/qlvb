using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.XacMinhVanBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChinhSuaVanBangController : BaseAppController
    {
        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private ChinhSuaVanBangCL _cacheChinhSuaVanBang;


        private ILogger<ChinhSuaVanBangController> _logger;
        private readonly IFileService _fileService;
        public ChinhSuaVanBangController(ICacheService cacheService, IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<ChinhSuaVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheChinhSuaVanBang = new ChinhSuaVanBangCL(cacheService, configuration);
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] ChinhSuaVanBangInputModel model)
        {

            #region Save File

            if (model.FileDonDeNghi != null)
            {
                string folderName = "DonYeuCau/DonDeNghi";
                var fileResult = _fileService.SaveFile(model.FileDonDeNghi, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileDonDeNghi = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            if (model.FileVanBang != null)
            {
                string folderName = "DonYeuCau/VanBang";
                var fileResult = _fileService.SaveFile(model.FileVanBang, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileVanBang = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }


            if (model.FileGiayKhaiSinh != null)
            {
                string folderName = "DonYeuCau/GiayKhaiSinh";
                var fileResult = _fileService.SaveFile(model.FileGiayKhaiSinh, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileGiayKhaiSinh = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }


            if (model.FileTrichLuc != null)
            {
                string folderName = "DonYeuCau/TrichLuc";
                var fileResult = _fileService.SaveFile(model.FileTrichLuc, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileTrichLuc = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            if (model.FileCCCD != null)
            {
                string folderName = "DonYeuCau/CCCD";
                var fileResult = _fileService.SaveFile(model.FileCCCD, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileCCCD = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            #endregion

            var data = await _cacheChinhSuaVanBang.Create(model);
            if (data == (int)XacMinhVanBangEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("Thêm đơn chỉnh sửa thất bại"));
            if (data == (int)XacMinhVanBangEnum.HocSinhNotExist)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("Không tồn tại"));
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage("Thêm đơn chỉnh sửa thành công"));
        }
    }
}
