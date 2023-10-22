using Amazon.Runtime.Internal.Util;
using CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang
{
    public class ChinhSuaVanBangCL
    {
        private string _masterCacheKey = "LichSuChinhSuaVanBangCL";
        private string _masterCacheKeyHocSinh = "HocSinhCache";

        private CacheLayer _cache;
        private ChinhSuaVanBangBL _BL;

        public ChinhSuaVanBangCL(ICacheService cacheService, IConfiguration configuration)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            _BL = new ChinhSuaVanBangBL(configuration);
        }

        public async Task<int> Create(ChinhSuaVanBangInputModel model)
        {

            var result = await _BL.Create(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
                _cache.InvalidateCache(_masterCacheKeyHocSinh);
            }

            return result;
        }

        public List<ChinhSuaVanBangModel> GetSearchLichSuChinhSuaVanBang(out int total,string idHocSinh ,SearchParamModel modelSearch)
        {
            string objectKey = EHashMd5.FromObject(modelSearch) + idHocSinh;
            string rawKey = string.Concat("HocSinhs-GetSearchLichSuChinhSuaVanBang-", objectKey);
            string rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            int? cacheTotal = _cache.GetCacheKey<int>(rawKeyTotal);
            total = cacheTotal ?? 0;

            // See if the item is in the cache
            List<ChinhSuaVanBangModel> hocSinhs = _cache.GetCacheKey<List<ChinhSuaVanBangModel>>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetSerachChinhSuaVanBang(out total, idHocSinh ,modelSearch);
            _cache.AddCacheItem(rawKey, hocSinhs);
            _cache.AddCacheItem(rawKeyTotal, total);
            return hocSinhs;
        }

        public ChinhSuaVanBangModel GetChinhSuaVanBangById(string cccd, string idLichSuChinhSua)
        {
            string objectKey = cccd + idLichSuChinhSua;
            string rawKey = string.Concat("HocSinhs-GetChinhSuaVanBangById-", objectKey);

            // See if the item is in the cache
            ChinhSuaVanBangModel hocSinhs = _cache.GetCacheKey<ChinhSuaVanBangModel>(rawKey, _masterCacheKey)!;
            if (hocSinhs != null) return hocSinhs;
            // Item not found in cache - retrieve it and insert it into the cache
            hocSinhs = _BL.GetChinhSuaVanBangById(cccd, idLichSuChinhSua);
            _cache.AddCacheItem(rawKey, hocSinhs);
            return hocSinhs;
        }

    }


}
