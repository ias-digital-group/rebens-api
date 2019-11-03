using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class WirecardPaymentRepository : IWirecardPaymentRepository
    {
        private string _connectionString;
        public WirecardPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public WirecardPaymentRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool Create(WirecardPayment wirecardPayment, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    wirecardPayment.Modified = wirecardPayment.Created = DateTime.UtcNow;
                    db.WirecardPayment.Add(wirecardPayment);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WirecardPaymentRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o pagamento. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public WirecardPayment Read(string id, out string error)
        {
            WirecardPayment ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.WirecardPayment.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WirecardPaymentRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o pagamento. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(WirecardPayment wirecardPayment, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.WirecardPayment.SingleOrDefault(c => c.Id == wirecardPayment.Id);
                    if (update != null)
                    {
                        update.Status = wirecardPayment.Status;
                        update.Amount = wirecardPayment.Amount;
                        update.Method = wirecardPayment.Method;
                        update.BilletLineCode = wirecardPayment.BilletLineCode;
                        update.BilletLink = wirecardPayment.BilletLink;
                        update.BilletLinkPrint = wirecardPayment.BilletLinkPrint;
                        update.CardBrand = wirecardPayment.CardBrand;
                        update.CardFirstSix = wirecardPayment.CardFirstSix;
                        update.CardLastFour = wirecardPayment.CardLastFour;
                        update.CardHolderName = wirecardPayment.CardHolderName;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Pagamento não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WirecardPaymentRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o pagamento. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool HasPaymentToProcess()
        {
            bool ret = false;
            try
            {
                using(var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.UtcNow.AddMinutes(-15);
                    ret = db.WirecardPayment.Any(p => (p.Status == "CREATED" || p.Status == "WAITING" || p.Status == "IN_ANALYSIS")
                                && p.Modified < dt);
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WirecardPaymentRepository.HasPaymentToProcess", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public void ProcessPayments()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.UtcNow.AddMinutes(-15);
                    var list = db.WirecardPayment
                        .Where(p => (p.Status == "CREATED" || p.Status == "WAITING" || p.Status == "IN_ANALYSIS") && p.Modified < dt)
                        .OrderBy(p => p.Modified).Take(10);
                    var wcHelper = new Integration.WirecardHelper();
                    foreach(var item in list)
                    {
                        wcHelper.CheckPaymentStatus(item);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("WirecardPaymentRepository.ProcessPayments", ex.Message, "", ex.StackTrace);
            }
        }
    }
}
