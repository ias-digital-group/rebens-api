using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    public class ScratchcardJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;


        public ScratchcardJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                //IScratchcardRepository repo = serviceScope.ServiceProvider.GetService<IScratchcardRepository>();
                //var tmp = repo.Read(1, out string error);
                //Console.WriteLine("teste " + tmp.Id);
            }
        }
    }
}
