using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ias.Rebens.api.helper
{
    public class BenefitLinkCheckerJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;
        public BenefitLinkCheckerJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IBenefitRepository repo = serviceScope.ServiceProvider.GetService<IBenefitRepository>();

                log.Create("BenefitLinkCheckerJob", "START", "", "");

                var benefits = repo.ListToCheckLinks();

                if (benefits != null)
                {
                    List<string> error = new List<string>();
                    foreach (var benefit in benefits)
                    {
                        if (Uri.TryCreate(benefit.Link, UriKind.RelativeOrAbsolute, out Uri uriResult))
                        {
                            HttpWebRequest request = WebRequest.Create(benefit.Link) as HttpWebRequest;
                            request.Method = "GET";
                            request.Timeout = 30000;
                            try
                            {
                                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                                if (response.StatusCode != HttpStatusCode.OK)
                                    error.Add($"Beneficio: {benefit.Id} ({benefit.Link}) - Erro: {response.StatusCode} | {response.StatusDescription}");
                            }
                            catch (WebException ex)
                            {
                                error.Add($"Beneficio: {benefit.Id} ({benefit.Link}) - Erro: {ex.Status} | {ex.Message}");
                            }
                            Thread.Sleep(100);
                        }
                        else
                        {
                            error.Add($"Beneficio: {benefit.Id} ({benefit.Link}) - Erro: Invalid URL");
                        }
                    }

                    if (error.Count > 0)
                    {
                        string body = "<p>Os benefícios abaixo estão com problemas no link: </p><br /><p>";
                        foreach (var item in error)
                        {
                            body += $" - {item} <br /><br />";
                        }
                        body += "</p>";
                        var listDestinataries = new Dictionary<string, string>() { { "botlink@sistemarebens.com.br", "Bot Link" },  { "israel@iasdigitalgroup.com", "Israel Silva" }, { "fernando@fbbcomunicacao.com.br", "Fernando" } };
                        Helper.EmailHelper.SendAdminEmail(listDestinataries, "[Rebens] - Links com problemas", body, out _);
                    }
                }
            }
        }
    }
}
