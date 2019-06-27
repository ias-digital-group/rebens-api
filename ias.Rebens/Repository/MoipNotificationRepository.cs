using System;
using System.Collections.Generic;
using ias.Rebens.Enums;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace ias.Rebens
{
    public class MoipNotificationRepository : IMoipNotificationRepository
    {
        private string _connectionString;
        public MoipNotificationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public MoipNotificationRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool Create(MoipNotification notification, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    notification.Modified = notification.Created = DateTime.UtcNow;
                    db.MoipNotification.Add(notification);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipNotificationRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a notificação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<MoipNotification> ListPage(int page, int pageItems, out string error, MoipNotificationStatus? status = null, MoipNotificationEvent? notificationEvent = null)
        {
            ResultPage<MoipNotification> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter = null;
                    if (notificationEvent.HasValue)
                        filter = Enums.EnumHelper.GetEnumDescription(notificationEvent.Value);
                    
                    var tmp = db.MoipNotification.Where(n => (string.IsNullOrEmpty(filter) || n.Event == filter)
                                    && (!status.HasValue || (status.HasValue && n.Status == (int)status.Value)));
                    var total = tmp.Count();
                    var list = tmp.OrderByDescending(n => n.Created).Skip(page * pageItems).Take(pageItems).ToList();

                    ret = new ResultPage<MoipNotification>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipNotificationRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as notificações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<MoipNotification> ListToProcess(MoipNotificationEvent notificationEvent, out string error)
        {
            List<MoipNotification> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter = Enums.EnumHelper.GetEnumDescription(notificationEvent);

                    ret = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created).Take(30).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipNotificationRepository.ListToProcess", ex.Message, $"notificationEvent: {notificationEvent.ToString()}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as notificações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool UpdateStatus(int id, MoipNotificationStatus status, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.MoipNotification.SingleOrDefault(n => n.Id == id);
                    if(update != null)
                    {
                        update.Status = (int)status;
                        update.Modified = DateTime.Now;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Notificação não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("MoipNotificationRepository.UpdateStatus", ex.Message, $"id:{id}, status:{status.ToString()}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o status da notificação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool HasSubscriptionToProcess()
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter1 = EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionCreated);
                    string filter2 = EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionUpdated);

                    ret = db.MoipNotification.Any(n => n.Status == (int)MoipNotificationStatus.New && (n.Event == filter1 || n.Event == filter2));
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipNotificationRepository.HasSubscriptionToProcess", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public bool HasInvoicesToProcess()
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter1 = EnumHelper.GetEnumDescription(MoipNotificationEvent.InvoiceCreated);
                    string filter2 = EnumHelper.GetEnumDescription(MoipNotificationEvent.InvoiceStatusUpdated);

                    ret = db.MoipNotification.Any(n => n.Status == (int)MoipNotificationStatus.New && (n.Event == filter1 || n.Event == filter2));
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipNotificationRepository.HasInvoicesToProcess", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public bool HasPaymentsToProcess()
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter1 = EnumHelper.GetEnumDescription(MoipNotificationEvent.PaymentCreated);
                    string filter2 = EnumHelper.GetEnumDescription(MoipNotificationEvent.PaymentStatusUpdated);

                    ret = db.MoipNotification.Any(n => n.Status == (int)MoipNotificationStatus.New && (n.Event == filter1 || n.Event == filter2));
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipNotificationRepository.HasPaymentsToProcess", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public void ProcessSubscription()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionCreated);
                    var list = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if (list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string cpf = jObj["customer"]["code"].ToString();
                                var customer = db.Customer.SingleOrDefault(c => c.Cpf == cpf && c.IdOperation == item.IdOperation);
                                if (customer != null)
                                {
                                    string code = jObj["code"].ToString();
                                    MoipSignature signature = new MoipSignature();

                                    signature.IdCustomer = customer.Id;
                                    signature.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                    signature.Code = code;
                                    signature.Created = DateTime.Now;
                                    signature.CreationDate = new DateTime(Convert.ToInt32(jObj["creation_date"]["year"].ToString()), Convert.ToInt32(jObj["creation_date"]["month"].ToString()), Convert.ToInt32(jObj["creation_date"]["day"].ToString()));
                                    signature.Modified = DateTime.Now;
                                    signature.NextInvoiceDate = new DateTime(Convert.ToInt32(jObj["next_invoice_date"]["year"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["month"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["day"].ToString()));
                                    signature.PaymentMethod = jObj["payment_method"].ToString();
                                    signature.PlanCode = jObj["plan"]["code"].ToString();
                                    signature.Status = jObj["status"].ToString();
                                    signature.IdOperation = item.IdOperation;
                                    if(jObj["expiration_date"] != null)
                                        signature.ExpirationDate = new DateTime(Convert.ToInt32(jObj["expiration_date"]["year"].ToString()), Convert.ToInt32(jObj["expiration_date"]["month"].ToString()), Convert.ToInt32(jObj["expiration_date"]["day"].ToString()));
                                    else
                                        signature.ExpirationDate = signature.NextInvoiceDate;

                                    db.MoipSignature.Add(signature);

                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    item.Status = (int)MoipNotificationStatus.Ignored;
                                    item.Modified = DateTime.UtcNow;
                                }
                            }
                            catch(Exception ex)
                            {
                                item.Status = (int)MoipNotificationStatus.Error;
                                item.Modified = DateTime.UtcNow;

                                db.LogError.Add(new LogError()
                                {
                                    Reference = "ProcessSubscription",
                                    Complement = "id:" + item.Id,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace,
                                    Created = DateTime.UtcNow
                                });
                            }
                            Thread.Sleep(100);
                        }

                        db.SaveChanges();
                        Thread.Sleep(500);
                    }

                    filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionUpdated);
                    var list2 = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if (list2 != null && list2.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            try { 
                                var jObj = JObject.Parse(item.Resources);
                                string cpf = jObj["customer"]["code"].ToString();
                                var customer = db.Customer.SingleOrDefault(c => c.Cpf == cpf && c.IdOperation == item.IdOperation);
                                if (customer != null)
                                {
                                    string code = jObj["code"].ToString();
                                    var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                    if (signature != null)
                                    {
                                        signature.IdCustomer = customer.Id;
                                        signature.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                        signature.CreationDate = new DateTime(Convert.ToInt32(jObj["creation_date"]["year"].ToString()), Convert.ToInt32(jObj["creation_date"]["month"].ToString()), Convert.ToInt32(jObj["creation_date"]["day"].ToString()));
                                        signature.Modified = DateTime.Now;
                                        signature.NextInvoiceDate = new DateTime(Convert.ToInt32(jObj["next_invoice_date"]["year"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["month"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["day"].ToString()));
                                        signature.PaymentMethod = jObj["payment_method"].ToString();
                                        signature.PlanCode = jObj["plan"]["code"].ToString();
                                        signature.Status = jObj["status"].ToString();

                                        if (jObj["next_invoice_date"] != null)
                                            signature.ExpirationDate = new DateTime(Convert.ToInt32(jObj["expiration_date"]["year"].ToString()), Convert.ToInt32(jObj["expiration_date"]["month"].ToString()), Convert.ToInt32(jObj["expiration_date"]["day"].ToString()));
                                        else
                                            signature.ExpirationDate = signature.NextInvoiceDate;
                                    }
                                    else
                                    {
                                        signature = new MoipSignature();

                                        signature.IdCustomer = customer.Id;
                                        signature.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                        signature.Code = code;
                                        signature.Created = DateTime.Now;
                                        signature.CreationDate = new DateTime(Convert.ToInt32(jObj["creation_date"]["year"].ToString()), Convert.ToInt32(jObj["creation_date"]["month"].ToString()), Convert.ToInt32(jObj["creation_date"]["day"].ToString()));
                                        signature.Modified = DateTime.Now;
                                        signature.NextInvoiceDate = new DateTime(Convert.ToInt32(jObj["next_invoice_date"]["year"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["month"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["day"].ToString()));
                                        signature.PaymentMethod = jObj["payment_method"].ToString();
                                        signature.PlanCode = jObj["plan"]["code"].ToString();
                                        signature.Status = jObj["status"].ToString();
                                        signature.IdOperation = item.IdOperation;

                                        if (jObj["next_invoice_date"] != null)
                                            signature.ExpirationDate = new DateTime(Convert.ToInt32(jObj["expiration_date"]["year"].ToString()), Convert.ToInt32(jObj["expiration_date"]["month"].ToString()), Convert.ToInt32(jObj["expiration_date"]["day"].ToString()));
                                        else
                                            signature.ExpirationDate = signature.NextInvoiceDate;

                                        db.MoipSignature.Add(signature);
                                    }
                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    item.Status = (int)MoipNotificationStatus.Ignored;
                                    item.Modified = DateTime.UtcNow;
                                }

                            }
                            catch (Exception ex)
                            {
                                item.Status = (int)MoipNotificationStatus.Error;
                                item.Modified = DateTime.UtcNow;

                                db.LogError.Add(new LogError()
                                {
                                    Reference = "ProcessSubscription",
                                    Complement = "id:" + item.Id,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace,
                                    Created = DateTime.UtcNow
                                });
                            }

                            Thread.Sleep(100);
                        }

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipNotificationRepository.ProcessSubscription", ex.Message, "", ex.StackTrace);
            }
        }

        public void ProcessInvoices()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.InvoiceCreated);

                    var list = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if(list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["subscription_code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    var invoice = new MoipInvoice();
                                    invoice.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                    invoice.Created = DateTime.Now;
                                    invoice.Modified = DateTime.Now;
                                    invoice.Status = jObj["status"]["description"].ToString();
                                    invoice.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());
                                    invoice.Id = Convert.ToInt32(jObj["id"].ToString());
                                    invoice.Occurrence = 0;
                                    invoice.IdMoipSignature = signature.Id;

                                    db.MoipInvoice.Add(invoice);

                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    item.Status = (int)MoipNotificationStatus.Ignored;
                                    item.Modified = DateTime.UtcNow;
                                }
                            }
                            catch(Exception ex)
                            {
                                item.Status = (int)MoipNotificationStatus.Error;
                                item.Modified = DateTime.UtcNow;

                                db.LogError.Add(new LogError()
                                {
                                    Reference = "ProcessInvoices",
                                    Complement = "id:" + item.Id,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace,
                                    Created = DateTime.UtcNow
                                });
                            }

                            Thread.Sleep(100);
                        }
                        db.SaveChanges();

                        Thread.Sleep(500);
                    }
                    

                    filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.InvoiceStatusUpdated);
                    var list2 = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if(list2 != null && list2.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["subscription_code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    int invoiceId = Convert.ToInt32(jObj["id"].ToString());
                                    var invoice = db.MoipInvoice.SingleOrDefault(i => i.Id == invoiceId);
                                    if (invoice != null)
                                    {
                                        invoice.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                        invoice.Modified = DateTime.Now;
                                        invoice.Status = jObj["status"]["description"].ToString();
                                        invoice.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());
                                        invoice.Occurrence = 0;
                                    }
                                    else
                                    {
                                        invoice = new MoipInvoice();
                                        invoice.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                        invoice.Created = DateTime.Now;
                                        invoice.Modified = DateTime.Now;
                                        invoice.Status = jObj["status"]["description"].ToString();
                                        invoice.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());
                                        invoice.Id = invoiceId;
                                        invoice.Occurrence = 0;
                                        invoice.IdMoipSignature = signature.Id;

                                        db.MoipInvoice.Add(invoice);
                                    }
                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    item.Status = (int)MoipNotificationStatus.Ignored;
                                    item.Modified = DateTime.UtcNow;
                                }
                            }
                            catch (Exception ex)
                            {
                                item.Status = (int)MoipNotificationStatus.Error;
                                item.Modified = DateTime.UtcNow;

                                db.LogError.Add(new LogError()
                                {
                                    Reference = "ProcessInvoices",
                                    Complement = "id:" + item.Id,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace,
                                    Created = DateTime.UtcNow
                                });
                            }

                            Thread.Sleep(100);
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipNotificationRepository.ProcessInvoices", ex.Message, "", ex.StackTrace);
            }
        }

        public void ProcessPayments()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.PaymentCreated);

                    var list = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if (list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            try
                            {

                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["subscription_code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    var payment = new MoipPayment();
                                    payment.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                    payment.Created = DateTime.Now;
                                    payment.Modified = DateTime.Now;
                                    payment.Id = Convert.ToInt32(jObj["id"].ToString());
                                    payment.IdMoipInvoice = Convert.ToInt32(jObj["invoice_id"].ToString());
                                    payment.MoipId = Convert.ToInt32(jObj["moip_id"].ToString());
                                    payment.Status = jObj["status"]["description"].ToString();
                                    payment.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());
                                    payment.IdMoipSignature = signature.Id;

                                    if (jObj["payment_method"] != null)
                                    {
                                        payment.PaymentMethod = jObj["payment_method"]["code"].ToString();
                                        payment.Description = jObj["payment_method"]["description"].ToString();
                                        if (jObj["payment_method"]["credit_card"] != null)
                                        {
                                            payment.Brand = jObj["payment_method"]["credit_card"]["brand"].ToString();
                                            payment.HolderName = jObj["payment_method"]["credit_card"]["holder_name"].ToString();
                                            payment.FirstSixDigits = jObj["payment_method"]["credit_card"]["first_six_digits"].ToString();
                                            payment.LastFourDigits = jObj["payment_method"]["credit_card"]["last_four_digits"].ToString();
                                            payment.Vault = jObj["payment_method"]["credit_card"]["vault"].ToString();
                                        }
                                    }

                                    db.MoipPayment.Add(payment);

                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    item.Status = (int)MoipNotificationStatus.Ignored;
                                    item.Modified = DateTime.UtcNow;
                                }
                            }
                            catch (Exception ex)
                            {
                                item.Status = (int)MoipNotificationStatus.Error;
                                item.Modified = DateTime.UtcNow;

                                db.LogError.Add(new LogError()
                                {
                                    Reference = "ProcessPayments",
                                    Complement = "id:" + item.Id,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace,
                                    Created = DateTime.UtcNow
                                });

                                if(ex.InnerException != null)
                                {
                                    db.LogError.Add(new LogError()
                                    {
                                        Reference = "ProcessPayments - INNER",
                                        Complement = "id:" + item.Id,
                                        Message = ex.InnerException.Message,
                                        StackTrace = ex.InnerException.StackTrace,
                                        Created = DateTime.UtcNow
                                    });
                                }
                            }

                            Thread.Sleep(100);
                        }
                        db.SaveChanges();

                        Thread.Sleep(500);
                    }


                    filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.PaymentStatusUpdated);
                    var list2 = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if (list2 != null && list2.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["subscription_code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    int paymentId = Convert.ToInt32(jObj["id"].ToString());
                                    var payment = db.MoipPayment.SingleOrDefault(p => p.Id == paymentId);
                                    if (payment != null)
                                    {
                                        payment.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                        payment.Modified = DateTime.Now;
                                        payment.IdMoipInvoice = Convert.ToInt32(jObj["invoice_id"].ToString());
                                        payment.MoipId = Convert.ToInt32(jObj["moip_id"].ToString());
                                        payment.Status = jObj["status"]["description"].ToString();
                                        payment.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());

                                        if (jObj["payment_method"] != null)
                                        {
                                            payment.PaymentMethod = jObj["payment_method"]["code"].ToString();
                                            payment.Description = jObj["payment_method"]["description"].ToString();
                                            if (jObj["payment_method"]["credit_card"] != null)
                                            {
                                                payment.Brand = jObj["payment_method"]["credit_card"]["brand"].ToString();
                                                payment.HolderName = jObj["payment_method"]["credit_card"]["holder_name"].ToString();
                                                payment.FirstSixDigits = jObj["payment_method"]["credit_card"]["first_six_digits"].ToString();
                                                payment.LastFourDigits = jObj["payment_method"]["credit_card"]["last_four_digits"].ToString();
                                                payment.Vault = jObj["payment_method"]["credit_card"]["vault"].ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        payment = new MoipPayment();
                                        payment.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                                        payment.Created = DateTime.Now;
                                        payment.Modified = DateTime.Now;
                                        payment.Id = paymentId;
                                        payment.IdMoipInvoice = Convert.ToInt32(jObj["invoice_id"].ToString());
                                        payment.MoipId = Convert.ToInt32(jObj["moip_id"].ToString());
                                        payment.Status = jObj["status"]["description"].ToString();
                                        payment.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());
                                        payment.IdMoipSignature = signature.Id;

                                        if (jObj["payment_method"] != null)
                                        {
                                            payment.PaymentMethod = jObj["payment_method"]["code"].ToString();
                                            payment.Description = jObj["payment_method"]["description"].ToString();
                                            if (jObj["payment_method"]["credit_card"] != null)
                                            {
                                                payment.Brand = jObj["payment_method"]["credit_card"]["brand"].ToString();
                                                payment.HolderName = jObj["payment_method"]["credit_card"]["holder_name"].ToString();
                                                payment.FirstSixDigits = jObj["payment_method"]["credit_card"]["first_six_digits"].ToString();
                                                payment.LastFourDigits = jObj["payment_method"]["credit_card"]["last_four_digits"].ToString();
                                                payment.Vault = jObj["payment_method"]["credit_card"]["vault"].ToString();
                                            }
                                        }

                                        db.MoipPayment.Add(payment);
                                    }
                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    item.Status = (int)MoipNotificationStatus.Ignored;
                                    item.Modified = DateTime.UtcNow;
                                }
                            }
                            catch (Exception ex)
                            {
                                item.Status = (int)MoipNotificationStatus.Error;
                                item.Modified = DateTime.UtcNow;

                                db.LogError.Add(new LogError()
                                {
                                    Reference = "ProcessPayments",
                                    Complement = "id:" + item.Id,
                                    Message = ex.Message,
                                    StackTrace = ex.StackTrace,
                                    Created = DateTime.UtcNow
                                });

                                if (ex.InnerException != null)
                                {
                                    db.LogError.Add(new LogError()
                                    {
                                        Reference = "ProcessPayments - INNER",
                                        Complement = "id:" + item.Id,
                                        Message = ex.InnerException.Message,
                                        StackTrace = ex.InnerException.StackTrace,
                                        Created = DateTime.UtcNow
                                    });
                                }
                            }

                            Thread.Sleep(100);
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("MoipNotificationRepository.ProcessPayments", ex.Message, "", ex.StackTrace);
                if(ex.InnerException != null)
                    logError.Create("MoipNotificationRepository.ProcessPayments - INNER", ex.InnerException.Message, "", ex.InnerException.StackTrace);
            }
        }
    }
}
