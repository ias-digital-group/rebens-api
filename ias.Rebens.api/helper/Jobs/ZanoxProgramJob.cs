﻿using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ias.Rebens.api.helper
{
    public class ZanoxProgramJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;

        public ZanoxProgramJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            LoadIncentives();
        }

        public void LoadIncentives()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IZanoxProgramRepository programRepo = serviceScope.ServiceProvider.GetService<IZanoxProgramRepository>();
                IZanoxIncentiveRepository incentiveRepo = serviceScope.ServiceProvider.GetService<IZanoxIncentiveRepository>();

                log.Create("ZanoxProgramJob", "START", "", "");

                var zanox = new Integration.ZanoxHelper();
                var incentives = zanox.GetIncentives(out string error);
                if (string.IsNullOrEmpty(error))
                {
                    log.Create("ZanoxProgramJob", $"Has {incentives.Count} incentives.", error, "");
                    var programs = incentives.Select(i => i.IdProgram).Distinct();
                    foreach (var programId in programs)
                    {
                        var program = zanox.GetProgram(programId, out error);
                        if (string.IsNullOrEmpty(error))
                        {
                            if (programRepo.Save(program, out error))
                            {
                                foreach (var incentive in incentives.Where(i => i.IdProgram == programId))
                                {
                                    if (!incentiveRepo.Save(incentive, out error))
                                        log.Create("ZanoxProgramJob", $"ERROR Incentive: {incentive.Id}", error, "");
                                    Thread.Sleep(100);
                                }
                            }
                            else
                                log.Create("ZanoxProgramJob", $"ERROR Program: {program.Id}", error, "");
                        }
                        else
                            log.Create("ZanoxProgramJob", "ERROR", error, "");
                        //Thread.Sleep(1000);
                    }
                }
                else
                    log.Create("ZanoxProgramJob", "ERROR", error, "");

                log.Create("ZanoxProgramJob", "END", "", "");

            }
        }
    }
}
