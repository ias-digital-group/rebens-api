using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class CouponCampaignRepository : ICouponCampaignRepository
    {
        private string _connectionString;
        public CouponCampaignRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(CouponCampaign couponCampaign, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    couponCampaign.Modified = couponCampaign.Created = DateTime.UtcNow;
                    db.CouponCampaign.Add(couponCampaign);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponCampaignRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o campanha de cupom. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public CouponCampaign Read(int id, out string error)
        {
            CouponCampaign ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CouponCampaign.SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponCampaignRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o campanha de cupom. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CouponCampaign couponCampaign, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CouponCampaign.SingleOrDefault(c => c.Id == couponCampaign.Id);
                    if (update != null)
                    {
                        update.CampaignId = couponCampaign.CampaignId;
                        update.Code = couponCampaign.Code;
                        update.Url = couponCampaign.Url;
                        update.Name = couponCampaign.Name;
                        update.Status = couponCampaign.Status;
                        update.Title = couponCampaign.Title;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Campanha não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CouponCampaignRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o campanha de cupom. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
