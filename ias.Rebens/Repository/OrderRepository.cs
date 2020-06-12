using ias.Rebens.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ias.Rebens
{
    public class OrderRepository : IOrderRepository
    {
        private string _connectionString;
        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Create(Order order, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string dispId = "";
                    while (true)
                    {
                        dispId = Helper.SecurityHelper.GenerateCode(12);
                        if (!db.Order.Any(o => o.DispId == dispId))
                            break;
                    }

                    order.DispId = dispId;
                    order.Modified = order.Created = DateTime.UtcNow;
                    db.Order.Add(order);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o pedido. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Order> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Order> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Order.Include("OrderItems").Where(o => o.IdCustomer == idCustomer &&
                                    (string.IsNullOrEmpty(word) || o.DispId.Contains(word) 
                                    || (!string.IsNullOrEmpty(o.WirecardId) && o.WirecardId.Contains(word)) 
                                    || o.PaymentType.Contains(word) || o.Status.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "disId asc":
                            tmpList = tmpList.OrderBy(f => f.DispId);
                            break;
                        case "disId desc":
                            tmpList = tmpList.OrderByDescending(f => f.DispId);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "total asc":
                            tmpList = tmpList.OrderBy(f => f.Total);
                            break;
                        case "total desc":
                            tmpList = tmpList.OrderByDescending(f => f.Total);
                            break;
                        case "paymentType asc":
                            tmpList = tmpList.OrderBy(f => f.PaymentType);
                            break;
                        case "paymentType desc":
                            tmpList = tmpList.OrderByDescending(f => f.PaymentType);
                            break;
                        case "date asc":
                            tmpList = tmpList.OrderBy(f => f.Created);
                            break;
                        default:
                            tmpList = tmpList.OrderByDescending(f => f.Created);
                            break;
                    }

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();

                    ret = new ResultPage<Order>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.ListByCustomer", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os pedidos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Order> ListToUpdate(int count, out string error)
        {
            List<Order> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.Now.AddHours(-2);
                    ret = db.Order.Where(o => (o.Status == "CREATED" || o.Status == "WAITING") 
                                && (!o.WirecardDate.HasValue || o.WirecardDate.Value < dt))
                            .OrderBy(o => o.WirecardDate).Take(count).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.ListToUpdate", ex.Message, null, ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os pedidos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Order Read(int id, out string error)
        {
            Order ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Order.Include("OrderItems").Include("WirecardPayments").SingleOrDefault(o => o.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o pedido. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SaveWirecardInfo(int id, string wirecardId, string status, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Order.SingleOrDefault(c => c.Id == id);
                    if (update != null)
                    {
                        update.WirecardId = wirecardId;
                        update.Status = status;
                        update.WirecardDate = DateTime.UtcNow;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Pedido não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.SaveWirecardInfo", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o pedido. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public Order ReadByDispId(string id, out string error)
        {
            Order ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Order.Include("OrderItems").SingleOrDefault(o => o.DispId == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.ReadByDispId", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o pedido. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Order ReadByWirecardId(string id, out string error)
        {
            Order ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Order.Include("OrderItems").SingleOrDefault(o => o.WirecardId == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.ReadByWirecardId", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o pedido. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SendOrderConfirmationEmail(int idOrder, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var order = db.Order.Include("Customer").Include("OrderItems").SingleOrDefault(o => o.Id == idOrder);
                   if(order != null)
                    {
                        var operation = db.Operation.Single(o => o.Id == order.IdOperation);
                        var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                        string fromEmail = "";
                        if (configuration != null)
                        {
                            var jObj = JObject.Parse(configuration.Html);
                            var list = jObj["fields"].Children();
                            foreach (var item in list)
                            {
                                if (item["name"].ToString() == "contact-email")
                                {
                                    fromEmail = item["data"].ToString();
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                        string html = "###BODY###";
                        var staticText = db.StaticText.SingleOrDefault(s=> s.IdOperation == operation.Id && s.IdStaticTextType == (int)Enums.StaticTextType.Email);
                        if (staticText != null) html = staticText.Html;
                        if (Helper.EmailHelper.SendOrderConfirmationEmail(order, operation, fromEmail, html, out error))
                        {
                            error = null;
                            ret = true;
                        }
                    }
                    else
                        error = "Pedido não encontrado";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar enviar a confirmação do pedido para o cliente. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool HasOrderToProcess()
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.UtcNow.AddMinutes(-15);
                    ret = db.Order.Any(o => (o.Status == "CREATED" || o.Status == "WAITING") 
                                && o.WirecardPayments.Any(p => p.Status != "CREATED" && p.Status != "WAITING" && p.Status != "IN_ANALYSIS")
                                && o.Modified < dt);
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WirecardPaymentRepository.HasPaymentToProcess", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public void ProcessOrder()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.UtcNow.AddMinutes(-15);
                    var list = db.Order.Include("OrderItems")
                        .Where(o => (o.Status == "CREATED" || o.Status == "WAITING")
                                && o.WirecardPayments.Any(p => p.Status != "CREATED" && p.Status != "WAITING" && p.Status != "IN_ANALYSIS")
                                && o.Modified < dt)
                        .OrderBy(o => o.Modified).Take(10);
                    var wcHelper = new Integration.WirecardHelper();
                    var orderHelper = new Helper.OrderHelper();
                    var constant = new Constant();

                    foreach (var item in list)
                    {
                        if (wcHelper.CheckOrderStatus(item))
                        {
                            if (item.Status == "PAID")
                            {
                                var customer = db.Customer.Single(c => c.Id == item.IdCustomer);
                                var operation = db.Operation.Single(o => o.Id == item.IdOperation);
                                string fromEmail = "";
                                var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == item.IdOperation && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                                if (configuration != null)
                                {
                                    var jObj = JObject.Parse(configuration.Html);
                                    var fields = jObj["fields"].Children();
                                    foreach (var field in fields)
                                    {
                                        if (field["name"].ToString() == "contact-email")
                                        {
                                            fromEmail = field["data"].ToString();
                                            break;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";

                                string fileName = "";
                                if (orderHelper.GeneratePdf(item.DispId))
                                    fileName = $"{constant.URL}files/{item.DispId}-order.pdf";

                                string html = "###BODY###";
                                var staticText = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)Enums.StaticTextType.Email);
                                if (staticText != null) html = staticText.Html;
                                if (Helper.EmailHelper.SendProductVoucher(customer, item, operation, fromEmail, fileName, html, out string error))
                                {
                                    var logError = new LogErrorRepository(this._connectionString);
                                    logError.Create("WirecardPaymentRepository.ProcessOrder SendMail", error, "", "");
                                }
                                Thread.Sleep(100);
                            }
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("OrderRepository.ProcessOrder", ex.Message, "", ex.StackTrace);
            }
        }

        public Order ReadByItem(string code, string voucher, out string error)
        {
            Order ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Order.SingleOrDefault(o => o.DispId == code);
                    var item = db.OrderItem.SingleOrDefault(i => i.IdOrder == ret.Id && i.Voucher == voucher);
                    if (item != null)
                        ret.OrderItems.Add(item);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.ReadByItem", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o pedido. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<ProductValidateItem> ListItemsByOperation(int page, int pageItems, string word, out string error, int? idOperation = null)
        {
            ResultPage<ProductValidateItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OrderItem.Where(o => o.Order.Status == "PAID" &&
                                        (!idOperation.HasValue || o.Order.IdOperation == idOperation) &&
                                        (string.IsNullOrEmpty(word) || o.Voucher.Contains(word)));

                    var total = tmpList.Count();
                    var list = tmpList.OrderBy(o => o.UsedDate).ThenByDescending(o => o.Created).Skip(page * pageItems).Take(pageItems)
                                    .Select(i => new ProductValidateItem() {
                                        Id = i.Id,
                                        ItemName = i.Name,
                                        Created = i.Created,
                                        Used = i.UsedDate,
                                        IdOrder = i.Order.Id,
                                        IdCustomer = i.Order.IdCustomer,
                                        IdOperation = i.Order.IdOperation,
                                        CustomerCpf = i.Order.Customer.Cpf,
                                        CustomerName = i.Order.Customer.Name,
                                        OperationName = i.Order.Operation.Title, 
                                        Voucher = i.Voucher
                                    }).ToList();

                    foreach(var item in list)
                    {
                        if (item.Used.HasValue)
                        {
                            var log = db.LogAction.FirstOrDefault(a => a.Action == (int)Enums.LogAction.voucherValidate
                                                && a.Item == (int)Enums.LogItem.OrderItem
                                                && a.IdItem == item.Id);
                            if(log != null)
                            {
                                item.IdAdminUser = log.IdAdminUser;
                                var admin = db.AdminUser.SingleOrDefault(a => a.Id == log.IdAdminUser);
                                if (admin != null)
                                    item.AdminUserName = admin.Name;
                            }
                        }
                    }

                    ret = new ResultPage<ProductValidateItem>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OrderRepository.ListItemsByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os pedidos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SetItemUsed(int id, int idAdminUser, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OrderItem.SingleOrDefault(i => i.Id == id);
                    if(update != null)
                    {
                        update.UsedDate = DateTime.UtcNow;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.voucherValidate,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = id,
                            Item = (int)Enums.LogItem.OrderItem
                        });

                        db.SaveChanges();
                        ret = true;
                        error = null;
                    } 
                    else
                        error = "Produto não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WirecardPaymentRepository.SetItemUsed", ex.Message, "", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar validar o item. (erro: {idLog})";
            }
            return ret;
        }
    }
}
