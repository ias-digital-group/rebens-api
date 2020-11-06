using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using ias.Rebens.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices.ComTypes;

namespace ias.Rebens
{
    public class CustomerRepository : ICustomerRepository
    {
        private string _connectionString;
        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Create(Customer customer, out string error)
        {
            return Create(customer, 0, out error);
        }

        public bool Create(Customer customer, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(db.Customer.Any(c => c.IdOperation == customer.IdOperation && (c.Email == customer.Email || (!string.IsNullOrEmpty(customer.Cpf) && c.Cpf == customer.Cpf))))
                    {
                        error = "Este Cpf ou e-mail já está cadastrado na nossa base.";
                        return false;
                    }

                    customer.Modified = customer.Created = DateTime.UtcNow;
                    if(idAdminUser > 0)
                    {
                        customer.Code = "";
                        customer.Status = (int)CustomerStatus.PreSignup;
                        customer.CustomerType = (int)CustomerType.PreSignup;
                    }
                    db.Customer.Add(customer);
                    db.SaveChanges();

                    if(idAdminUser > 0)
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.create,
                            Created = DateTime.UtcNow,
                            Item = (int)LogItem.Customer,
                            IdItem = customer.Id,
                            IdAdminUser = idAdminUser
                        });
                        db.SaveChanges();
                    }

                    error = null;
                }
                SaveLog(customer.Id, CustomerLogAction.signup);
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cliente. (erro:" + idLog + ")";
                if(ex.InnerException != null)
                    logError.Create("CustomerRepository.Create INNER", ex.InnerException.Message, "", ex.InnerException.StackTrace);
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var customer = db.Customer.SingleOrDefault(c => c.Id == id);
                    customer.ComplementaryStatus = (int)Enums.CustomerComplementaryStatus.deleted;
                    customer.Modified = DateTime.UtcNow;

                    if (idAdminUser > 0)
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            Item = (int)LogItem.Customer,
                            IdItem = customer.Id,
                            IdAdminUser = idAdminUser
                        });
                    }
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<Helper.Config.ConfigurationValue> ListConfigurationValues(int idCustomer, out string error)
        {
            List<Helper.Config.ConfigurationValue> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    string tmp = db.Customer.Where(c => c.Id == idCustomer).Select(c => c.Configuration).First();
                    ret = Helper.Config.JsonHelper<List<Helper.Config.ConfigurationValue>>.GetObject(tmp);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ListConfigurationValues", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a configuração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Customer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, 
                                                int? idOperationPartner = null, int? status = null, bool? active = null, int? idPromoter = null)
        {
            ResultPage<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Include("Operation").Include("OperationPartner")
                                    .Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                        && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word) || a.Cpf.Contains(word))
                                        && (!idOperationPartner.HasValue || (a.IdOperationPartner.Value == idOperationPartner.Value))
                                        && (!status.HasValue 
                                            || (status.HasValue && ((status.Value == (int)CustomerStatus.Incomplete && (a.Status == (int)CustomerStatus.ChangePassword)) 
                                            || a.Status == status.Value)))
                                        && (!active.HasValue || (a.Active == active.Value))
                                        && (!idPromoter.HasValue || (idPromoter.HasValue && a.IdPromoter == idPromoter.Value))
                                        && a.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
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

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();

                    ret = new ResultPage<Customer>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ListPage", ex.Message, "", ex.StackTrace);
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
                    ret = db.Customer.Include("Address").Include("Operation").Include("OperationPartner")
                                        .SingleOrDefault(c => c.Id == id && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o cliente. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Customer ReadByEmail(string email, int idOperation, out string error)
        {
            Customer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Customer.SingleOrDefault(c => c.Email == email && c.IdOperation == idOperation && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ReadByEmail", ex.Message, $"email:{email}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public string ReadConfigurationValuesString(int idCustomer, out string error)
        {
            string ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Customer.Where(c => c.Id == idCustomer).Select(c => c.Configuration).First();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ReadConfigurationValues", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a configuração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Customer customer, out string error)
        {
            return Update(customer, 0, out error);
        }

        public bool Update(Customer customer, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (db.Customer.Any(c => c.Id != customer.Id && c.IdOperation == customer.IdOperation && (c.Email == customer.Email || c.Cpf == customer.Cpf)))
                    {
                        error = "Este Cpf ou e-mail já está cadastrado na nossa base.";
                        return false;
                    }

                    var update = db.Customer.SingleOrDefault(c => c.Id == customer.Id);
                    if (update != null)
                    {
                        update.Birthday = customer.Birthday;
                        update.Cellphone = customer.Cellphone;
                        update.Configuration = customer.Configuration;
                        update.Cpf = customer.Cpf;
                        update.Email = customer.Email;
                        update.Gender = customer.Gender;
                        update.Modified = DateTime.UtcNow;
                        update.Name = customer.Name;
                        update.Surname = customer.Surname;
                        update.Phone = customer.Phone;
                        update.RG = customer.RG;
                        if(!string.IsNullOrEmpty(customer.Code))
                            update.Code = customer.Code;
                        update.Active = customer.Active;
                        if(customer.Status != 0)
                            update.Status = customer.Status;
                        update.Picture = customer.Picture;

                        if (customer.IdAddress.HasValue && customer.IdAddress.Value != 0)
                            update.IdAddress = customer.IdAddress.Value;

                        if (idAdminUser > 0)
                        {
                            db.LogAction.Add(new LogAction()
                            {
                                Action = (int)Enums.LogAction.update,
                                Created = DateTime.UtcNow,
                                Item = (int)LogItem.Customer,
                                IdItem = customer.Id,
                                IdAdminUser = idAdminUser
                            });

                            update.IdOperation = customer.IdOperation;
                            update.IdOperationPartner = customer.IdOperationPartner;
                        }

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Cliente não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, int? status, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var user = db.Customer.SingleOrDefault(s => s.Id == id);
                    user.EncryptedPassword = passwordEncrypted;
                    user.PasswordSalt = passwordSalt;
                    if (status.HasValue)
                        user.Status = status.Value;
                    user.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ChangePassword", ex.Message, $"id:{id}", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar alterar a senha do cliente. (erro:{idLog})";
                ret = false;
            }
            return ret;
        }

        public Customer ReadByCode(string code, int idOperation, out string error)
        {
            Customer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Customer.SingleOrDefault(c => c.IdOperation == idOperation && c.Code == code && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ReadByCode", ex.Message, $"code:{code}, idOperation:{idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o cliente. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool ChangeStatus(int id, CustomerStatus status, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var user = db.Customer.SingleOrDefault(s => s.Id == id);
                    user.Status = (int)status;
                    user.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ChangeStatus", ex.Message, $"id:{id}, status:{status.ToString()}", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar alterar o status do cliente. (erro:{idLog})";
                ret = false;
            }
            return ret;
        }

        public bool CheckEmailAndCpf(string email, string cpf, int idOperation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Customer.Any(c => c.IdOperation == idOperation && (c.Email == email || c.Cpf == cpf) && c.Status != (int)CustomerStatus.PreSignup);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.CheckEmailAndCpf", ex.Message, $"email:'{email}', cpf:'{cpf}', idOperation:{idOperation}", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar alterar a senha do cliente. (erro:{idLog})";
                ret = false;
            }
            return ret;
        }

        public List<Customer> ListToGenerateCoupon(int idOperation, int totalItems)
        {
            List<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.Now.Date;
                    ret = db.Customer.Where(c => c.IdOperation == idOperation && !string.IsNullOrEmpty(c.Name)  
                                && !c.Coupons.Any(cp => cp.Created > dt)
                                && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted
                                //&& c.Signatures.Any(s => s.Status.ToUpper() == "ACTIVE")
                                )
                            .Take(totalItems).ToList();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ListToGenerateCoupon", ex.Message, "", ex.StackTrace);
                ret = null;
            }
            return ret;
        }

        public bool HasToGenerateCoupon(int idOperation)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.Now.Date;
                    ret = db.Customer.Any(c => c.IdOperation == idOperation && !string.IsNullOrEmpty(c.Name) && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted && !c.Coupons.Any(cp => cp.Created > dt));
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.HasToGenerateCoupon", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public MoipSignature CheckPlanStatus(int id)
        {
            MoipSignature ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.MoipSignature.Where(s => s.IdCustomer == id).OrderByDescending(s => s.Created).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.CheckPlanStatus", ex.Message, $"id:{id}", ex.StackTrace);
                ret = null;
            }
            return ret;
        }

        public bool SaveSendingblueId(int id, int blueId, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var user = db.Customer.SingleOrDefault(s => s.Id == id);
                    user.SendinblueListId = blueId;
                    user.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ChangeStatus", ex.Message, $"id:{id}, blueId:{blueId}", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar alterar o status do cliente. (erro:{idLog})";
                ret = false;
            }
            return ret;
        }

        public bool ToggleActive(int id, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Customer.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;
                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.AdminUser,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Usuário não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public Customer ReadPreSign(string cpf, int idOperation, out string error)
        {
            Customer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    cpf = cpf.Replace(".", "").Replace("-", "");
                    string cpfMasked = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9);

                    ret = db.Customer.SingleOrDefault(o => (o.Cpf == cpf || o.Cpf == cpfMasked) && o.IdOperation == idOperation && o.Status == (int)Enums.CustomerStatus.PreSignup);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ReadByCpf", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o cliente. (erro:" + idLog + ")";
                ret = null;
            }

            return ret;
        }

        public bool ChangeComplementaryStatus(int id, Enums.CustomerComplementaryStatus status, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var cr = db.Customer.SingleOrDefault(c => c.Id == id);
                    cr.ComplementaryStatus = (int)status;
                    cr.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ChangeComplementaryStatus", ex.Message, $"id:{id}, status:{status.ToString()}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar alterar o status complementar. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
        
        public void SaveLog(int id, CustomerLogAction action, string extra = null)
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.CustomerLog.Add(new CustomerLog()
                    {
                        IdCustomer = id,
                        Action = (int)action,
                        Created = DateTime.UtcNow,
                        Extra = extra
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("CustomerRepository.SaveLog", ex.Message, $"id:{id}, action:{action}", ex.StackTrace);
            }
        }

        public ResultPage<Customer> ListForApprovalPage(int page, int pageItems, string word, out string error, int? idOperation = null,
                                                int? idOperationPartner = null)
        {
            ResultPage<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Include("Operation").Include("OperationPartner")
                                    .Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                        && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word) || a.Cpf.Contains(word))
                                        && (!idOperationPartner.HasValue || (a.IdOperationPartner.Value == idOperationPartner.Value))
                                        && a.Active
                                        && a.CustomerType == (int)CustomerType.Partner
                                        && a.ComplementaryStatus == (int)CustomerComplementaryStatus.waittingApproval).OrderBy(f => f.Name);

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();

                    ret = new ResultPage<Customer>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ListForApprovalPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os clientes. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        #region Referal
        public bool CheckReferalLimit(int idOperation, int idCustomer, out int limit, out string error)
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
                    foreach (var module in config.Modules)
                    {
                        if (module.Name == "customerReferal")
                        {
                            foreach (var field in module.Info.Fields)
                            {
                                if (field.Name == "max")
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
                        var count = db.Customer.Count(c => c.IdCustomerReferer == idCustomer);
                        ret = count < limit;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.CheckReferalLimit", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar verificar o limite. (erro:" + idLog + ")";
                ret = false;
            }

            return ret;
        }

        public ResultPage<Customer> ListReferalByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Where(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)) && a.IdCustomerReferer == idCustomer && a.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
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
                    var total = tmpList.Count();

                    ret = new ResultPage<Customer>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ListReferalByCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as indicações do cliente. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Customer> ListReferalPage(int page, int pageItems, string word, string sort, int? idOperation, out string error)
        {
            ResultPage<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Where(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word))
                                                && (!idOperation.HasValue || (idOperation.HasValue && a.IdOperation == idOperation.Value))
                                                && a.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted
                                                && a.IdCustomerReferer.HasValue);
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
                    var total = tmpList.Count();

                    ret = new ResultPage<Customer>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.ListReferalPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as indicações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool DeleteReferal(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var customer = db.Customer.SingleOrDefault(c => c.Id == id);
                    if(customer.ComplementaryStatus == (int)CustomerComplementaryStatus.pending)
                    {
                        db.Customer.Remove(customer);
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Essa indicação não pode ser apagada.";
                        ret = false;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.DeleteReferal", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
        #endregion Referal

        #region Promoter
        public ResultPage<PromoterReportModel> Report(int page, int pageItems, string word, out string error, int? idOperation = null)
        {
            ResultPage<PromoterReportModel> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.AdminUser.Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                            && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word))
                                            && a.Roles == Enums.Roles.promoter.ToString())
                                        .Select(a => new PromoterReportModel()
                                        {
                                            Id = a.Id,
                                            Name = a.Name,
                                            Surname = a.Surname,
                                            Email = a.Email,
                                            Picture = a.Picture,
                                            Operation = a.Operation.Title
                                        }).OrderBy(a => a.Name).Skip(page * pageItems).Take(pageItems).ToList();

                    list.ForEach(p =>
                    {
                        p.TotalActive = db.Customer.Count(c => c.IdPromoter == p.Id && c.Active && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                        p.TotalInactive = db.Customer.Count(c => c.IdPromoter == p.Id && !c.Active && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                        p.TotalIncomplete = db.Customer.Count(c => c.IdPromoter == p.Id && (c.Status == (int)CustomerStatus.Incomplete || c.Status == (int)CustomerStatus.ChangePassword) && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                        p.TotalValidation = db.Customer.Count(c => c.IdPromoter == p.Id && c.Status == (int)CustomerStatus.Validation && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                        p.TotalComplete = db.Customer.Count(c => c.IdPromoter == p.Id && c.Status == (int)CustomerStatus.Active && c.ComplementaryStatus != (int)Enums.CustomerComplementaryStatus.deleted);
                    });

                    var total = db.AdminUser.Count(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                            && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word))
                                            && a.Roles == Enums.Roles.promoter.ToString());
                    ret = new ResultPage<PromoterReportModel>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.Report", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gerar o relatório os promotores. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
        #endregion Promoter

        public List<Customer> ListForCustomerValidationReminder(int idOperation)
        {
            List<Customer> ret;
            using (var db = new RebensContext(this._connectionString))
            {
                ret = db.Customer.Where(c => c.Status == (int)CustomerStatus.Validation
                                               && !c.CustomerLogs.Any()
                                            //&& !c.CustomerLogs.Any(l => l.Action == (int)CustomerLogAction.validationReminder) 
                                            && (c.ComplementaryStatus == null || (c.ComplementaryStatus == (int)CustomerComplementaryStatus.approved))
                                            //&& c.IdOperation == idOperation
                                        ).ToList();

                //var oldestCustomer = db.Customer.Where(c => c.Status == (int)CustomerStatus.Validation).OrderByDescending(c => c.Created).FirstOrDefault();
                //if (oldestCustomer != null)
                //{
                //    var dt = DateTime.UtcNow.AddDays(-2);
                //    if (dt.Date > oldestCustomer.Created.Date)
                //    {
                //        ret.AddRange(db.Customer.Where(c => c.Status == (int)CustomerStatus.Validation && db.CustomerLog.Any()
                //                        && (c.ComplementaryStatus == null || (c.ComplementaryStatus == (int)CustomerComplementaryStatus.approved))
                //                        && c.IdOperation == idOperation
                //                        && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Year == dt.Year
                //                        && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Month == dt.Month
                //                        && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Day == dt.Day
                //                    ));

                //        dt = DateTime.UtcNow.AddDays(-4);
                //        if (dt.Date > oldestCustomer.Created.Date)
                //        {
                //            ret.AddRange(db.Customer.Where(c => c.Status == (int)CustomerStatus.Validation && db.CustomerLog.Any()
                //                            && (c.ComplementaryStatus == null || (c.ComplementaryStatus == (int)CustomerComplementaryStatus.approved))
                //                            && c.IdOperation == idOperation
                //                            && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Year == dt.Year
                //                            && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Month == dt.Month
                //                            && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Day == dt.Day
                //                        ));

                //            dt = DateTime.UtcNow.AddDays(-6);
                //            if (dt.Date > oldestCustomer.Created.Date)
                //            {
                //                ret.AddRange(db.Customer.Where(c => c.Status == (int)CustomerStatus.Validation && db.CustomerLog.Any()
                //                                && (c.ComplementaryStatus == null || (c.ComplementaryStatus == (int)CustomerComplementaryStatus.approved))
                //                                && c.IdOperation == idOperation
                //                                && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Year == dt.Year
                //                                && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Month == dt.Month
                //                                && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Day == dt.Day
                //                            ));

                //                dt = dt.AddDays(-7);
                //                while (dt.Date >= oldestCustomer.Created.Date)
                //                {
                //                    ret.AddRange(db.Customer.Where(c => c.Status == (int)CustomerStatus.Validation && db.CustomerLog.Any()
                //                                    && (c.ComplementaryStatus == null || (c.ComplementaryStatus == (int)CustomerComplementaryStatus.approved))
                //                                    && c.IdOperation == idOperation
                //                                    && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Year == dt.Year
                //                                    && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Month == dt.Month
                //                                    && c.CustomerLogs.OrderByDescending(l => l.Action == (int)CustomerLogAction.validationReminder).First().Created.Day == dt.Day
                //                                ));

                //                    dt = dt.AddDays(-7);
                //                }
                //            }
                //        }
                //    }
                //}
            }
            
            return ret;
        }
    }
}