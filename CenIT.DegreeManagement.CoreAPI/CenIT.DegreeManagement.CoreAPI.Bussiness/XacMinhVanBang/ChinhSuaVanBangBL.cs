using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang
{
    public class ChinhSuaVanBangBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public ChinhSuaVanBangBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(ChinhSuaVanBangInputModel model)
        {
            try
            {
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == model.IdHocSinh && t.TrangThai == TrangThaiHocSinhEnum.DaNhanBang).FirstOrDefault();

                if (hocSinh == null) return (int)LichSuChinhSuaVanBangEnum.NotExist;

                var chinhSuaVanBang = new PhuLucSoGocModel();
                ModelProvider.MapProperties(model, chinhSuaVanBang);
                chinhSuaVanBang.NguoiTao = model.NguoiThucHien;
                chinhSuaVanBang.NgayTao = DateTime.Now;
                chinhSuaVanBang.SoHieuVanBangCu = hocSinh.SoHieuVanBang;
                chinhSuaVanBang.SoVaoSoCapBangCu = hocSinh.SoVaoSoCapBang;
                chinhSuaVanBang.IdHocSinh = hocSinh.Id;

                if (model.SoHieuVanBang != hocSinh.SoHieuVanBang)
                {
                    if(KiemTraSoHieuVanBang(model.SoHieuVanBang, hocSinh.Id)) return (int)LichSuChinhSuaVanBangEnum.SoHieuExist;
                    chinhSuaVanBang.SoHieuVanBangCapLai = model.SoHieuVanBang;
                }

                if (model.SoVaoSoCapBang != hocSinh.SoVaoSoCapBang)
                {
                    if (KiemTraSoVaoSo(model.SoVaoSoCapBang, hocSinh.Id)) return (int)LichSuChinhSuaVanBangEnum.SoVaoSoExist;
                    chinhSuaVanBang.SoVaoSoCapBangCapLai = model.SoVaoSoCapBang;
                }


                await _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName).InsertOneAsync(chinhSuaVanBang);
                if (chinhSuaVanBang.Id != null)
                {
                    return (int)LichSuChinhSuaVanBangEnum.Success;

                }
                return (int)LichSuChinhSuaVanBangEnum.Fail;
            }
            catch
            {
                return (int)LichSuChinhSuaVanBangEnum.Fail;

            }
        }

        public List<PhuLucSoGocModel> GetSerachChinhSuaVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            var conllectionPhuLucSoGoc = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName);
            var hocSinh = conllectionPhuLucSoGoc.Find(t => t.Xoa == false && t.IdHocSinh == idHocSinh).ToList();

            //var chinhSuaVanBang = hocSinh.LichSuChinhSuaVanBang.OrderBy(x=>x.NgayTao).ToList();

            total = hocSinh.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.NgayTao).ToList()
                        : hocSinh.OrderByDescending(x => x.NgayTao).ToList();
                    break;

                case "1":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.NgayTao).ToList()
                        : hocSinh.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                hocSinh = hocSinh.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinh;
        }

        public LichSuChinhSuaModel GetChinhSuaVanBangById(string idPhuLuc)
        {
            var conllectionPhuLucSoGoc = _mongoDatabase.GetCollection<LichSuChinhSuaModel>(_collectionPhuLucSoGocName);
            var hocSinh = conllectionPhuLucSoGoc.Find(t => t.Xoa == false && t.Id == idPhuLuc).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var htdtCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
            hocSinh = hocSinh
                      .Join(
                          htdtCollection.AsQueryable(),
                          hs => hs.MaHTDT,
                          htdt => htdt.Ma,
                          (hs, htdt) =>
                          {
                              hs.TenHinhThucDaoTao = htdt.Ten;
                              return hs;
                          }
                      )
                      .Join(
                          namThiCollection.AsQueryable(),
                          hs => hs.IdNamThi,
                          nt => nt.Id,
                          (hs, nt) =>
                          {
                              hs.NamThi = nt.Ten;
                              hs.KhoaThi = nt.KhoaThis.Where(x=>x.Id == hs.IdKhoaThi).FirstOrDefault().Ngay;
                              return hs;
                          }
                      ).ToList();
                 
            return hocSinh.FirstOrDefault();
        }

        #region Phụ Lục
        public List<PhuLucSoGocModel> GetSerachPhuLuc(out int total, string namThi, SearchParamModel modelSearch)
        {

            var phuLucs = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName)
                                .Find(x=>x.Xoa == false)
                                .ToList();

            if (!string.IsNullOrEmpty(namThi))
            {
                var namThiValue = !string.IsNullOrEmpty(namThi) ? Int32.Parse(namThi) : 0;

                phuLucs = phuLucs.Where(x => x.NgayTao.Year == namThiValue).ToList();
            }

          
            total = phuLucs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    phuLucs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    phuLucs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                phuLucs = phuLucs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return phuLucs;
        }
        #endregion


        private bool KiemTraSoHieuVanBang(string soHieuVanBang, string idHocSinh)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);

            var checkSoHieuVanBang = conllectionHocSinh.Find(t => t.Xoa == false && t.Id != idHocSinh && t.SoHieuVanBang == soHieuVanBang).FirstOrDefault();

            return checkSoHieuVanBang != null ? true : false;
        }

        private bool KiemTraSoVaoSo(string soVaoSo, string idHocSinh)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);

            var checkSoHieuVanBang = conllectionHocSinh.Find(t => t.Xoa == false && t.Id != idHocSinh && t.SoVaoSoCapBang == soVaoSo).FirstOrDefault();

            return checkSoHieuVanBang != null ? true : false;
        }


    }
}
