using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ias.Rebens.api.helper
{
    public class CustomerValidationJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        public CustomerValidationJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                //ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IOperationRepository operationRepo = serviceScope.ServiceProvider.GetService<IOperationRepository>();

                //log.Create("CustomerValidationJob", "START", "", "");

                try
                {
                    var operations = operationRepo.ListActive();
                    if (operations != null)
                    {
                        //log.Create("CustomerValidationJob", "Operations", operations.Count.ToString(), "");
                        //IStaticTextRepository staticRepo = serviceScope.ServiceProvider.GetService<IStaticTextRepository>();
                        ICustomerRepository repo = serviceScope.ServiceProvider.GetService<ICustomerRepository>();
                        foreach(var operation in operations)
                        {
                            var customers = repo.ListForCustomerValidationReminder(operation.Id);
                            if (customers != null)
                            {
                                //log.Create("CustomerValidationJob", operation.Title +  " - Customers", customers.Count.ToString(), "");

                                //string fromEmail = operationRepo.GetConfigurationOption(operation.Id, "contact-email", out _);
                                //if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                                Console.WriteLine($"{operation.Title} - {customers.Count}");

                                foreach (var customer in customers)
                                {
                                    //if(Helper.EmailHelper.SendCustomerValidation(staticRepo, operation, customer, fromEmail, out _))
                                    //{
                                        repo.SaveLog(customer.Id, Enums.CustomerLogAction.validationReminder, "");
                                    //}
                                    if (customer.Status != 3)
                                    {
                                        Console.WriteLine($"{customer.Id} - {customer.Name} - {customer.Status} | {customer.ComplementaryStatus}");
                                        Thread.Sleep(100);
                                    }
                                }
                            }
                            //else
                            //    log.Create("CustomerValidationJob", operation.Title + " - Customers", "no customers", "");
                            //Thread.Sleep(1000);
                        }
                    }
                    //else
                    //    log.Create("CustomerValidationJob", "Operations", "no operations", "");
                }
                catch(Exception ex)
                {
                    //log.Create("CustomerValidationJob", ex.Message, "ERROR", ex.StackTrace);
                }

                //log.Create("CustomerValidationJob", "END", "", "");

            }
        }
    }
}
