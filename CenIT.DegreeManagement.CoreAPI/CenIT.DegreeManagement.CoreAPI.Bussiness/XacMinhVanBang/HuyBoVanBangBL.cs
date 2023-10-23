using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Models;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang
{
    public class HuyBoVanBangBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public HuyBoVanBangBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(HuyBoVangBangInputModel model)
        {

            try
            {
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == model.IdHocSinh && t.TrangThai == TrangThaiHocSinhEnum.DaNhanBang).FirstOrDefault();

                if (hocSinh == null) return (int)LichSuHuyBoVanBangEnum.NotExist;

                var huyBoVB = new HuyBoVangBangModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    PathFileVanBan = model.PathFileVanBan,
                    LyDo = model.LyDo,
                    NguoiTao = model.NguoiThucHien,
                    NgayTao = DateTime.Now
                };

                hocSinh.TrangThai = TrangThaiHocSinhEnum.HuyBo;
                hocSinh.LichSuHuyBoVanBang.Add(huyBoVB);

                var updateResult = await conllectionHocSinh.ReplaceOneAsync(h => h.Id == hocSinh.Id, hocSinh);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)LichSuHuyBoVanBangEnum.Fail;
                }
                return (int)LichSuHuyBoVanBangEnum.Success;

            }
            catch
            {
                return (int)LichSuHuyBoVanBangEnum.Fail;

            }
        }

        public List<HuyBoVangBangModel> GetSerachHuyBoVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == idHocSinh).FirstOrDefault();

            var huyBoVangBangs = hocSinh.LichSuHuyBoVanBang.OrderBy(x => x.NgayTao).ToList();

            total = huyBoVangBangs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    huyBoVangBangs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? huyBoVangBangs.OrderBy(x => x.NgayTao).ToList()
                        : huyBoVangBangs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                huyBoVangBangs = huyBoVangBangs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return huyBoVangBangs;
        }

        public HuyBoVangBangModel GetHuyBoVanBangById(string cccd, string idLichSuHuyBo)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.CCCD == cccd).FirstOrDefault();

            var huyBoVangBangs = hocSinh.LichSuHuyBoVanBang.OrderBy(x => x.NgayTao).ToList();
            var huyBoVangBang = huyBoVangBangs.Where(x => x.Id == idLichSuHuyBo).FirstOrDefault();


            return huyBoVangBang;
        }

    }
}
