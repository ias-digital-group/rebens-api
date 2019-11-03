using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "customer")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository repo;
        private IWirecardPaymentRepository paymentRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="orderRepository"></param>
        public OrderController(IOrderRepository orderRepository, IWirecardPaymentRepository wirecardPaymentRepository)
        {
            this.repo = orderRepository;
            this.paymentRepo = wirecardPaymentRepository;
        }

        /// <summary>
        /// Retorna um pedido
        /// </summary>
        /// <param name="id">Id do pedido desejada</param>
        /// <returns>Order</returns>
        /// <response code="200">Retorna o pedido, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<OrderModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var order = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (order == null || order.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<OrderModel>() { Data = new OrderModel(order) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um Pedido
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do pedido criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] OrderModel order)
        {
            int idCustomer = 0;
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });

                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var o = order.GetEntity();
            o.IdCustomer = idCustomer;
            o.IdOperation = idOperation;
            if (repo.Create(o, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Pedido criado com sucesso!", Id = o.Id, Extra = o.DispId });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma lista com os números da sorte
        /// </summary>
        /// <returns>Lista com os números</returns>
        /// <response code="200">Retorna a lista com os números, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(JsonDataModel<List<OrderModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 20, [FromQuery]string word = null, [FromQuery]string sort = "" )
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var list = repo.ListByCustomer(idCustomer, page, pageItems, word, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<OrderModel>>()
                {
                    Data = new List<OrderModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new OrderModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um Pedido com as informações do Wirecard
        /// </summary>
        /// <param name="info">info</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SaveWirecardInfo")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveWirecardInfo([FromBody] OrderWirecardInfo info)
        {
            if (repo.SaveWirecardInfo(info.Id, info.WirecardId, info.Status, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Pedido atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um Pagamento
        /// </summary>
        /// <param name="payment">pagamento</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("CreatePayment")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CreatePayment([FromBody] WirecardPaymentModel payment)
        {
            var wp = payment.GetEntity();

            if (paymentRepo.Create(wp, out string error))
            {
                repo.SendOrderConfirmationEmail(wp.IdOrder, out error);
                return Ok(value: new JsonCreateResultModel() { Status = "ok", Message = "Pagamento criado com sucesso!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }


        /// <summary>
        /// Atualiza um Pagamento
        /// </summary>
        /// <param name="payment">pagamento</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for ataulizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("UpdatePayment")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UpdatePayment([FromBody] WirecardPaymentModel payment)
        {
            var wp = payment.GetEntity();

            if (paymentRepo.Update(wp, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Pagamento atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}