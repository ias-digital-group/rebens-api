﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Portal não logado model
    /// </summary>
    public class PortalHomeLockedModel
    {
        /// <summary>
        /// Lista dos banners full
        /// </summary>
        public List<PortalBannerModel> BannerFullList { get; set; }
        /// <summary>
        /// Lista dos banners imperdíveis
        /// </summary>
        public List<PortalBannerModel> BannerUnmissable { get; set; }
    }
}
