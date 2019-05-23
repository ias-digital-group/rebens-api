using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ias.Rebens
{
    public class OperationPartnerRepository : IOperationPartnerRepository
    {
        private string _connectionString;
        public OperationPartnerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }
        
        public bool Create(OperationPartner partner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    partner.Modified = partner.Created = DateTime.UtcNow;
                    partner.Deleted = false;
                    db.OperationPartner.Add(partner);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o parceiro. (erro:" + idLog + ")";
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
                    var update = db.OperationPartner.SingleOrDefault(p => p.Id == id);
                    update.Modified = DateTime.UtcNow;
                    update.Deleted = true;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteCustomer(int idCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartnerCustomer.SingleOrDefault(p => p.Id == idCustomer);
                    update.Modified = DateTime.UtcNow;
                    update.Status = (int)Enums.OperationPartnerCustomerStatus.deleted;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.DeleteCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar o cliente parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<OperationPartnerCustomer> ListCustomers(int page, int pageItems, string word, string sort, out string error, int? status = null, int? idOperationPartner = null, int? idOperation = null)
        {
            ResultPage<OperationPartnerCustomer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OperationPartnerCustomer.Include("OperationPartner").Where(c => !c.OperationPartner.Deleted 
                                        && c.Status != (int)Enums.OperationPartnerCustomerStatus.deleted 
                                        && (!idOperationPartner.HasValue || idOperationPartner.Value == 0 || c.IdOperationPartner == idOperationPartner)
                                        && (!idOperation.HasValue || idOperation.Value == 0 || c.OperationPartner.IdOperation == idOperation)
                                        && (!status.HasValue || c.Status == status.Value)
                                        && (string.IsNullOrEmpty(word) || c.Name.Contains(word) || c.Email.Contains(word)));
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
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.OperationPartnerCustomer.Count(c => !c.OperationPartner.Deleted
                                        && c.Status != (int)Enums.OperationPartnerCustomerStatus.deleted
                                        && (!idOperationPartner.HasValue || idOperationPartner.Value == 0 || c.IdOperationPartner == idOperationPartner)
                                        && (!idOperation.HasValue || idOperation.Value == 0 || c.OperationPartner.IdOperation == idOperation)
                                        && (!status.HasValue || c.Status == status.Value)
                                        && (string.IsNullOrEmpty(word) || c.Name.Contains(word) || c.Email.Contains(word)));

                    ret = new ResultPage<OperationPartnerCustomer>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ListCustomers", ex.Message, $"idOperationPartner: {idOperationPartner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os clientes. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<OperationPartner> ListPage(int page, int pageItems, string word, string sort, int idOperation, out string error, bool? status = null)
        {
            ResultPage<OperationPartner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OperationPartner.Where(p => !p.Deleted && p.IdOperation == idOperation && (!status.HasValue || p.Active == status.Value) && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));
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
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.OperationPartner.Count(p => !p.Deleted && p.IdOperation == idOperation && (!status.HasValue || p.Active == status.Value) && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));

                    ret = new ResultPage<OperationPartner>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ListPage", ex.Message,"", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os parceiros. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public OperationPartner Read(int id, out string error)
        {
            OperationPartner ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.OperationPartner.SingleOrDefault(p => !p.Deleted && p.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o parceiro. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public OperationPartnerCustomer ReadCustomer(int idCustomer, out string error)
        {
            OperationPartnerCustomer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.OperationPartnerCustomer.SingleOrDefault(p => p.Status != (int)Enums.OperationPartnerCustomerStatus.deleted && p.Id == idCustomer);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ReadCustomer", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o parceiro. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool CreateCustomer(OperationPartnerCustomer customer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(!db.OperationPartnerCustomer.Any(c => (c.Email == customer.Email || c.Cpf == customer.Cpf) && c.IdOperationPartner == customer.IdOperationPartner))
                    {
                        customer.Modified = customer.Created = DateTime.UtcNow;
                        customer.Status = (int)Enums.OperationPartnerCustomerStatus.newCustomer;
                        db.OperationPartnerCustomer.Add(customer);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.CreateCustomer", ex.Message, "", ex.StackTrace);
                if(ex.InnerException != null)
                    logError.Create("OperationPartnerRepository.CreateCustomer", ex.InnerException.Message, "", ex.InnerException.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(OperationPartner partner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartner.SingleOrDefault(c => c.Id == partner.Id);
                    if (update != null)
                    {
                        update.Active = partner.Active;
                        update.Name = partner.Name;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Parceiro não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool UpdateCustomerStatus(int idCustomer, int status, out string error, out Operation operation, out Customer customer)
        {
            bool ret = false;
            operation = null;
            customer = null;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartnerCustomer.SingleOrDefault(c => c.Id == idCustomer);
                    if (update != null)
                    {
                        if (update.Status == (int)Enums.OperationPartnerCustomerStatus.newCustomer || update.Status == (int)Enums.OperationPartnerCustomerStatus.reproved)
                        {
                            update.Status = status;
                            update.Modified = DateTime.UtcNow;

                            if(update.Status == (int)Enums.OperationPartnerCustomerStatus.approved)
                            {
                                operation = db.Operation.Single(o => o.OperationPartners.Any(p => p.Id == update.IdOperationPartner));
                                int idOperation = operation.Id;

                                customer = db.Customer.SingleOrDefault(c => (c.Cpf == update.Cpf || c.Email == update.Email) && c.IdOperation == idOperation);
                                if(customer == null)
                                {
                                    customer = new Customer()
                                    {
                                        Name = update.Name,
                                        Cpf = update.Cpf,
                                        Email = update.Email,
                                        CustomerType = (int)Enums.CustomerType.Customer,
                                        Created = DateTime.Now,
                                        Modified = DateTime.Now,
                                        Status = (int)Enums.CustomerStatus.Validation,
                                        Code = Helper.SecurityHelper.HMACSHA1(update.Email, update.Email + "|" + update.Cpf),
                                        IdOperation = operation.Id
                                    };

                                    db.Customer.Add(customer);
                                    db.SaveChanges();

                                    update.IdCustomer = customer.Id;
                                    db.SaveChanges();
                                }
                            }

                            db.SaveChanges();
                            error = null;
                            ret = true;
                        }
                        else
                            error = "O status desse cliente não pode ser alterado, pois ele já foi aprovado!";
                    }
                    else
                        error = "Cliente não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.UpdateCustomerStatus", ex.Message, $"idCustomer: {idCustomer}, status:{status}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o cliente. (erro:" + idLog + ")";
            }
            return ret;
        }

        public List<OperationPartner> ListActiveByOperation(Guid operationCode, out string error)
        {
            List<OperationPartner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.OperationPartner.Where(p => !p.Deleted && p.Operation.Code == operationCode && p.Active).OrderBy(o => o.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ListActiveByOperation", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os parceiros ativos de uma operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Dictionary<string, string> ListDestinataries(int idOperationPartner, out string error)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var operation = db.Operation.Single(o => o.OperationPartners.Any(p => p.Id == idOperationPartner));
                    var list = db.AdminUser.Where(a => a.IdOperationPartner == idOperationPartner);
                    foreach (var user in list)
                        ret.Add(user.Email, user.Name);
                    if(ret.Count == 0)
                    {
                        list = db.AdminUser.Where(a => a.OperationPartner.IdOperation == idOperationPartner);
                        foreach (var user in list)
                            ret.Add(user.Email, user.Name);
                    }
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ListDestinataries", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os parceiros ativos de uma operação. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
