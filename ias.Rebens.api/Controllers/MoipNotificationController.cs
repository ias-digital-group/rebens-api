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
        public IMoipNotificationRepository moipNotificationRepo;
        public ILogErrorRepository logErrorRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="moipNotificationRepository"></param>
        /// <param name="logErrorRepository"></param>
        public MoipNotificationController(IMoipNotificationRepository moipNotificationRepository, ILogErrorRepository logErrorRepository)
        {
            this.moipNotificationRepo = moipNotificationRepository;
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
            if(authorization == Constant.MoipNotificationAuthorization)
            {
                if (notification != null && notification.Resource != null)
                {
                    try
                    {
                        logErrorRepo.Create(new LogError() { Reference = "Controller.MoipNotification.Post", Complement = "event:" + notification.Event, Message = notification.Resource.ToString(), Created = DateTime.Now, StackTrace = "Env:" + notification.Env });
                    }
                    catch { }

                    var moipNotification = new MoipNotification()
                    {
                        Created = DateTime.UtcNow,
                        Envoirement = notification.Env,
                        Event = notification.Event,
                        Modified = DateTime.UtcNow,
                        Status = (int)Enums.MoipNotificationStatus.New,
                        Resources = notification.Resource.ToString(),
                        IdOperation = 1
                    };

                    moipNotificationRepo.Create(moipNotification, out string error);
                }
            }
            return Ok();
        }
    }
}