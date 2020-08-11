using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,administratorRebens,")]
    [ApiController]
    public class WirecardController : BaseApiController
    {
        private readonly IMoipRepository repo;
        private IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="moipRepository"></param>
        /// <param name="hostingEnvironment"></param>
        public WirecardController(IMoipRepository moipRepository, IHostingEnvironment hostingEnvironment)
        {
            this.repo = moipRepository;
            this._hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Retorna uma lista com as assinaturas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageItems"></param>
        /// <param name="word"></param>
        /// <param name="idOperation"></param>
        /// <param name="plan"></param>
        /// <param name="status"></param>
        /// <returns>Lista com as assinaturas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<MoipSignatureModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery] int page = 0, [FromQuery] int pageItems = 20, [FromQuery] string searchWord = null, [FromQuery]int? idOperation = null, [FromQuery]string plan = null, [FromQuery]string status = null)
        {
            if (CheckRole("administrator"))
            {
                idOperation = GetOperationId(out string errorId);
                if (!string.IsNullOrEmpty(errorId))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrado!" });
            }

            ResultPage<Entity.MoipSignatureItem> list = repo.ListSubscriptions(page, pageItems, searchWord, out string error, plan, status, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<MoipSignatureModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<MoipSignatureModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new MoipSignatureModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Gera um arquivo excel com as assinaturas
        /// </summary>
        /// <param name="word"></param>
        /// <param name="idOperation"></param>
        /// <param name="plan"></param>
        /// <param name="status"></param>
        /// <returns>caminho do arquivo gerado</returns>
        /// <response code="200">Retorna a url do arquivo gerado, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("GenerateExcel")]
        [ProducesResponseType(typeof(ResultPageModel<MoipSignatureModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GenerateExcel([FromQuery]string searchWord = null, [FromQuery]int? idOperation = null, [FromQuery] string plan = null, [FromQuery] string status = null)
        {
            if (CheckRole("administrator"))
            {
                idOperation = GetOperationId(out string errorId);
                if (!string.IsNullOrEmpty(errorId))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrado!" });
            }

            ResultPage<Entity.MoipSignatureItem> list = repo.ListSubscriptions(0, 9999999, searchWord, out string error, plan, status, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();
                try
                {
                    string fileName = Guid.NewGuid().ToString("n", Constant.FormatProvider) + ".xlsx";
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        IXLWorksheet worksheet = workbook.Worksheets.Add("Authors");
                        worksheet.Cell(1, 1).Value = "Código";
                        worksheet.Cell(1, 2).Value = "Nome";
                        worksheet.Cell(1, 3).Value = "Plano";
                        worksheet.Cell(1, 4).Value = "Valor";
                        worksheet.Cell(1, 5).Value = "Próxima cobrança";
                        worksheet.Cell(1, 6).Value = "Status";
                        int row = 2;
                        foreach (var item in list)
                        {
                            var newitem = new MoipSignatureModel(item);
                            worksheet.Cell(row, 1).Value = newitem.Code;
                            worksheet.Cell(row, 1).DataType = XLDataType.Number;
                            worksheet.Cell(row, 2).Value = newitem.Customer.Name;
                            worksheet.Cell(row, 3).Value = newitem.PlanName;
                            worksheet.Cell(row, 4).Value = newitem.AmountString;
                            worksheet.Cell(row, 5).Value = newitem.NextInvoiceDateString;
                            worksheet.Cell(row, 6).Value = newitem.Status;

                            row++;
                        }

                        string newPath = Path.Combine(_hostingEnvironment.WebRootPath, "files", "excel");
                        if (!Directory.Exists(newPath))
                            Directory.CreateDirectory(newPath);

                        workbook.SaveAs(Path.Combine(newPath, fileName));
                        workbook.Dispose();
                    }


                    var constant = new Constant();
                    return Ok(new FileUploadResultModel() { FileName = fileName, Url = $"{constant.URL}files/excel/{fileName}" });
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    msg += " | " + ex.StackTrace;
                    if (ex.InnerException != null)
                    {
                        msg += " | INNER - " + ex.InnerException.Message;
                        msg += " | " + ex.InnerException.StackTrace;
                    }
                    return StatusCode(400, new JsonModel() { Status = "error", Message = msg });
                }
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
