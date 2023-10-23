using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Resources;
using CenIT.DegreeManagement.CoreAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

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
        private DanTocCL _danTocCL;
        private MonThiCL _monThiCL;
        private HocSinhTmpCl _hocSinhTmpCl;
        private HocSinhCL _hocSinhCache;

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
            _danTocCL = new DanTocCL(cacheService, configuration);
            _monThiCL = new MonThiCL(cacheService, configuration);
            _mapper = mapper;
            _hocSinhTmpCl = new HocSinhTmpCl(cacheService, configuration);
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



        #region Import

        /// <summary>
        /// Thêm danh sách học sinh từ excel
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost("ImportVanBang")]
        [AllowAnonymous]
        public async Task<IActionResult> ImportVanBang([FromForm] HocSinhImportModel model)
        {
            try
            {
                //Kiểm tra định dạng file excel
                var checkFile = FileHelper.CheckValidFileExcel(model.FileExcel);
                if (checkFile.Item1 == (int)ErrorFileEnum.InvalidFileImage)
                {
                    return ResponseHelper.BadRequest(checkFile.Item2);
                }

                //Đọc file excel
                DataTable dataFromFile = ReadExcelData(model.FileExcel);

                var monThis = _monThiCL.GetAllMaMonThi();
                var danTocs = _danTocCL.GetAllTenDanToc();
                var hocSinhs = _hocSinhCache.GetAllHocSinhDaCoSoHieu();
                string[] arrayCccd = hocSinhs.Select(x => x.CCCD).OrderByDescending(x => x).ToArray();
                string[] arraySoHieu = hocSinhs.Select(x => x.SoHieuVanBang).OrderByDescending(x => x).ToArray();
                string[] arraySoVaoSo = hocSinhs.Select(x => x.SoVaoSoCapBang).OrderByDescending(x => x).ToArray();

                var validationRules = HocSinhValidationRules.GetRuleHocSinhOld(monThis, danTocs, arrayCccd, arraySoHieu, arraySoVaoSo);
                //// Kiểm tra dữ liệu
                DataTable checkValue = CheckValidateDataTable(dataFromFile, validationRules);

                //Map từ datatable sang List<HocSinhImportViewModel>
                List<HocSinhImportVM> hocSinhImports = ModelProvider.CreateListFromTable<HocSinhImportVM>(checkValue);
                hocSinhImports.ForEach(hocSinh =>
                {
                    hocSinh.IdTruong = model.IdTruong;
                    hocSinh.IdDanhMucTotNghiep = model.IdDanhMucTotNghiep;
                    hocSinh.NgayTao = DateTime.Now;
                    hocSinh.NguoiTao = model.NguoiThucHien;
                    hocSinh.IdKhoaThi = model.IdKhoaThi;
                    hocSinh.TrangThai = TrangThaiHocSinhEnum.DaNhanBang;
                    hocSinh.KetQuaHocTaps = new List<KetQuaHocTapModel>
                    {
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon1, Diem = hocSinh.DiemMon1 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon2, Diem = hocSinh.DiemMon2 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon3, Diem = hocSinh.DiemMon3 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon4, Diem = hocSinh.DiemMon4 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon5, Diem = hocSinh.DiemMon5 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon6, Diem = hocSinh.DiemMon6 ?? 0 }
                    };
                });

                List<HocSinhTmpModel> hocSinhTmp = _mapper.Map<List<HocSinhTmpModel>>(hocSinhImports);

                var response = await _hocSinhTmpCl.SaveImport(hocSinhTmp);
                if (response == (int)HocSinhEnum.Fail)
                    return ResponseHelper.BadRequest(_localizer.GetImportErrorMessage(NameControllerEnum.HocSinh.ToStringValue()));
                return ResponseHelper.Success(_localizer.GetImportSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResponseHelper.Error500(ex.Message);
            }
        }

        private DataTable CheckValidateDataTable(DataTable dataFromFile, List<DataTableValidatorHelper.ValidationRule> validationRules)
        {
            throw new NotImplementedException();
        }

        [HttpGet("GetSearchHocSinhTmp")]
        [AllowAnonymous]
        public IActionResult GetSearchHocSinhTmp([FromQuery] HocSinhTmpModelParamModel model)
        {
            int total;

            var data = _hocSinhTmpCl.GetSearchHocSinhTmp(out total, model);

            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetThongKeHocSinhTmp")]
        [AllowAnonymous]
        public IActionResult GetThongKeHocSinhTmp(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {

            var data = _hocSinhTmpCl.GetThongKeHocSinhTmp(idTruong, nguoiThucHien, idDanhMucTotNghiep);
            return ResponseHelper.Ok(data);
        }

        [HttpPost("DeleteImport")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteImport(string nguoiThucHien)
        {

            var response = await _hocSinhTmpCl.Delete(nguoiThucHien);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Xóa Thất BẠI");
            return ResponseHelper.Success("Xóa thành công");
        }


        [HttpPost("SaveImport")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveImport(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {

            var hocSinhs = _hocSinhTmpCl.GetAllHocSinhTmpSave(idTruong, nguoiThucHien, idDanhMucTotNghiep);
            if (hocSinhs.Where(x => x.ErrorCode == -1).Count() > 0) return ResponseHelper.BadRequest("Cố các hàng bị lỗi");

            List<HocSinhModel> hocSinhTmp = _mapper.Map<List<HocSinhModel>>(hocSinhs);

            var response = await _hocSinhCache.SaveImport(idTruong, idDanhMucTotNghiep, hocSinhTmp);

            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Thêm danh sách học sinh thất bại");
            if (response.MaLoi == (int)HocSinhEnum.NotExistPhoi)
                return ResponseHelper.BadRequest("Phôi gốc không tồn tại");
            return ResponseHelper.Success("Thêm danh sách học sinh thành công");
        }

        #endregion

        private DataTable ReadExcelData(IFormFile file)
        {
            var dataTable = new DataTable();

            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Lấy sheet đầu tiên

                // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
                IRow headerRow = sheet.GetRow(0);
                foreach (var cell in headerRow.Cells)
                {
                    dataTable.Columns.Add(cell.ToString());
                }

                // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    // Tạo một hàng mới trong DataTable
                    DataRow dataRow = dataTable.NewRow();

                    // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
                    for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                    {
                        ICell cell = row.GetCell(cellIndex);
                        dataRow[cellIndex] = cell?.ToString() ?? string.Empty;
                    }

                    // Thêm DataRow vào DataTable
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }


    }
}
