using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ias.Rebens.api.Models;
using System.Globalization;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Controller que recebe as notificações do moip
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MoipNotificationController : ControllerBase
    {
        public IMoipRepository moipRepo;
        public ILogErrorRepository logErrorRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="moipRepository"></param>
        /// <param name="logErrorRepository"></param>
        public MoipNotificationController(IMoipRepository moipRepository, ILogErrorRepository logErrorRepository)
        {
            this.moipRepo = moipRepository;
            this.logErrorRepo = logErrorRepository;
        }

        /// <summary>
        /// Webhook GET
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPost("Test")]
        public IActionResult Test([FromHeader]string authorization, [FromBody]MoipNotificationModel notification)
        {
            logErrorRepo.Create(new LogError() {
                Reference = "Controller.MoipNotification.Test-0",
                Complement = "authorization:" + authorization,
                Message = notification.Resource.ToString(),
                Created = DateTime.Now,
                StackTrace = $"event:{notification.Event}, env:{notification.Env}, date:{notification.Date}" });

            try
            {
                //var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<MoipNotificationModel>(notification.ToString());
                //logErrorRepo.Create(new LogError() { Reference = "Controller.MoipNotification.Test-1", Complement = "event:" + temp.Event, Message = temp.Resource.ToString(), Created = DateTime.Now, StackTrace = "Env:" + temp.Env });

                var culture = new CultureInfo("pt-BR");
                var date = Convert.ToDateTime(notification.Date, culture);

                logErrorRepo.Create(new LogError() { Reference = "Controller.MoipNotification.Test-1", Complement = "Date Converted", Message = date.ToString("dd/MM/yyyy HH:mm:ss"), Created = DateTime.Now, StackTrace = "" });

            }
            catch (Exception ex)
            {
                logErrorRepo.Create(new LogError() { Reference = "Controller.MoipNotification.Test", Complement = "ERROR", Message = ex.Message, Created = DateTime.Now, StackTrace = ex.StackTrace });
            }

            //if (authorization == "c7c609fcdb7ef70ac57afdc782574ee3")
            //{
            //    ThreatNotification(notification);
            //}
            return Ok();
        }

        /// <summary>
        /// Webhook POST
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromHeader]string authorization, [FromBody]MoipNotificationModel notification)
        {
            if(authorization == "c7c609fcdb7ef70ac57afdc782574ee3")
            {
                ThreatNotification(notification);
            }
            return Ok();
        }


        private void ThreatNotification(MoipNotificationModel notification)
        {
            if (notification != null && notification.Resource != null)
            {
                try
                {
                    logErrorRepo.Create(new LogError() { Reference = "Controller.MoipNotification.Post", Complement = "event:" + notification.Event, Message = notification.Resource.ToString(), Created = DateTime.Now, StackTrace = "Env:" + notification.Env });
                }
                catch { }

                var jObj = JObject.Parse(notification.Resource.ToString());

                if (notification.Event.ToLower().StartsWith("subscription.")
                && (notification.Event.ToLower().EndsWith("created") || notification.Event.ToLower().EndsWith("updated")))
                {
                    var signature = new MoipSignature();

                    signature.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                    signature.Code = jObj["code"].ToString();
                    signature.Created = DateTime.Now;
                    signature.CreationDate = new DateTime(Convert.ToInt32(jObj["creation_date"]["year"].ToString()), Convert.ToInt32(jObj["creation_date"]["month"].ToString()), Convert.ToInt32(jObj["creation_date"]["day"].ToString()));
                    signature.ExpirationDate = new DateTime(Convert.ToInt32(jObj["expiration_date"]["year"].ToString()), Convert.ToInt32(jObj["expiration_date"]["month"].ToString()), Convert.ToInt32(jObj["expiration_date"]["day"].ToString()));
                    signature.Modified = DateTime.Now;
                    signature.NextInvoiceDate = new DateTime(Convert.ToInt32(jObj["next_invoice_date"]["year"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["month"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["day"].ToString()));
                    signature.PaymentMethod = jObj["payment_method"].ToString();
                    signature.PlanCode = jObj["plan"]["code"].ToString();
                    signature.Status = jObj["status"].ToString();
                    
                    signature.IdCustomer = Convert.ToInt32(jObj["customer"]["code"].ToString());
                    

                    moipRepo.SaveSignature(signature);
                }
                else if (notification.Event.ToLower().StartsWith("invoice.")
                    && (notification.Event.ToLower().EndsWith("created") || notification.Event.ToLower().EndsWith("status_updated")))
                {
                    var invoice = new MoipInvoice();
                    invoice.Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100;
                    invoice.Created = DateTime.Now;
                    invoice.Modified = DateTime.Now;
                    invoice.Status = jObj["status"]["description"].ToString();
                    invoice.IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString());
                    invoice.Id = Convert.ToInt32(jObj["Id"].ToString());
                    invoice.Occurrence = Convert.ToInt32(jObj["occurrence"].ToString());

                    moipRepo.SaveInvoice(invoice, jObj["subscription_code"].ToString());

                }
                else if (notification.Event.ToLower().StartsWith("payment.")
                    && (notification.Event.ToLower().EndsWith("created") || notification.Event.ToLower().EndsWith("status_updated")))
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

                    moipRepo.SavePayment(payment, jObj["subscription_code"].ToString());
                }
            }
        }
    }

}