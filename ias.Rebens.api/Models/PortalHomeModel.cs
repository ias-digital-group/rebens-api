using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Portal logado
    /// </summary>
    public class PortalHomeModel
    {
        /// <summary>
        /// Lista dos banners full
        /// </summary>
        public List<PortalBannerModel> BannerFullList { get; set; }
        /// <summary>
        /// Lista dos banners imperdíveis
        /// </summary>
        public List<PortalBannerModel> BannerUnmissable { get; set; }
        /// <summary>
        /// Lista com os benefícios da home
        /// </summary>
        public List<PortalBenefitModel> Benefits { get; set; }
    }
}
