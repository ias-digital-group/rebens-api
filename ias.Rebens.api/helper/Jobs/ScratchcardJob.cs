using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    public class ScratchcardDailyJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;


        public ScratchcardDailyJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                IScratchcardDrawRepository drawRepo = serviceScope.ServiceProvider.GetService<IScratchcardDrawRepository>();
                IHostingEnvironment environment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();

                log.Create("ScratchcardDailyJob", "START", "", "");
                string path = Path.Combine(environment.WebRootPath, "files", "scratchcard");

                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.daily);
                foreach (var item in list)
                {
                    var users = repo.ListCustomers(item.IdOperation, item.Type);
                    foreach(var user in users)
                    {
                        drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.Date : null), out _);
                        Thread.Sleep(100);
                    }

                    Thread.Sleep(1000);
                }
                log.Create("ScratchcardDailyJob", "END", "", "");
            }
        }
    }

    public class ScratchcardWeeklyJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;


        public ScratchcardWeeklyJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                IScratchcardDrawRepository drawRepo = serviceScope.ServiceProvider.GetService<IScratchcardDrawRepository>();
                IHostingEnvironment environment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
                string path = Path.Combine(environment.WebRootPath, "files", "scratchcard");

                log.Create("ScratchcardWeeklyJob", "START", "", "");
                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.weekly);
                foreach (var item in list)
                {
                    var users = repo.ListCustomers(item.IdOperation, item.Type);
                    foreach (var user in users)
                    {
                        switch (item.Quantity)
                        {
                            case 1:
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Wednesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                break;
                            case 2:
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Tuesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Thursday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                break;
                            case 3:
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Monday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Wednesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Friday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                break;
                            case 4:
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Monday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Tuesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Thursday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Friday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                break;
                            case 5:
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Monday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Tuesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Wednesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _); 
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Thursday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Friday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                
                                break;
                            case 6:
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Monday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Tuesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Wednesday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Thursday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Friday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                Thread.Sleep(100);
                                drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.AddDays((int)DayOfWeek.Saturday).Date : null), out _);
                                break;
                        }
                        Thread.Sleep(100);
                    }
                }
                log.Create("ScratchcardWeeklyJob", "END", "", "");
            }
        }
    }

    public class ScratchcardMonthlyJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;


        public ScratchcardMonthlyJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                IScratchcardDrawRepository drawRepo = serviceScope.ServiceProvider.GetService<IScratchcardDrawRepository>();
                IHostingEnvironment environment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
                string path = Path.Combine(environment.WebRootPath, "files", "scratchcard");

                log.Create("ScratchcardMonthlyJob", "START", "", "");
                var lastDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).Date;
                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.monthly);
                foreach(var item in list)
                {
                    var daysInBetween = Convert.ToInt32(lastDayOfMonth.Day / item.Quantity);
                    var users = repo.ListCustomers(item.IdOperation, item.Type);

                    foreach (var user in users)
                    {
                        for(int i = 0; i< item.Quantity; i++)
                        {
                            drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.Date.AddDays(daysInBetween*i), (item.ScratchcardExpire ? (DateTime?)lastDayOfMonth : null), out _);
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(1000);
                    }
                }
                log.Create("ScratchcardMonthlyJob", "END", "", "");
            }
        }
    }
}
