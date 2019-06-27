using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using ias.Rebens.Enums;

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
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    customer.Modified = customer.Created = DateTime.UtcNow;
                    db.Customer.Add(customer);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cliente. (erro:" + idLog + ")";
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
                    var customer = db.Customer.SingleOrDefault(c => c.Id == id);
                    db.Customer.Remove(customer);
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
                    ret = Helper.Config.ConfigurationHelper.GetConfigurationValues(tmp);
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

        public ResultPage<Customer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<Customer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Customer.Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation)) 
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

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Customer.Count(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation)) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word)));

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
                    ret = db.Customer.Include("Address").SingleOrDefault(c => c.Id == id);
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
                    ret = db.Customer.SingleOrDefault(c => c.Email == email && c.IdOperation == idOperation);
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
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Customer.SingleOrDefault(c => c.Id == customer.Id);
                    if (update != null)
                    {
                        update.Birthday = customer.Birthday;
                        update.Cellphone = customer.Cellphone;
                        update.Configuration = customer.Configuration;
                        update.Cpf = customer.Cpf;
                        update.CustomerType = customer.CustomerType;
                        update.Email = customer.Email;
                        update.Gender = customer.Gender;
                        update.Modified = DateTime.UtcNow;
                        update.Name = customer.Name;
                        update.Surname = customer.Surname;
                        update.Phone = customer.Phone;
                        update.RG = customer.RG;
                        update.Status = customer.Status;
                        update.Picture = customer.Picture;

                        if (customer.IdAddress.HasValue)
                            update.IdAddress = customer.IdAddress.Value;

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
                    ret = db.Customer.SingleOrDefault(c => c.IdOperation == idOperation && c.Code == code);
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
                    ret = db.Customer.Any(c => c.IdOperation == idOperation && (c.Email == email || c.Cpf == cpf));
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
                    ret = db.Customer.Where(c => c.IdOperation == idOperation && !string.IsNullOrEmpty(c.Name)  && !c.Coupons.Any(cp => cp.Created > dt)).Take(totalItems).ToList();
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
                    ret = db.Customer.Any(c => c.IdOperation == idOperation && !string.IsNullOrEmpty(c.Name) && !c.Coupons.Any(cp => cp.Created > dt));
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.HasToGenerateCoupon", ex.Message, "", ex.StackTrace);
            }
            return ret;
        }

        public bool CheckPlanStatus(int id)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.MoipSignature.Any(s => s.IdCustomer == id && s.Status.ToUpper() == "ACTIVE");
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.CheckPlanStatus", ex.Message, $"id:{id}", ex.StackTrace);
            }
            return ret;
        }
    }
}