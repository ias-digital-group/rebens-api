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
    [Route("api/[controller]"), Authorize("Bearer", Roles = "customer,master,administrator,administratorRebens,ticketChecker")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository repo;
        private IWirecardPaymentRepository paymentRepo;
        private ICustomerRepository customerRepo;
        private IAddressRepository addrRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="orderRepository"></param>
        public OrderController(IOrderRepository orderRepository, IWirecardPaymentRepository wirecardPaymentRepository, 
            ICustomerRepository customerRepository, IAddressRepository addressRepository)
        {
            this.repo = orderRepository;
            this.paymentRepo = wirecardPaymentRepository;
            this.customerRepo = customerRepository;
            this.addrRepo = addressRepository;
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
            if(order == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto nulo!" });

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

            var addr = order.Customer.Address.GetEntity();
            if (addr.Id == 0)
                addrRepo.Create(addr, out _);
            else
                addrRepo.Update(addr, out _);
            var custo = order.Customer.GetEntity();
            custo.IdAddress = addr.Id;
            customerRepo.Update(custo, out _);

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
        [ProducesResponseType(typeof(ResultPageModel<OrderModel>), 200)]
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

                var ret = new ResultPageModel<OrderModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
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
            if (info != null)
            {
                if (repo.SaveWirecardInfo(info.Id, info.WirecardId, info.Status, out string error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Pedido atualizado com sucesso!" });

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto vazio" });
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
            if (payment != null)
            {
                var wp = payment.GetEntity();

                if (paymentRepo.Create(wp, out string error))
                {
                    repo.SendOrderConfirmationEmail(wp.IdOrder, out error);
                    return Ok(value: new JsonCreateResultModel() { Status = "ok", Message = "Pagamento criado com sucesso!" });
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto vazio" });
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
            if (payment != null)
            {
                var wp = payment.GetEntity();

                if (paymentRepo.Update(wp, out string error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Pagamento atualizado com sucesso!" });

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto vazio" });
        }

        /// <summary>
        /// Retorna uma lista com produtos para validação
        /// </summary>
        /// <returns>Lista com os números</returns>
        /// <response code="200">Retorna a lista com os números, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Validation")]
        [ProducesResponseType(typeof(ResultPageModel<ProductValidationModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListForValidation([FromQuery] int page = 0, [FromQuery] int pageItems = 20, [FromQuery] string word = null, [FromQuery] int? idOperation = null)
        {
            var principal = HttpContext.User;
            if (principal.IsInRole(Enums.Roles.administrator.ToString()) || principal.IsInRole(Enums.Roles.ticketChecker.ToString()))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmp))
                        idOperation = tmp;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var list = repo.ListItemsByOperation(page, pageItems, word, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ProductValidationModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ProductValidationModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new ProductValidationModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Valida um Produto
        /// </summary>
        /// <param name="id">id do produto</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for ataulizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("Validate/{id}")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Validate(int id)
        {
            int idAdminUser = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (userId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
                if (!int.TryParse(userId.Value, out idAdminUser))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            if(repo.SetItemUsed(id, idAdminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Item validado com sucesso!" });
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Salva uma Assinatura
        /// </summary>
        /// <param name="payment">pagamento</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SaveSubscription")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveSubscription([FromBody] WirecardPaymentModel payment)
        {
            if (payment != null)
            {
                var wp = payment.GetEntity();

                if (paymentRepo.Create(wp, out string error))
                {
                    repo.SendOrderConfirmationEmail(wp.IdOrder, out error);
                    return Ok(value: new JsonCreateResultModel() { Status = "ok", Message = "Pagamento criado com sucesso!" });
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto vazio" });
        }
    }
}