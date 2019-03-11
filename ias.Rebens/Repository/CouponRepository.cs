using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ias.Rebens
{
    public class CouponRepository : ICouponRepository
    {
        private string _connectionString;
        public CouponRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public CouponRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Create(Coupon coupon, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    coupon.Modified = coupon.Created = DateTime.UtcNow;
                    db.Coupon.Add(coupon);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cupom. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Coupon> ListPageByCustomer(int idCustomer, int page, int pageItems, out string error)
        {
            ResultPage<Coupon> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.Coupon.Include("CouponCampaign")
                        .Where(c => c.IdCustomer == idCustomer)
                        .OrderByDescending(c => c.Created)
                        .Skip(page * pageItems)
                        .Take(pageItems).ToList();
                    var total = db.Coupon.Count(c => c.IdCustomer == idCustomer);

                    ret = new ResultPage<Coupon>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os cupons. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Coupon Read(int id, out string error)
        {
            Coupon ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Coupon.SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o cupom. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Coupon coupon, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Coupon.SingleOrDefault(c => c.Id == coupon.Id);
                    if (update != null)
                    {
                        update.SingleUseCode = coupon.SingleUseCode;
                        update.SingleUseUrl = coupon.SingleUseUrl;
                        update.WidgetValidationCode = coupon.WidgetValidationCode;
                        update.OpenDate = coupon.OpenDate;
                        update.PlayedDate = coupon.PlayedDate;
                        update.ClaimDate = coupon.ClaimDate;
                        update.ClaimType = coupon.ClaimType;
                        update.ValidationDate = coupon.ValidationDate;
                        update.ValidationValue = coupon.ValidationValue;
                        update.VoidedDate = coupon.VoidedDate;
                        update.Locked = coupon.Locked;
                        update.Value = coupon.Value;
                        update.SequenceId = coupon.SequenceId;
                        update.Status = coupon.Status;
                        update.VerifiedDate = coupon.VerifiedDate;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Cupom não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o cupom. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
