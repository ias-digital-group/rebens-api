using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICouponRepository
    {
        bool Create(Coupon coupon, out string error);

        bool Update(Coupon coupon, out string error);

        Coupon Read(int id, out string error);
    }
}
