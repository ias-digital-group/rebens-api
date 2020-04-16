using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    var tmpList = db.Order.Where(o => o.IdCustomer == idCustomer &&
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
                error = "Ocorreu um erro ao tentar criar ler o pedido. (erro:" + idLog + ")";
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
                    var order = db.Order.Include("Customer").SingleOrDefault(o => o.Id == idOrder);
                   if(order != null)
                    {
                        string body = $"<p>Olá {order.Customer.Name}, </p><br />";
                        body += $"<h2>Recebemos o seu pedido #{order.DispId}</h2><br /><br />";
                        body += $"<p>Estamos aguardando a confirmação do pagamento, e assim que for autorizado enviaremos um e-mail com todas as informações necessárias para você. </p>";

                        var staticText = db.StaticText.Where(t => t.IdOperation == order.IdOperation && t.IdStaticTextType == (int)Enums.StaticTextType.Email && t.Active)
                                            .OrderByDescending(t => t.Modified).FirstOrDefault();
                        string message = staticText.Html.Replace("###BODY###", body);
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
                        if (Helper.EmailHelper.SendDefaultEmail(order.Customer.Email, order.Customer.Name, order.IdOperation, $"UNICAMPI EDUCAÇÃO - Pedido #{order.DispId}", message, fromEmail, operation.Title, out error))
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
                    
                    foreach (var item in list)
                    {
                        if (wcHelper.CheckOrderStatus(item))
                        {
                            if (item.Status == "PAID")
                            {
                                var staticText = db.StaticText.Where(t => t.IdOperation == item.IdOperation && t.IdStaticTextType == (int)Enums.StaticTextType.Email && t.Active).OrderByDescending(t => t.Modified).FirstOrDefault();
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

                                if (Helper.EmailHelper.SendCourseVoucher(staticText, customer, item, operation, fromEmail, out string error))
                                {
                                    var logError = new LogErrorRepository(this._connectionString);
                                    logError.Create("WirecardPaymentRepository.ProcessOrder SendMail", error, "", "");
                                }
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
    }
}
