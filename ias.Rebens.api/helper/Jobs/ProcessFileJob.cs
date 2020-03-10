using FluentScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    public class ProcessFileJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;

        public ProcessFileJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                IFileToProcessRepository repo = serviceScope.ServiceProvider.GetService<IFileToProcessRepository>();
                IOperationCustomerRepository ocRepo = serviceScope.ServiceProvider.GetService<IOperationCustomerRepository>();
                
                var file = repo.GetNextFile();
                if (file != null)
                {
                    repo.UpdateStatus(file.Id, (int)Enums.FileToProcessStatus.Processing, out _);
                    var list = new List<OperationCustomer>();
                    try
                    {
                        using (var stream = new StreamReader(file.Name))
                        {
                            string extension = Path.GetExtension(file.Name);
                            ISheet sheet;
                            if (extension == ".xls")
                            {
                                HSSFWorkbook hssfwb = new HSSFWorkbook(stream.BaseStream); //This will read the Excel 97-2000 formats  
                                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                            }
                            else
                            {
                                XSSFWorkbook hssfwb = new XSSFWorkbook(stream.BaseStream); //This will read 2007 Excel format  
                                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                            }

                            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                            {
                                IRow row = sheet.GetRow(i);
                                if (row == null) continue;
                                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                                if (row.GetCell(0) == null || row.GetCell(1) == null) continue;

                                list.Add(new OperationCustomer()
                                {
                                    Name = row.GetCell(0) != null ? row.GetCell(0).ToString().Trim() : "",
                                    CPF = row.GetCell(1) != null ? row.GetCell(1).ToString().Trim() : "",
                                    Phone = row.GetCell(2) != null ? row.GetCell(2).ToString().Trim() : "",
                                    Cellphone = row.GetCell(3) != null ? row.GetCell(3).ToString().Trim() : "",
                                    Email1 = row.GetCell(4) != null ? row.GetCell(4).ToString().Trim() : "",
                                    Email2 = row.GetCell(5) != null ? row.GetCell(5).ToString().Trim() : "",
                                    Signed = false,
                                    Created = DateTime.UtcNow,
                                    Modified = DateTime.UtcNow,
                                    IdOperation = file.IdOperation.Value
                                });

                            }
                        }
                    }
                    catch
                    {
                        repo.UpdateStatus(file.Id, (int)Enums.FileToProcessStatus.Error, out _);
                    }

                    foreach (var customer in list)
                    {
                        if (ocRepo.Create(customer, out string error))
                            repo.UpdateProcessed(file.Id, out _);
                    }

                    repo.UpdateStatus(file.Id, (int)Enums.FileToProcessStatus.Done, out _);
                }
            }
        }
    }
}
