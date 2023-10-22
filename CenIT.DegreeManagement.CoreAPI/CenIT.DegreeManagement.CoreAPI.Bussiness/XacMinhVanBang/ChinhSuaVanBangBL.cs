using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
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

                var chinhSuaVanBang = new ChinhSuaVanBangModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    DanTocCu = hocSinh.DanToc,
                    HoTenCu = hocSinh.HoTen,
                    CCCDCu = hocSinh.CCCD,
                    NgaySinhCu = hocSinh.NgaySinh,
                    NoiSinhCu = hocSinh.NoiSinh,
                    GioiTinhCu = hocSinh.GioiTinh,
                    PathFileVanBan = model.PathFileVanBan,
                    LyDo = model.LyDo,
                };

                hocSinh.LichSuChinhSuaVanBang.Add(chinhSuaVanBang);

                hocSinh.DanToc = model.DanToc;
                hocSinh.HoTen = model.HoTen;
                hocSinh.CCCD = model.CCCD;
                hocSinh.NgaySinh = model.NgaySinh;
                hocSinh.NoiSinh = model.NoiSinh;
                hocSinh.GioiTinh = model.GioiTinh;

                var updateResult = await conllectionHocSinh.ReplaceOneAsync(h => h.Id == hocSinh.Id, hocSinh);


                if (updateResult.ModifiedCount == 0)
                {
                    return (int)LichSuChinhSuaVanBangEnum.Fail;
                }
                return (int)LichSuChinhSuaVanBangEnum.Success;

            }
            catch
            {
                return (int)LichSuChinhSuaVanBangEnum.Fail;

            }
        }

        public List<ChinhSuaVanBangModel> GetSerachChinhSuaVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == idHocSinh).FirstOrDefault();

            var chinhSuaVanBang = hocSinh.LichSuChinhSuaVanBang.OrderBy(x=>x.NgayTao).ToList();

            total = chinhSuaVanBang.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    chinhSuaVanBang = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? chinhSuaVanBang.OrderBy(x => x.NgayTao).ToList()
                        : chinhSuaVanBang.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                chinhSuaVanBang = chinhSuaVanBang.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return chinhSuaVanBang;
        }

        public ChinhSuaVanBangModel GetChinhSuaVanBangById(string cccd, string idLichSuChinhSua)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.CCCD == cccd).FirstOrDefault();

            var chinhSuaVanBangLists = hocSinh.LichSuChinhSuaVanBang.OrderBy(x => x.NgayTao).ToList();
            var chinhSuaVanBang = chinhSuaVanBangLists.Where(x => x.Id == idLichSuChinhSua).FirstOrDefault();

           
            return chinhSuaVanBang;
        }
    }
}
