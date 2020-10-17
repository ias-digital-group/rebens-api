using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper
{
    public class ExcelHelper
    {
        //https://dzone.com/articles/import-and-export-excel-file-in-asp-net-core-31-ra
        public string GenerateExcel()
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet summarySheet = workbook.CreateSheet("Resumo");

            return "";
        }
    }
}
