using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class CustomerReferalRepository : ICustomerReferalRepository
    {
        private string _connectionString;
        public CustomerReferalRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(CustomerReferal customerReferal, int idOperation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(!db.CustomerReferal.Any(c => c.Email == customerReferal.Email && c.Customer.IdOperation == idOperation))
                    {
                        customerReferal.Modified = customerReferal.Created = DateTime.UtcNow;
                        db.CustomerReferal.Add(customerReferal);
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Esse e-mail já foi indicado para participar do clube.";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar uma referência. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool CheckLimit(int idOperation, int idCustomer, out int limit, out string error)
        {
            bool ret = true;
            error = null;
            limit = 0;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var staticText = db.StaticText.SingleOrDefault(c => c.IdOperation == idOperation && c.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                    var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(staticText.Html);
                    foreach(var module in config.Modules)
                    {
                        if(module.Name == "customerReferal")
                        {
                            foreach(var field in module.Info.Fields)
                            {
                                if(field.Name == "max")
                                {
                                    if (int.TryParse(field.Data, out int temp))
                                        limit = temp;

                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (limit == 0)
                        ret = true;
                    else
                    {
                        var count = db.CustomerReferal.Count(c => c.IdCustomer == idCustomer);
                        ret = count < limit;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.CheckLimit", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar verificar o limite. (erro:" + idLog + ")";
                ret = false;
            }

            return ret;
        }

        public bool Delete(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var cr = db.CustomerReferal.SingleOrDefault(c => c.Id == id);
                    db.CustomerReferal.Remove(cr);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a referência. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<CustomerReferal> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<CustomerReferal> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CustomerReferal.Where(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)) && a.IdCustomer == idCustomer);
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
                        case "status asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "status desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.CustomerReferal.Count(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)) && a.IdCustomer == idCustomer);

                    ret = new ResultPage<CustomerReferal>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CustomerReferal> ListPage(int page, int pageItems, string word, string sort, int? idOperation, out string error)
        {
            ResultPage<CustomerReferal> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CustomerReferal.Where(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)) && 
                                    (!idOperation.HasValue || (idOperation.HasValue && a.Customer.IdOperation == idOperation.Value)));
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
                        case "status asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "status desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.CustomerReferal.Count(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)) &&
                                    (!idOperation.HasValue || (idOperation.HasValue && a.Customer.IdOperation == idOperation.Value)));

                    ret = new ResultPage<CustomerReferal>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CustomerReferal Read(int id, out string error)
        {
            CustomerReferal ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CustomerReferal.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a referência. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CustomerReferal ReadByEmail(string email, int idOperation, out string error)
        {
            CustomerReferal ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CustomerReferal.SingleOrDefault(c => c.Email == email && c.Customer.IdOperation == idOperation);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.ReadByEmail", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a referência. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CustomerReferal customerReferal, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CustomerReferal.SingleOrDefault(c => c.Id == customerReferal.Id);
                    if (update != null)
                    {
                        update.Email = customerReferal.Email;
                        update.IdStatus = customerReferal.IdStatus;
                        update.Modified = DateTime.Now;
                        update.Name = customerReferal.Name;
                        update.DegreeOfKinship = customerReferal.DegreeOfKinship;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Referência não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a referência. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool ChangeStatus(int id, Enums.CustomerReferalStatus status, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var cr = db.CustomerReferal.SingleOrDefault(c => c.Id == id);
                    cr.IdStatus = (int)status;
                    cr.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerReferalRepository.ChangeStatus", ex.Message, $"id:{id}, status:{status.ToString()}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar alterar o status a referência. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
