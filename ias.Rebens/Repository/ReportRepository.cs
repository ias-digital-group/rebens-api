using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class ReportRepository : IReportRepository
    {
        private string _connectionString;
        public ReportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public Dashboard LoadDashboard(out string error, DateTime? begin = null, DateTime? end = null, int? idOperation = null)
        {
            Dashboard ret = null;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = new Dashboard()
                    {
                        Benefits = new List<DashboardGraph>(),
                        Users = new List<DashboardUser>()
                    };

                    // Usuários
                    var operations = db.Operation.Where(o => o.Active && (!idOperation.HasValue ||(idOperation.HasValue && o.Id == idOperation)));
                    foreach (var operation in operations)
                    {
                        // Usuários por status
                        var item = new DashboardUser() { Operation = operation.Title, Users = new DashboardGraph(), Region = new List<DashboardGraph>() };
                        item.Users.Title = "Usuários";
                        item.Users.Type = "pie";
                        item.Users.Items = new List<DashboardGraphItem>();
                        int totalPaid = 0;
                        if (operation.Id == 1)
                        {
                            totalPaid = db.MoipSignature.Where(s => s.Customer.IdOperation == operation.Id && s.Status.ToLower() == "active").Count();
                            item.Users.Items.Add(new DashboardGraphItem() { Title = "Associados", Total = totalPaid });
                        }
                        var users = from c in db.Customer
                                    where c.IdOperation == operation.Id
                                    && c.Status != (int)Enums.CustomerStatus.ChangePassword
                                    group c by c.Status into g
                                    select new { idStatus = g.Key, total = g.Count() };
                        foreach (var user in users)
                        {
                            item.Users.Items.Add(new DashboardGraphItem() {
                                Title = Enums.EnumHelper.GetEnumDescription((Enums.CustomerStatus)user.idStatus),
                                Total = user.idStatus == (int)Enums.CustomerStatus.Active ? user.total - totalPaid : user.total });
                        }

                        // indicações
                        item.TotalReferals = db.CustomerReferal.Count(c => c.Customer.IdOperation == operation.Id);

                        // Região - estados
                        var graphState = new DashboardGraph() {
                                            Title = "Estados",
                                            Type = "pie"
                                         };
                        graphState.Items = (from c in db.Customer
                                          where c.IdOperation == operation.Id
                                          group c.Address by c.Address.State into g
                                          orderby g.Count()
                                          select new DashboardGraphItem() { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                        item.Region.Add(graphState);

                        // Região - estados
                        var graphCity = new DashboardGraph()
                        {
                            Title = "Cidades",
                            Type = "pie"
                        };
                        graphCity.Items = (from c in db.Customer
                                            where c.IdOperation == operation.Id
                                            group c.Address by c.Address.City into g
                                            orderby g.Count()
                                            select new DashboardGraphItem() { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                        item.Region.Add(graphCity);

                        // Região - Bairros
                        var graphNeighborhood = new DashboardGraph()
                        {
                            Title = "Bairros",
                            Type = "pie"
                        };
                        graphNeighborhood.Items = (from c in db.Customer
                                            where c.IdOperation == operation.Id
                                            group c.Address by c.Address.Neighborhood into g
                                            orderby g.Count()
                                            select new DashboardGraphItem() { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                        item.Region.Add(graphNeighborhood);
                        ret.Users.Add(item);
                    }

                    // Benefícios - Mais vistos
                    var graphView = new DashboardGraph()
                    {
                        Title = "Mais Vistos",
                        Type = "bar"
                    };
                    graphView.Items = (from b in db.BenefitView
                                       where (!idOperation.HasValue || (idOperation.HasValue && b.Customer.IdOperation == idOperation.Value))
                                       group b by b.Benefit.Name into g
                                       orderby g.Count()
                                       select new DashboardGraphItem() { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                    ret.Benefits.Add(graphView);

                    // Benefícios - Mais usados
                    var graphUsed = new DashboardGraph()
                    {
                        Title = "Mais Usados",
                        Type = "bar"
                    };
                    graphUsed.Items = (from b in db.BenefitUse
                                     where (!idOperation.HasValue || (idOperation.HasValue && b.Customer.IdOperation == idOperation.Value))
                                     group b by b.Name into g
                                     orderby g.Count()
                                     select new DashboardGraphItem() { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                    ret.Benefits.Add(graphUsed);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ReportRepository.LoadDashboard", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar carregar o Dashboard. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
