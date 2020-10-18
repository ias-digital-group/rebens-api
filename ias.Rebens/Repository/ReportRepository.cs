using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using ias.Rebens.Models;
using Remotion.Linq.Clauses;
using Amazon.Route53.Model.Internal.MarshallTransformations;

namespace ias.Rebens
{
    public class ReportRepository : IReportRepository
    {
        private string _connectionString;

        public ReportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public ResultPage<CustomerReportItem> ListCustomerPage(int page, int pageItems, string word, string sort, out string error, int? idOperation, int? idPartner, int? status)
        {
            ResultPage<CustomerReportItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Include("Operation").Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                    && (!idPartner.HasValue || (idPartner.HasValue && a.IdOperationPartner == idPartner))
                                    && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word))
                                    && (!status.HasValue || (status.HasValue && a.Status == status)));
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
                    var total = tmpList.Count();

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
                    ret = new Dashboard() { Operations = new List<DashboardOperation>() };

                    // Usuários
                    var operations = db.Operation.Where(o => o.Active && (!idOperation.HasValue ||(idOperation.HasValue && o.Id == idOperation)));
                    foreach (var operation in operations)
                    {
                        if(db.Customer.Any(c => c.IdOperation == operation.Id))
                        {
                            var dashOperation = new DashboardOperation() { Operation = operation.Title };

                            // Usuários por status
                            dashOperation.Users = new DashboardGraph() { Title = "Usuários", Type = "pie", Labels = new List<string>(), Data = new List<int>() };
                            int totalPaid = 0;
                            if (operation.Id == 1)
                            {
                                totalPaid = db.MoipSignature.Where(s => s.Customer.IdOperation == operation.Id && s.Status.ToLower() == "active").Count();
                                dashOperation.Users.Labels.Add("Associados");
                                dashOperation.Users.Data.Add(totalPaid);
                            }

                            var users = from c in db.Customer
                                        where c.IdOperation == operation.Id
                                        && c.Status != (int)Enums.CustomerStatus.ChangePassword
                                        && c.Status != (int)Enums.CustomerStatus.PreSignup
                                        group c by c.Status into g
                                        select new { idStatus = g.Key, total = g.Count() };
                            foreach (var user in users)
                            {
                                dashOperation.Users.Labels.Add(Enums.EnumHelper.GetEnumDescription((Enums.CustomerStatus)user.idStatus));
                                dashOperation.Users.Data.Add(user.total);
                            }

                            // indicações
                            dashOperation.TotalReferals = db.Customer.Count(c => c.IdOperation == operation.Id && c.IdCustomerReferer.HasValue);

                            // Região - estados
                            var tmpStates = (from c in db.Customer
                                             where c.IdOperation == operation.Id
                                             && c.Status != (int)Enums.CustomerStatus.ChangePassword
                                             && c.Status != (int)Enums.CustomerStatus.PreSignup
                                             && !string.IsNullOrEmpty(c.Address.State)
                                             group c.Address by c.Address.State into g
                                             orderby g.Count()
                                             select new { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                            if (tmpStates.Count > 0)
                            {
                                dashOperation.RegionState = new DashboardGraph()
                                {
                                    Title = "Estados",
                                    Type = "pie",
                                    Labels = new List<string>(),
                                    Data = new List<int>()
                                };
                                foreach (var i in tmpStates)
                                {
                                    dashOperation.RegionState.Labels.Add(i.Title);
                                    dashOperation.RegionState.Data.Add(i.Total);
                                }
                            }

                            // Região - cidades
                            var tmpCities = (from c in db.Customer
                                             where c.IdOperation == operation.Id
                                             && c.Status != (int)Enums.CustomerStatus.ChangePassword
                                             && c.Status != (int)Enums.CustomerStatus.PreSignup
                                             && !string.IsNullOrEmpty(c.Address.City)
                                             group c.Address by c.Address.City into g
                                             orderby g.Count()
                                             select new { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                            if(tmpCities.Count > 0)
                            {
                                dashOperation.RegionCity = new DashboardGraph()
                                {
                                    Title = "Cidades",
                                    Type = "pie",
                                    Labels = new List<string>(),
                                    Data = new List<int>()
                                };
                                foreach (var i in tmpCities)
                                {
                                    dashOperation.RegionCity.Labels.Add(i.Title);
                                    dashOperation.RegionCity.Data.Add(i.Total);
                                }
                            }
                            
                            // Região - Bairros
                            var tmpNeighborhood = (from c in db.Customer
                                                   where c.IdOperation == operation.Id
                                                   && c.Status != (int)Enums.CustomerStatus.ChangePassword
                                                   && c.Status != (int)Enums.CustomerStatus.PreSignup
                                                   && !string.IsNullOrEmpty(c.Address.Neighborhood)
                                                   group c.Address by c.Address.Neighborhood into g
                                                   orderby g.Count()
                                                   select new { Title = g.Key, Total = g.Count() }).Take(10).ToList();
                            if(tmpNeighborhood.Count > 0)
                            {
                                dashOperation.RegionNeighborhood = new DashboardGraph()
                                {
                                    Title = "Bairros",
                                    Type = "pie",
                                    Labels = new List<string>(),
                                    Data = new List<int>()
                                };
                                foreach (var i in tmpNeighborhood)
                                {
                                    dashOperation.RegionNeighborhood.Labels.Add(i.Title);
                                    dashOperation.RegionNeighborhood.Data.Add(i.Total);
                                }
                            }
                            ret.Operations.Add(dashOperation);
                        }
                    }

                    // Benefícios - Mais vistos
                    ret.BenefitView = new DashboardGraph()
                    {
                        Title = "Mais Vistos",
                        Type = "bar",
                        Labels = new List<string>(),
                        Data = new List<int>()
                    };
                    var tmpView = (from b in db.BenefitView
                                       where (!idOperation.HasValue || (idOperation.HasValue && b.Customer.IdOperation == idOperation.Value))
                                       group b by b.Benefit.Name into g
                                       orderby g.Count()
                                       select new { Title = g.Key, Total = g.Count() }).OrderByDescending(t => t.Total).Take(10).ToList();
                    foreach (var i in tmpView)
                    {
                        ret.BenefitView.Labels.Add(i.Title);
                        ret.BenefitView.Data.Add(i.Total);
                    }


                    // Benefícios - Mais usados
                    ret.BenefitUse = new DashboardGraph()
                    {
                        Title = "Mais Usados",
                        Type = "bar",
                        Labels = new List<string>(),
                        Data = new List<int>()
                    };
                    var tmpUsed = (from b in db.BenefitUse
                                     where (!idOperation.HasValue || (idOperation.HasValue && b.Customer.IdOperation == idOperation.Value))
                                     group b by b.Name into g
                                     orderby g.Count()
                                     select new { Title = g.Key, Total = g.Count() }).OrderByDescending(t => t.Total).Take(10).ToList();
                    foreach (var i in tmpUsed)
                    {
                        string tit = i.Title;
                        if (tit.Length > 14)
                            tit = tit.Substring(0, 11) + "...";
                        ret.BenefitUse.Labels.Add(tit);
                        ret.BenefitUse.Data.Add(i.Total);
                    }

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

        public UnicsulReport UnicsulWeekly()
        {
            UnicsulReport result = new UnicsulReport()
            {
                Summary = new UnicsulReportSummary(),
                BenefitsUsed = new List<UnicsulReportBenefit>(),
                BenefitViews = new List<UnicsulReportBenefit>(),
                Customers = new List<UnicsulReportCustomer>()
            };

            var dt = DateTime.UtcNow.Date.AddDays(1);
            DateTime end = DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday ? DateTime.UtcNow.AddDays(-1).Date : DateTime.UtcNow.AddDays(-((int)dt.DayOfWeek + 2)).Date;
            DateTime start = end.AddDays(-6).Date.AddMinutes(-1);
            end = end.AddDays(1).Date;
            
            using(var db = new RebensContext(this._connectionString))
            {
                var lastWeekUsers = (from c in db.Customer
                                     where c.IdOperation == 66
                                         && c.Created > start
                                         && c.Created < end
                                     group c by c.Status into g
                                     select new
                                     {
                                         Status = g.Key,
                                         Count = g.Count(x => x.Status == g.Key)
                                     }).ToList();
                result.Summary.CompleteWeek = lastWeekUsers.Any(c => c.Status == (int)Enums.CustomerStatus.Active) ? lastWeekUsers.SingleOrDefault(c => c.Status == (int)Enums.CustomerStatus.Active).Count : 0;
                result.Summary.IncompleteWeek = lastWeekUsers.Any(c => c.Status == (int)Enums.CustomerStatus.Incomplete) ? lastWeekUsers.SingleOrDefault(c => c.Status == (int)Enums.CustomerStatus.Incomplete).Count : 0;
                result.Summary.IncompleteWeek += lastWeekUsers.Any(c => c.Status == (int)Enums.CustomerStatus.ChangePassword) ? lastWeekUsers.SingleOrDefault(c => c.Status == (int)Enums.CustomerStatus.ChangePassword).Count : 0;
                result.Summary.ValidateWeek = lastWeekUsers.Any(c => c.Status == (int)Enums.CustomerStatus.Validation) ? lastWeekUsers.SingleOrDefault(c => c.Status == (int)Enums.CustomerStatus.Validation).Count : 0;

                var listCustomers = db.Customer.Include("Address").Where(c => c.IdOperation == 66);

                var benefitViews = (from bv in db.BenefitView
                                   where listCustomers.Any(c => c.Id == bv.IdCustomer)
                                   && bv.Created > start
                                   && bv.Created < end
                                   select bv.IdBenefit).Distinct().ToList();
                var benefitsUsed = (from bv in db.BenefitUse
                                    where listCustomers.Any(c => c.Id == bv.IdCustomer)
                                    && bv.Created > start
                                    && bv.Created < end
                                    select bv.IdBenefit).Distinct().ToList();

                foreach(var benefitId in benefitViews)
                {
                    var benefit = db.Benefit.Include("Partner").Single(b => b.Id == benefitId);
                    result.BenefitViews.Add(new UnicsulReportBenefit()
                    {
                        Name = benefit.Title,
                        PartnerName = benefit.Partner.Name,
                        Total = db.BenefitView.Count(bv => bv.IdBenefit == benefitId && listCustomers.Any(c => c.Id == bv.IdCustomer)),
                        TotalWeek = db.BenefitView.Count(bv => bv.IdBenefit == benefitId 
                                        && listCustomers.Any(c => c.Id == bv.IdCustomer)
                                        && bv.Created > start
                                        && bv.Created < end)
                    });
                }

                foreach (var benefitId in benefitsUsed)
                {
                    var benefit = db.Benefit.Include("Partner").Single(b => b.Id == benefitId);
                    result.BenefitsUsed.Add(new UnicsulReportBenefit()
                    {
                        Name = benefit.Title,
                        PartnerName = benefit.Partner.Name,
                        Total = db.BenefitView.Count(bv => bv.IdBenefit == benefitId && listCustomers.Any(c => c.Id == bv.IdCustomer)),
                        TotalWeek = db.BenefitView.Count(bv => bv.IdBenefit == benefitId
                                        && listCustomers.Any(c => c.Id == bv.IdCustomer)
                                        && bv.Created > start
                                        && bv.Created < end)
                    });
                }

                foreach(var customer in listCustomers)
                {
                    var tmpCustomer = new UnicsulReportCustomer(customer);
                    if(customer.Status != (int)Enums.CustomerStatus.PreSignup && customer.Status != (int)Enums.CustomerStatus.Inactive)
                    {
                        var tmpLastLogin = db.CustomerLog.Where(l => l.IdCustomer == customer.Id
                                                && l.Action == (int)Enums.CustomerLogAction.login)
                                        .OrderByDescending(l => l.Created)
                                        .FirstOrDefault();
                        if (tmpLastLogin != null)
                            tmpCustomer.LastLogin = tmpLastLogin.Created.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                        
                    result.Customers.Add(tmpCustomer);
                }

                result.Summary.Complete = result.Customers.Count(c => c.StatusId == (int)Enums.CustomerStatus.Active);
                result.Summary.Validate = result.Customers.Count(c => c.StatusId == (int)Enums.CustomerStatus.Validation);
                result.Summary.Incomplete = result.Customers.Count(c => c.StatusId == (int)Enums.CustomerStatus.Incomplete);
                result.Summary.Incomplete += result.Customers.Count(c => c.StatusId == (int)Enums.CustomerStatus.ChangePassword);
                result.Summary.PreSign = result.Customers.Count(c => c.StatusId == (int)Enums.CustomerStatus.PreSignup);
            }
            
            
            return result;   
        }
    }
}
