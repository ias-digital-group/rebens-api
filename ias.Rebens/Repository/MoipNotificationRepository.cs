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
                    string[] filter = {
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionCreated),
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionUpdated),
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionUpdated),
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionUpdated)
                        };

                    ret = db.MoipNotification.Any(n => n.Status == (int)MoipNotificationStatus.New && filter.Contains(n.Event));
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
                    string[] filter = {
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.InvoiceCreated),
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.InvoiceStatusUpdated)
                    };

                    ret = db.MoipNotification.Any(n => n.Status == (int)MoipNotificationStatus.New && filter.Contains(n.Event));
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
                    string[] filter = {
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.PaymentCreated),
                        EnumHelper.GetEnumDescription(MoipNotificationEvent.PaymentStatusUpdated)
                    };

                    ret = db.MoipNotification.Any(n => n.Status == (int)MoipNotificationStatus.New && filter.Contains(n.Event));
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
                                int customerId = Convert.ToInt32(jObj["customer"]["code"].ToString());
                                var customer = db.Customer.SingleOrDefault(c => c.Id == customerId);
                                if (customer != null)
                                {
                                    string code = jObj["code"].ToString();
                                    MoipSignature signature = new MoipSignature()
                                    {
                                        IdCustomer = customer.Id,
                                        Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100,
                                        Code = code,
                                        Created = DateTime.Now,
                                        CreationDate = new DateTime(Convert.ToInt32(jObj["creation_date"]["year"].ToString()), Convert.ToInt32(jObj["creation_date"]["month"].ToString()), Convert.ToInt32(jObj["creation_date"]["day"].ToString())),
                                        Modified = DateTime.Now,
                                        NextInvoiceDate = new DateTime(Convert.ToInt32(jObj["next_invoice_date"]["year"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["month"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["day"].ToString())),
                                        PaymentMethod = jObj["payment_method"].ToString(),
                                        PlanCode = jObj["plan"]["code"].ToString(),
                                        Status = jObj["status"].ToString(),
                                        IdOperation = customer.IdOperation
                                    };

                                    if(jObj["expiration_date"] != null)
                                        signature.ExpirationDate = new DateTime(Convert.ToInt32(jObj["expiration_date"]["year"].ToString()), Convert.ToInt32(jObj["expiration_date"]["month"].ToString()), Convert.ToInt32(jObj["expiration_date"]["day"].ToString()));
                                    else
                                        signature.ExpirationDate = signature.NextInvoiceDate;

                                    db.MoipSignature.Add(signature);

                                    if(signature.Status.ToUpper() == "ACTIVE" || signature.Status.ToUpper() == "TRIAL")
                                    {
                                        var operation = db.Operation.Single(o => o.Id == customer.IdOperation);
                                        var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)StaticTextType.OperationConfiguration);
                                        string fromEmail = "";
                                        if (configuration != null)
                                        {
                                            var jObj2 = JObject.Parse(configuration.Html);
                                            var listFields = jObj2["fields"].Children();
                                            foreach (var item2 in listFields)
                                            {
                                                if (item2["name"].ToString() == "contact-email")
                                                {
                                                    fromEmail = item2["data"].ToString();
                                                    break;
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                                        string html = "###BODY###";
                                        var staticText = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)StaticTextType.Email);
                                        if (staticText != null) html = staticText.Html;
                                        Helper.EmailHelper.SendSignatureConfirmationEmail(customer, operation, fromEmail, html, out _);
                                    }

                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;

                                    //if(signature.Status == "ACTIVE")
                                    //{
                                        //if (!db.DrawItem.Any(d => d.IdCustomer == customer.Id && d.IdDraw == 1 && d.Modified.Year == DateTime.Now.Year && d.Modified.Month == DateTime.Now.Month))
                                        //{
                                        //    var di = db.DrawItem.Where(d => d.IdDraw == 1 && !d.IdCustomer.HasValue).OrderBy(d => Guid.NewGuid()).FirstOrDefault();
                                        //    if (di != null)
                                        //    {
                                        //        di.IdCustomer = customer.Id;
                                        //        di.Modified = DateTime.Now;
                                        //        db.SaveChanges();
                                        //    }
                                        //}

                                        //if(!db.Coupon.Any(c => c.IdCustomer == customer.Id && c.Modified.Year == DateTime.Now.Year && c.Modified.Month == DateTime.Now.Month && c.Modified.Day == DateTime.Now.Day))
                                        //{
                                        //    var couponHelper = new Integration.CouponToolsHelper();
                                        //    var coupon = new Coupon()
                                        //    {
                                        //        Campaign = "Raspadinha Unicap",
                                        //        IdCustomer = customer.Id,
                                        //        IdCouponCampaign = 1,
                                        //        ValidationCode = Helper.SecurityHelper.GenerateCode(18),
                                        //        Locked = false,
                                        //        Status = (int)Enums.CouponStatus.pendent,
                                        //        VerifiedDate = DateTime.UtcNow,
                                        //        Created = DateTime.UtcNow,
                                        //        Modified = DateTime.UtcNow
                                        //    };

                                        //    if (couponHelper.CreateSingle(customer, coupon, out string error))
                                        //    {
                                        //        db.Coupon.Add(coupon);
                                        //        db.SaveChanges();
                                        //    }
                                        //}
                                    //}
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
                        foreach (var item in list2)
                        {
                            try { 
                                var jObj = JObject.Parse(item.Resources);
                                int customerId = Convert.ToInt32(jObj["customer"]["code"].ToString());
                                var customer = db.Customer.SingleOrDefault(c => c.Id == customerId);
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
                                        signature.IdOperation = customer.IdOperation;

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

                    filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionSuspended);
                    var list3 = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if (list3 != null && list3.Count() > 0)
                    {
                        foreach (var item in list3)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    signature.Status = "SUSPENDED";
                                    signature.Modified = DateTime.UtcNow;

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

                    filter = EnumHelper.GetEnumDescription(MoipNotificationEvent.SubscriptionCanceled);
                    var list4 = db.MoipNotification.Where(n => n.Status == (int)MoipNotificationStatus.New && n.Event == filter).OrderBy(n => n.Created);
                    if (list4 != null && list4.Count() > 0)
                    {
                        foreach (var item in list4)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    signature.Status = "CANCELED";
                                    signature.Modified = DateTime.UtcNow;

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
                                    var invoice = new MoipInvoice()
                                    {
                                        Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100,
                                        Created = DateTime.Now,
                                        Modified = DateTime.Now,
                                        Status = jObj["status"]["description"].ToString(),
                                        IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString()),
                                        Id = Convert.ToInt32(jObj["id"].ToString()),
                                        Occurrence = 0,
                                        IdMoipSignature = signature.Id
                                    };

                                    db.MoipInvoice.Add(invoice);

                                    item.Status = (int)MoipNotificationStatus.Processed;
                                    item.Modified = DateTime.UtcNow;
                                }
                                else
                                {
                                    if (item.Modified.AddDays(1) < DateTime.UtcNow)
                                    {
                                        item.Status = (int)MoipNotificationStatus.Ignored;
                                        item.Modified = DateTime.UtcNow;
                                    }
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
                        foreach (var item in list2)
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
                                    var payment = new MoipPayment()
                                    {
                                        Created = DateTime.Now,
                                        Modified = DateTime.Now,
                                        IdMoipInvoice = Convert.ToInt32(jObj["invoice_id"].ToString()),
                                        Status = jObj["status"]["description"].ToString(),
                                        IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString()),
                                        IdMoipSignature = signature.Id,
                                        Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100,
                                        MoipId = jObj["id"].ToString()

                                    };

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
                        foreach (var item in list2)
                        {
                            try
                            {
                                var jObj = JObject.Parse(item.Resources);
                                string code = jObj["subscription_code"].ToString();
                                var signature = db.MoipSignature.SingleOrDefault(s => s.Code == code);
                                if (signature != null)
                                {
                                    string paymentId = jObj["id"].ToString();
                                    var payment = db.MoipPayment.SingleOrDefault(p => p.MoipId == paymentId);
                                    if (payment != null)
                                    {
                                        payment.Modified = DateTime.Now;
                                        payment.Status = jObj["status"]["description"].ToString();
                                        payment.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());

                                        item.Status = (int)MoipNotificationStatus.Processed;
                                        item.Modified = DateTime.UtcNow;
                                    }
                                    else
                                    {
                                        item.Status = (int)MoipNotificationStatus.Ignored;
                                        item.Modified = DateTime.UtcNow;
                                    }
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
