using System;
using System.IO;
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
                IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                IScratchcardDrawRepository drawRepo = serviceScope.ServiceProvider.GetService<IScratchcardDrawRepository>();
                IHostingEnvironment hostingEnvironment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();

                string newPath = Path.Combine(hostingEnvironment.WebRootPath, "files", "scratchcard");
                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.daily);
                foreach (var item in list)
                {
                    var users = repo.ListCustomers(item.IdOperation, item.Type);
                    foreach(var user in users)
                    {
                        drawRepo.SaveRandom(item.Id, newPath, user, DateTime.Now.Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.Date : null), out _);
                    }
                }
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
                IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                IScratchcardDrawRepository drawRepo = serviceScope.ServiceProvider.GetService<IScratchcardDrawRepository>();
                IHostingEnvironment hostingEnvironment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();

                string newPath = Path.Combine(hostingEnvironment.WebRootPath, "files", "scratchcard");
                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.weekly);
                foreach (var item in list)
                {
                    var users = repo.ListCustomers(item.IdOperation, item.Type);
                    foreach (var user in users)
                    {
                        drawRepo.SaveRandom(item.Id, newPath, user, DateTime.Now.Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.Date : null), out _);
                    }
                }
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
                IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                IScratchcardDrawRepository drawRepo = serviceScope.ServiceProvider.GetService<IScratchcardDrawRepository>();
                IHostingEnvironment hostingEnvironment = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();

                string newPath = Path.Combine(hostingEnvironment.WebRootPath, "files", "scratchcard");
                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.monthly);
                foreach(var item in list)
                {
                    var users = repo.ListCustomers(item.IdOperation, item.Type);
                    foreach (var user in users)
                    {
                        drawRepo.SaveRandom(item.Id, newPath, user, DateTime.Now.Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.Date : null), out _);
                    }
                }
            }
        }
    }
}
