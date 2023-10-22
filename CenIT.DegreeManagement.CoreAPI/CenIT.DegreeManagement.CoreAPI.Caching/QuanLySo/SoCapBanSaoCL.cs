using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.QuanLySo
{
    public class SoCapBanSaoCL
    {
        private string _masterCacheKey = "SoCapBaSaoCache";
        private CacheLayer _cache;
        private SoCapBanSaoBL _BL;
        public SoCapBanSaoCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            _BL = new SoCapBanSaoBL(configuration);
        }

        public string GetHocSinhTheoCapBanSao(TruongViewModel truong ,DanhMucTotNghiepViewModel dmtn, SearchParamModel paramModel)
        {
            var hashKey = EHashMd5.FromObject(truong) + EHashMd5.FromObject(dmtn) + EHashMd5.FromObject(paramModel);
            var rawKey = string.Concat("SoBanSao-GetHocSinhTheoCapBanSao-", hashKey);
            var soBanSaoVM = _cache.GetCacheKey<string>(rawKey, _masterCacheKey)!;
            if (soBanSaoVM == null)
            {
                soBanSaoVM = _BL.GetHocSinhTheoCapBanSao(truong, dmtn, paramModel);
                _cache.AddCacheItem(rawKey, soBanSaoVM, _masterCacheKey);
            }
            return soBanSaoVM;
        }
    }
}
