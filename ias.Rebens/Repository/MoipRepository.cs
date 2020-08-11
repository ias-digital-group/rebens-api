    using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class MoipRepository : IMoipRepository
    {
        private string _connectionString;
        public MoipRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public MoipSignature GetUserSignature(int idCustomer, out string error)
        {
            MoipSignature ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.MoipSignature.FirstOrDefault(c => c.IdCustomer == idCustomer &&
                                (c.Status.ToUpper() == "ACTIVE" || c.Status.ToUpper() == "TRIAL"));

                    if(ret == null)
                        ret = db.MoipSignature.Where(c => c.IdCustomer == idCustomer).OrderByDescending(c => c.Modified).FirstOrDefault();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipRepository.GetUserSignature", ex.Message, $"idCustomer:{idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar carregar a assinatura. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<MoipPayment> ListPaymentsByCustomer(int idCustomer, int page, int pageItems, out string error)
        {
            ResultPage<MoipPayment> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.MoipPayment.Include("Signature").Include("Invoice").Where(p => p.Signature.IdCustomer == idCustomer)
                        .OrderByDescending(p => p.Created).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.MoipPayment.Count(p => p.Signature.IdCustomer == idCustomer);

                    ret = new ResultPage<MoipPayment>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipRepository.ListPaymentsByCustomer", ex.Message, $"idCustomer:{idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os pagamentos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SaveInvoice(MoipInvoice invoice, string signatureCode)
        {
            var ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var signature = db.MoipSignature.SingleOrDefault(s => s.Code == signatureCode);
                    if(signature != null)
                    {
                        var update = db.MoipInvoice.SingleOrDefault(i => i.Id == invoice.Id);
                        if (update != null)
                        {
                            update.Amount = invoice.Amount;
                            update.IdStatus = invoice.IdStatus;
                            update.Modified = DateTime.Now;
                            update.Occurrence = invoice.Occurrence;
                            update.Status = invoice.Status;
                        }
                        else {
                            invoice.IdMoipSignature = signature.Id;
                            db.MoipInvoice.Add(invoice);
                        }
                        db.SaveChanges();
                    }

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipRepository.SaveInvoice", ex.Message, "", ex.StackTrace);
                ret = false;
            }

            return ret;
        }

        public bool SavePayment(MoipPayment payment, string signatureCode)
        {
            var ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var signature = db.MoipSignature.SingleOrDefault(s => s.Code == signatureCode);
                    if (signature != null)
                    {
                        var update = db.MoipPayment.SingleOrDefault(i => i.Id == payment.Id);
                        if (update != null)
                        {
                            update.Amount = payment.Amount;
                            update.Brand = payment.Brand;
                            update.Description = payment.Description;
                            update.FirstSixDigits = payment.FirstSixDigits;
                            update.HolderName = payment.HolderName;
                            update.IdStatus = payment.IdStatus;
                            update.LastFourDigits = payment.LastFourDigits;
                            update.Modified = DateTime.Now;
                            update.MoipId = payment.MoipId;
                            update.PaymentMethod = payment.PaymentMethod;
                            update.Status = payment.Status;
                            update.Vault = payment.Vault;
                        }
                        else
                        {
                            payment.IdMoipSignature = signature.Id;
                            db.MoipPayment.Add(payment);
                        }
                        db.SaveChanges();
                    }

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipRepository.SavePayment", ex.Message, "", ex.StackTrace);
                ret = false;
            }

            return ret;
        }

        public bool SaveSignature(MoipSignature signature, out string error)
        {
            var ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.MoipSignature.SingleOrDefault(s => s.Code == signature.Code);
                    if(update != null)
                    {
                        update.Amount = signature.Amount;
                        update.ExpirationDate = signature.ExpirationDate;
                        update.Modified = DateTime.Now;
                        update.NextInvoiceDate = signature.NextInvoiceDate;
                        update.PaymentMethod = signature.PaymentMethod;
                        update.PlanCode = signature.PlanCode;
                        update.Status = signature.Status;
                        update.PlanName = signature.PlanName;
                    }
                    else
                        db.MoipSignature.Add(signature);

                    db.SaveChanges();
                    ret = true;
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipRepository.SaveSignature", ex.Message, "", ex.StackTrace);
                ret = false;
                error = $"Ocorreu um erro ao tentar salvar a assinatura. (erro: {idLog})";
            }

            return ret;
        }

        public bool UpdatePlan(string code, string planCode, string planName, decimal amount, DateTime nextInvoice, out string error)
        {
            var ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                    if (update != null)
                    {
                        update.PlanCode = planCode;
                        update.PlanName = planName;
                        update.Amount = amount;
                        update.NextInvoiceDate = nextInvoice;
                        update.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Assinatura não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipRepository.UpdatePlan", ex.Message, $"code: {code}", ex.StackTrace);
                ret = false;
                error = $"Ocorreu um erro ao tentar alterar o plano da assinatura. (erro: {idLog})";
            }

            return ret;
        }

        public bool CancelSignature(string code, out string error)
        {
            var ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                    if (update != null)
                    {
                        update.Status = "CANCELED";
                        update.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Assinatura não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipRepository.CancelSignature", ex.Message, $"code: {code}", ex.StackTrace);
                ret = false;
                error = $"Ocorreu um erro ao tentar cancelar a assinatura. (erro: {idLog})";
            }

            return ret;
        }

        public ResultPage<Entity.MoipSignatureItem> ListSubscriptions(int page, int pageItems, string word, out string error, string plan, string status, int? idOperation)
        {
            ResultPage<Entity.MoipSignatureItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    int? idInvoice = null;
                    if (int.TryParse(word, out int tmp))
                        idInvoice = tmp;

                    var tmpList = db.MoipSignature.Include("Customer").Where(s => (!idOperation.HasValue || s.IdOperation == idOperation)
                                    && (string.IsNullOrEmpty(word) || s.Customer.Name.Contains(word) || s.Code.Contains(word) || s.Customer.Surname.Contains(word))
                                    && (string.IsNullOrEmpty(plan) || s.PlanCode == plan)
                                    && (string.IsNullOrEmpty(status) || s.Status == status)
                                    );
                    var list = tmpList.OrderBy(s => s.Customer.Name).Skip(page * pageItems).Take(pageItems)
                                .Select(c => new Entity.MoipSignatureItem() { 
                                    Amount = c.Amount,
                                    Code = c.Code,
                                    Created = c.Created,
                                    CreationDate = c.CreationDate,
                                    Customer = c.Customer,
                                    ExpirationDate = c.ExpirationDate,
                                    Id = c.Id,
                                    IdCustomer = c.IdCustomer,
                                    IdOperation = c.IdOperation,
                                    Modified = c.Modified,
                                    NextInvoiceDate = c.NextInvoiceDate,
                                    PaymentMethod = c.PaymentMethod,
                                    PlanCode = c.PlanCode,
                                    PlanName = c.PlanName,
                                    Status = c.Status
                                }).ToList();
                    var total = tmpList.Count();

                    list.ForEach(i =>
                    {
                        if(i.IdOperation > 0)
                            i.OperationName = db.Operation.Single(o => o.Id == i.IdOperation).Title;
                    });


                    ret = new ResultPage<Entity.MoipSignatureItem>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipRepository.ListSubscriptions", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as assinaturas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
