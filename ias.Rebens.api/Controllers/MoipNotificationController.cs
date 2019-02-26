using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ias.Rebens.api.Models;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MoipNotificationController : ControllerBase
    {
        public IMoipRepository moipRepo;

        public MoipNotificationController(IMoipRepository moipRepository)
        {
            this.moipRepo = moipRepository;
        }

        /// <summary>
        /// Webhook
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="Authorization"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromHeader]string Authorization, [FromBody]MoipNotificationModel notification)
        {
            if(Authorization == "c7c609fcdb7ef70ac57afdc782574ee3")
            {
                if (notification.Resource != null)
                {
                    var jObj = JObject.Parse(notification.Resource.ToString());

                    if (notification.Event.ToLower().StartsWith("subscription.")
                    && (notification.Event.ToLower().EndsWith("created") || notification.Event.ToLower().EndsWith("updated")))
                    {
                        var signature = new MoipSignature()
                        {
                            Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100,
                            Code = jObj["code"].ToString(),
                            Created = DateTime.Now,
                            CreationDate = new DateTime(Convert.ToInt32(jObj["creation_date"]["year"].ToString()), Convert.ToInt32(jObj["creation_date"]["month"].ToString()), Convert.ToInt32(jObj["creation_date"]["day"].ToString())),
                            ExpirationDate = new DateTime(Convert.ToInt32(jObj["expiration_date"]["year"].ToString()), Convert.ToInt32(jObj["expiration_date"]["month"].ToString()), Convert.ToInt32(jObj["expiration_date"]["day"].ToString())),
                            Modified = DateTime.Now,
                            NextInvoiceDate = new DateTime(Convert.ToInt32(jObj["next_invoice_date"]["year"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["month"].ToString()), Convert.ToInt32(jObj["next_invoice_date"]["day"].ToString())),
                            PaymentMethod = jObj["payment_method"].ToString(),
                            PlanCode = jObj["plan"]["code"].ToString(),
                            Status = jObj["status"].ToString(),
                            IdCustomer = Convert.ToInt32(jObj["customer"]["code"].ToString())
                        };

                        moipRepo.SaveSignature(signature);
                    }
                    else if (notification.Event.ToLower().StartsWith("invoice.")
                        && (notification.Event.ToLower().EndsWith("created") || notification.Event.ToLower().EndsWith("status_updated")))
                    {
                        var invoice = new MoipInvoice()
                        {
                            Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            Status = jObj["status"]["description"].ToString(),
                            IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString()),
                            Id = Convert.ToInt32(jObj["Id"].ToString()),
                            Occurrence = Convert.ToInt32(jObj["occurrence"].ToString())
                        };

                        moipRepo.SaveInvoice(invoice, jObj["subscription_code"].ToString());

                    }
                    else if (notification.Event.ToLower().StartsWith("payment.")
                        && (notification.Event.ToLower().EndsWith("created") || notification.Event.ToLower().EndsWith("status_updated")))
                    {
                        var payment = new MoipPayment()
                        {
                            Amount = Convert.ToDecimal(jObj["amount"].ToString()) / 100,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            Id = Convert.ToInt32(jObj["id"].ToString()),
                            IdMoipInvoice = Convert.ToInt32(jObj["invoice_id"].ToString()),
                            MoipId = Convert.ToInt32(jObj["moip_id"].ToString()),
                            Status = jObj["status"]["description"].ToString(),
                            IdStatus = Convert.ToInt32(jObj["status"]["code"].ToString())
                        };
                        if (jObj["payment_method"] != null)
                        {
                            payment.PaymentMethod = Convert.ToInt32(jObj["payment_method"]["code"].ToString());
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
           
            return Ok();
        }
    }

}