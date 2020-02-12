using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class CustomerPromoterRepository : ICustomerPromoterRepository
    {
        private string _connectionString;
        public CustomerPromoterRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public CustomerPromoterRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ResultPage<Customer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, int? idPromoter = null)
        {
            ResultPage<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                    && (!idPromoter.HasValue || (idPromoter.HasValue && a.CustomerPromoters.Any(p => p.IdAminUser == idPromoter)))
                                    && a.CustomerPromoters.Any()
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
                        case "cpf asc":
                            tmpList = tmpList.OrderBy(f => f.Cpf);
                            break;
                        case "cpf desc":
                            tmpList = tmpList.OrderByDescending(f => f.Cpf);
                            break;
                        case "operation asc":
                            tmpList = tmpList.OrderBy(f => f.Operation.Title);
                            break;
                        case "operation desc":
                            tmpList = tmpList.OrderByDescending(f => f.Operation.Title);
                            break;
                        case "promoter asc":
                            tmpList = tmpList.OrderBy(f => f.CustomerPromoters.First().AdminUser.Name);
                            break;
                        case "promoter desc":
                            tmpList = tmpList.OrderByDescending(f => f.CustomerPromoters.First().AdminUser.Name);
                            break;
                        case "status asc":
                            tmpList = tmpList.OrderBy(f => f.Status);
                            break;
                        case "status desc":
                            tmpList = tmpList.OrderByDescending(f => f.Status);
                            break;
                    }

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    ret = new ResultPage<Customer>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerPromoterRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os clientes. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Customer Read(int id, out string error)
        {
            Customer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Customer.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerPromoterRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o cliente. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Create(Customer customer, int idPromoter, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(db.Customer.Any(c => c.IdOperation == customer.IdOperation && c.Cpf == customer.Cpf))
                        error = "O cpf já está cadastrado em nossa base!";
                    if (db.Customer.Any(c => c.IdOperation == customer.IdOperation && c.Email == customer.Email))
                        error = "O e-mail já está cadastrado em nossa base!";
                    else
                    {
                        customer.Modified = customer.Created = DateTime.UtcNow;
                        db.Customer.Add(customer);
                        db.SaveChanges();

                        db.CustomerPromoter.Add(new CustomerPromoter()
                        {
                            IdAminUser = idPromoter,
                            IdCustomer = customer.Id,
                            Created = DateTime.UtcNow,
                            Modified = DateTime.UtcNow
                        });
                        db.SaveChanges();
                        error = null;
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerPromoterRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cliente. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
