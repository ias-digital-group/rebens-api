using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICouponCampaignRepository
    {
        bool Create(CouponCampaign couponCampaign, out string error);

        bool Update(CouponCampaign couponCampaign, out string error);

        CouponCampaign Read(int id, out string error);
    }
}
