using Amazon.Runtime.Internal.Util;
using CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
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

            }

            return result;
        }
    }

   
}
