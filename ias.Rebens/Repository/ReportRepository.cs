﻿using Microsoft.EntityFrameworkCore;
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

        public ResultPage<CustomerReportItem> ListCustomerPage(int page, int pageItems, string word, string sort, out string error, int? idOperation)
        {
            ResultPage<CustomerReportItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Include("Operation").Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                    && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "email asc":
                            tmpList = tmpList.OrderBy(f => f.Email);
                            break;
                        case "email desc":
                            tmpList = tmpList.OrderByDescending(f => f.Email);
                            break;
                        case "birthday asc":
                            tmpList = tmpList.OrderBy(f => f.Birthday);
                            break;
                        case "birthday desc":
                            tmpList = tmpList.OrderByDescending(f => f.Birthday);
                            break;
                    }

                    var customers = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Customer.Count(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation)) 
                                    && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)));

                    var list = new List<CustomerReportItem>();
                    customers.ForEach(c => {
                        list.Add(new CustomerReportItem(c, c.Operation.Title));
                    });

                    ret = new ResultPage<CustomerReportItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ReportRepository.ListCustomerPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os clientes. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
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

        public ResultPage<BenefitReportItem> ListBenefitUsePage(int page, int pageItems, string word, string sort, out string error, int? idOperation, DateTime? start, DateTime? end)
        {
            ResultPage<BenefitReportItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Include("BenefitUses").Where(b => (!idOperation.HasValue || (idOperation.HasValue && idOperation == b.IdOperation))
                                    && (string.IsNullOrEmpty(word) || b.Name.Contains(word) || b.Title.Contains(word))
                                    && (!start.HasValue || (start.HasValue && b.BenefitUses.Any(u => u.Created >= start.Value) ))
                                    && (!end.HasValue || (end.HasValue && b.BenefitUses.Any(u => u.Created <= end.Value))));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var benefits = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => (!idOperation.HasValue || (idOperation.HasValue && idOperation == b.IdOperation))
                                    && (string.IsNullOrEmpty(word) || b.Name.Contains(word) || b.Title.Contains(word))
                                    && (!start.HasValue || (start.HasValue && b.BenefitUses.Any(u => u.Created >= start.Value)))
                                    && (!end.HasValue || (end.HasValue && b.BenefitUses.Any(u => u.Created <= end.Value))));

                    var list = new List<BenefitReportItem>();
                    benefits.ForEach(b => {
                        int totalUse = b.BenefitUses.Count(u => (!start.HasValue || (start.HasValue && u.Created >= start.Value))
                                    && (!end.HasValue || (end.HasValue && u.Created <= end.Value)));
                        list.Add(new BenefitReportItem(b, totalUse));
                    });

                    ret = new ResultPage<BenefitReportItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ReportRepository.ListBenefitUsePage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os Benefícios. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}