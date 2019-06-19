using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class OperationCustomerRepository : IOperationCustomerRepository
    {
        private string _connectionString;
        public OperationCustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public OperationCustomer ReadByCpf(string cpf, int idOperation, out string error)
        {
            OperationCustomer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    cpf = cpf.Replace(".", "").Replace("-", "");
                    string cpfMasked = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9);

                    ret = db.OperationCustomer.SingleOrDefault(o => (o.CPF == cpf || o.CPF == cpfMasked) && o.IdOperation == idOperation);
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.ReadByCpf", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }

            return ret;
        }

        public bool SetSigned(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationCustomer.SingleOrDefault(o => o.Id == id);
                    update.Signed = true;
                    update.Modified = DateTime.Now;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.SetSigned", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar. (erro:" + idLog + ")";
                ret = false;
            }

            return ret;
        }

        public OperationCustomer ReadByEmail(string email, int idOperation, out string error)
        {
            OperationCustomer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.OperationCustomer.SingleOrDefault(o => (o.Email1 == email || o.Email2 == email) && o.IdOperation == idOperation);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.ReadByEmail", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }

            return ret;
        }
        
        public ResultPage<OperationCustomer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<OperationCustomer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OperationCustomer.Where(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                    && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.CPF.Contains(word) || a.Email1.Contains(word) || a.Email1.Contains(word)));
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
                        case "email1 asc":
                            tmpList = tmpList.OrderBy(f => f.Email1);
                            break;
                        case "email1 desc":
                            tmpList = tmpList.OrderByDescending(f => f.Email1);
                            break;
                        case "email2 asc":
                            tmpList = tmpList.OrderBy(f => f.Email2);
                            break;
                        case "email2 desc":
                            tmpList = tmpList.OrderByDescending(f => f.Email2);
                            break;
                        case "phone asc":
                            tmpList = tmpList.OrderBy(f => f.Phone);
                            break;
                        case "phone desc":
                            tmpList = tmpList.OrderByDescending(f => f.Phone);
                            break;
                        case "cellphone asc":
                            tmpList = tmpList.OrderBy(f => f.Cellphone);
                            break;
                        case "cellphone desc":
                            tmpList = tmpList.OrderByDescending(f => f.Cellphone);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.OperationCustomer.Count(a => (!idOperation.HasValue || (idOperation.HasValue && idOperation == a.IdOperation))
                                    && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.CPF.Contains(word) || a.Email1.Contains(word) || a.Email1.Contains(word)));

                    ret = new ResultPage<OperationCustomer>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os clientes. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Create(OperationCustomer operationCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.OperationCustomer.Any(c => c.CPF == operationCustomer.CPF && c.IdOperation == operationCustomer.IdOperation))
                    {
                        operationCustomer.Phone = string.IsNullOrEmpty(operationCustomer.Phone) || operationCustomer.Phone.Trim().ToLower() == "null" ? "" : operationCustomer.Phone;
                        operationCustomer.Cellphone = string.IsNullOrEmpty(operationCustomer.Cellphone) || operationCustomer.Cellphone.Trim().ToLower() == "null" ? "" : operationCustomer.Cellphone;
                        operationCustomer.Email1 = string.IsNullOrEmpty(operationCustomer.Email1) || operationCustomer.Email1.Trim().ToLower() == "null" ? "" : operationCustomer.Email1;
                        operationCustomer.Email2 = string.IsNullOrEmpty(operationCustomer.Email2) || operationCustomer.Email2.Trim().ToLower() == "null" ? "" : operationCustomer.Email2;

                        operationCustomer.Modified = operationCustomer.Created = DateTime.UtcNow;
                        db.OperationCustomer.Add(operationCustomer);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(OperationCustomer operationCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationCustomer.SingleOrDefault(c => c.Id == operationCustomer.Id);
                    if (update != null)
                    {
                        update.Cellphone = operationCustomer.Cellphone;
                        update.CPF = operationCustomer.CPF;
                        update.Email1 = operationCustomer.Email1;
                        update.Email2 = operationCustomer.Email2;
                        update.Modified = DateTime.UtcNow;
                        update.Name = operationCustomer.Name;
                        update.Phone = operationCustomer.Phone;

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
                int idLog = logError.Create("OperationCustomerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o cliente. (erro:" + idLog + ")";
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
                    var customer = db.OperationCustomer.SingleOrDefault(c => c.Id == id);
                    db.OperationCustomer.Remove(customer);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
