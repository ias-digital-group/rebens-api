using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ias.Rebens.Helper
{
    public class ExcelHelper
    {
        //https://dzone.com/articles/import-and-export-excel-file-in-asp-net-core-31-ra
        public bool UnicsulReport(Models.UnicsulReport report, string filePath, ILogErrorRepository logRepo)
        {
            bool ret = false;
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                try
                {
                    workbook = new XSSFWorkbook();
                }
                catch(Exception ex)
                {
                    logRepo.Create("ExcelHelper.UnicsulReport", ex.Message, "create workbook", ex.StackTrace);
                    return false;
                }

                ISheet summarySheet = workbook.CreateSheet("Resumo Clientes");

                IRow row = summarySheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("");
                row.CreateCell(1).SetCellValue("");
                row.CreateCell(2).SetCellValue("");
                row.CreateCell(3).SetCellValue("");
                row.CreateCell(4).SetCellValue("");

                row = summarySheet.CreateRow(1);
                row.CreateCell(0).SetCellValue("");
                row.CreateCell(1).SetCellValue("Status");
                row.CreateCell(2).SetCellValue("Total");
                row.CreateCell(3).SetCellValue("Últimos 7 dias");
                row.CreateCell(4).SetCellValue("");

                row = summarySheet.CreateRow(2);
                row.CreateCell(0).SetCellValue("");
                row.CreateCell(1).SetCellValue("Completo");
                row.CreateCell(2).SetCellValue(report.Summary.Complete);
                row.CreateCell(3).SetCellValue(report.Summary.CompleteWeek);
                row.CreateCell(4).SetCellValue("");

                row = summarySheet.CreateRow(3);
                row.CreateCell(0).SetCellValue("");
                row.CreateCell(1).SetCellValue("Validação");
                row.CreateCell(2).SetCellValue(report.Summary.Validate);
                row.CreateCell(3).SetCellValue(report.Summary.ValidateWeek);
                row.CreateCell(4).SetCellValue("");

                row = summarySheet.CreateRow(4);
                row.CreateCell(0).SetCellValue("");
                row.CreateCell(1).SetCellValue("Incomplete");
                row.CreateCell(2).SetCellValue(report.Summary.Incomplete);
                row.CreateCell(3).SetCellValue(report.Summary.IncompleteWeek);
                row.CreateCell(4).SetCellValue("");

                row = summarySheet.CreateRow(5);
                row.CreateCell(0).SetCellValue("");
                row.CreateCell(1).SetCellValue("Pré-Cadastro");
                row.CreateCell(2).SetCellValue(report.Summary.PreSign);
                row.CreateCell(3).SetCellValue("");
                row.CreateCell(4).SetCellValue("");

                ISheet customerSheet = workbook.CreateSheet("Clientes");
                row = customerSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Nome");
                row.CreateCell(1).SetCellValue("Sobrenome");
                row.CreateCell(2).SetCellValue("Data Nascimento");
                row.CreateCell(3).SetCellValue("RG");
                row.CreateCell(4).SetCellValue("CPF");
                row.CreateCell(5).SetCellValue("Telefone");
                row.CreateCell(6).SetCellValue("Celular");
                row.CreateCell(7).SetCellValue("E-mail");
                row.CreateCell(8).SetCellValue("Endereço");
                row.CreateCell(9).SetCellValue("Número");
                row.CreateCell(10).SetCellValue("Complemento");
                row.CreateCell(11).SetCellValue("CEP");
                row.CreateCell(12).SetCellValue("Cidade");
                row.CreateCell(13).SetCellValue("Estado");
                row.CreateCell(14).SetCellValue("Data Cadastro");
                row.CreateCell(15).SetCellValue("Último Login");
                row.CreateCell(16).SetCellValue("Status");

                int idx = 1;
                foreach (var customer in report.Customers)
                {
                    row = customerSheet.CreateRow(idx);
                    row.CreateCell(0).SetCellValue(customer.Name);
                    row.CreateCell(1).SetCellValue(customer.Surname);
                    row.CreateCell(2).SetCellValue(customer.Birthday);
                    row.CreateCell(3).SetCellValue(customer.RG);
                    row.CreateCell(4).SetCellValue(customer.CPF);
                    row.CreateCell(5).SetCellValue(customer.Phone);
                    row.CreateCell(6).SetCellValue(customer.Cellphone);
                    row.CreateCell(7).SetCellValue(customer.Email);
                    row.CreateCell(8).SetCellValue(customer.Street);
                    row.CreateCell(9).SetCellValue(customer.Number);
                    row.CreateCell(10).SetCellValue(customer.Complement);
                    row.CreateCell(11).SetCellValue(customer.Zipcode);
                    row.CreateCell(12).SetCellValue(customer.City);
                    row.CreateCell(13).SetCellValue(customer.State);
                    row.CreateCell(14).SetCellValue(customer.Created);
                    row.CreateCell(15).SetCellValue(customer.LastLogin);
                    row.CreateCell(16).SetCellValue(customer.Status);

                    idx++;
                }

                ISheet benefitViewSheet = workbook.CreateSheet("Visualização Benefício");
                row = benefitViewSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Parceiro");
                row.CreateCell(1).SetCellValue("Nome");
                row.CreateCell(2).SetCellValue("Total");
                row.CreateCell(3).SetCellValue("Últimos 7 dias");

                idx = 1;
                foreach (var item in report.BenefitViews)
                {
                    row = benefitViewSheet.CreateRow(idx);
                    row.CreateCell(0).SetCellValue(item.PartnerName);
                    row.CreateCell(1).SetCellValue(item.Name);
                    row.CreateCell(2).SetCellValue(item.Total);
                    row.CreateCell(3).SetCellValue(item.TotalWeek);

                    idx++;
                }

                ISheet benefitUseSheet = workbook.CreateSheet("Utilização Benefício");
                row = benefitUseSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Parceiro");
                row.CreateCell(1).SetCellValue("Nome");
                row.CreateCell(2).SetCellValue("Total");
                row.CreateCell(3).SetCellValue("Últimos 7 dias");

                idx = 1;
                foreach (var item in report.BenefitsUsed)
                {
                    row = benefitUseSheet.CreateRow(idx);
                    row.CreateCell(0).SetCellValue(item.PartnerName);
                    row.CreateCell(1).SetCellValue(item.Name);
                    row.CreateCell(2).SetCellValue(item.Total);
                    row.CreateCell(3).SetCellValue(item.TotalWeek);

                    idx++;
                }

                workbook.Write(fs);
                ret = true;
            }
            return ret;
        }
    }
}
