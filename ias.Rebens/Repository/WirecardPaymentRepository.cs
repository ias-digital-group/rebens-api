using Amazon.Route53.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
                                && p.Modified < dt);                }
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
                        .OrderBy(p => p.Modified).Take(10).ToList();
                    var wcHelper = new Integration.WirecardHelper();
                    //var staticText = db.StaticText.Where(t => t.IdOperation == 1 && t.IdStaticTextType == (int)Enums.StaticTextType.Email && t.Active).OrderByDescending(t => t.Modified).FirstOrDefault();
                    //var operation = db.Operation.Single(o => o.Id == 1);
                    foreach (var item in list)
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

        public void ProcessSignatures()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var wcHelper = new Integration.WirecardHelper();
                    var list = wcHelper.ListSubscriptions();
                    foreach(var item in list)
                    {
                        var subscription = db.MoipSignature.SingleOrDefault(s => s.Code == item.Code);
                        if (subscription != null)
                        {
                            subscription.Amount = item.Amount;
                            subscription.PlanCode = item.PlanCode;
                            subscription.ExpirationDate = item.ExpirationDate;
                            subscription.NextInvoiceDate = item.NextInvoiceDate;
                            subscription.PaymentMethod = item.PaymentMethod;
                            subscription.Status = item.Status;
                            subscription.Modified = DateTime.UtcNow;

                            var tmpInvoice = item.Invoices.First();
                            var invoice = db.MoipInvoice.SingleOrDefault(i => i.Id == tmpInvoice.Id);
                            if(invoice != null)
                            {
                                invoice.Amount = tmpInvoice.Amount;
                                invoice.IdStatus = tmpInvoice.IdStatus;
                                invoice.Status = tmpInvoice.Status;
                                invoice.Modified = DateTime.UtcNow;
                                invoice.Occurrence++;

                                var tmpPayment = tmpInvoice.Payments.First();
                                if (tmpPayment != null)
                                {
                                    var payment = db.MoipPayment.SingleOrDefault(p => p.MoipId == tmpPayment.MoipId);
                                    if (payment != null)
                                    {
                                        payment.Amount = tmpPayment.Amount;
                                        payment.Brand = tmpPayment.Brand;
                                        payment.Description = tmpPayment.Description;
                                        payment.FirstSixDigits = tmpPayment.FirstSixDigits;
                                        payment.HolderName = tmpPayment.HolderName;
                                        payment.IdStatus = tmpPayment.IdStatus;
                                        payment.LastFourDigits = tmpPayment.LastFourDigits;
                                        payment.Modified = DateTime.UtcNow;
                                        payment.PaymentMethod = tmpPayment.PaymentMethod;
                                        payment.Status = tmpPayment.Status;
                                        payment.Vault = tmpPayment.Vault;
                                    }
                                    else
                                    {
                                        tmpPayment.Created = tmpPayment.Modified = DateTime.UtcNow;
                                        tmpPayment.IdMoipSignature = subscription.Id;
                                        tmpPayment.IdMoipInvoice = invoice.Id;
                                        db.MoipPayment.Add(tmpPayment);
                                    }
                                }
                            }
                            else
                            {
                                var tmpPayment = tmpInvoice.Payments.First();
                                tmpInvoice.Created = tmpInvoice.Modified = DateTime.UtcNow;
                                tmpInvoice.IdMoipSignature = subscription.Id;
                                tmpInvoice.Payments = null;
                                db.MoipInvoice.Add(tmpInvoice);
                                db.SaveChanges();

                                tmpPayment.Created = tmpPayment.Modified = DateTime.UtcNow;
                                tmpPayment.IdMoipSignature = subscription.Id;
                                tmpPayment.IdMoipInvoice = tmpInvoice.Id;
                                db.MoipPayment.Add(tmpPayment);
                            }
                            db.SaveChanges();
                        }
                        else
                        {
                            var customer = db.Customer.SingleOrDefault(c => c.Cpf == item.Customer.Cpf && c.IdOperation == 73);
                            if (customer != null)
                            {
                                var tmpSignature = item;
                                var tmpInvoice = item.Invoices.First();
                                var tmpPayment = item.Invoices.First().Payments.First();

                                tmpSignature.Customer = null;
                                tmpSignature.IdCustomer = customer.Id;
                                tmpSignature.IdOperation = 73;
                                tmpSignature.Invoices = null;
                                tmpSignature.Payments = null;
                                db.MoipSignature.Add(tmpSignature);
                                db.SaveChanges();
                                
                                tmpInvoice.Created = tmpInvoice.Modified = DateTime.UtcNow;
                                tmpInvoice.IdMoipSignature = tmpSignature.Id;
                                tmpInvoice.Payments = null;
                                db.MoipInvoice.Add(tmpInvoice);
                                db.SaveChanges();
                                
                                tmpPayment.IdMoipInvoice = tmpInvoice.Id;
                                tmpPayment.IdMoipSignature = tmpSignature.Id;
                                tmpPayment.Created = tmpPayment.Modified = DateTime.UtcNow;
                                db.MoipPayment.Add(tmpPayment);
                                db.SaveChanges();
                            }
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("WirecardPaymentRepository.ProcessSignatures", ex.Message, "", ex.StackTrace);
                if(ex.InnerException != null)
                {
                    logError.Create("WirecardPaymentRepository.ProcessSignatures - INNER", ex.InnerException.Message, "", ex.InnerException.Message);
                }
                
            }
        }
    }
}
