using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class OperationRepository : IOperationRepository
    {
        private string _connectionString;
        public OperationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddAddress(int idOperation, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(!db.OperationAddress.Any(o => o.IdOperation == idOperation && o.IdAddress == idAddress))
                    {
                        db.OperationAddress.Add(new OperationAddress() { IdAddress = idAddress, IdOperation = idOperation });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.AddAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool AddContact(int idOperation, int idContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.OperationContact.Any(o => o.IdOperation == idOperation && o.IdContact == idContact))
                    {
                        db.OperationContact.Add(new OperationContact() { IdContact = idContact, IdOperation = idOperation });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(Operation operation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    operation.Modified = operation.Created = DateTime.UtcNow;
                    db.Operation.Add(operation);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a opreação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int idOperation, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.OperationAddress.SingleOrDefault(o => o.IdOperation == idOperation && o.IdAddress == idAddress);
                    db.OperationAddress.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteContact(int idOperation, int idContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.OperationContact.SingleOrDefault(o => o.IdOperation == idOperation && o.IdContact == idContact);
                    db.OperationContact.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Operation> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Operation> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Operation.Where(o => string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word));
                    switch (sort)
                    {
                        case "Domain ASC":
                            tmpList = tmpList.OrderBy(f => f.Domain);
                            break;
                        case "Domain DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Domain);
                            break;
                        case "Id ASC":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "Id DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "Title ASC":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "Title DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "CompanyName ASC":
                            tmpList = tmpList.OrderBy(f => f.CompanyName);
                            break;
                        case "CompanyName DESC":
                            tmpList = tmpList.OrderByDescending(f => f.CompanyName);
                            break;
                        case "CompanyDoc ASC":
                            tmpList = tmpList.OrderBy(f => f.CompanyDoc);
                            break;
                        case "CompanyDoc DESC":
                            tmpList = tmpList.OrderByDescending(f => f.CompanyDoc);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Operation.Count(o => string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word));

                    ret = new ResultPage<Operation>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as operações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Operation Read(int id, out string error)
        {
            Operation ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Operation.Include("OperationContacts").SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Operation operation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Operation.SingleOrDefault(c => c.Id == operation.Id);
                    if (update != null)
                    {
                        update.Active = operation.Active;
                        update.CashbackPercentage = operation.CashbackPercentage;
                        update.CompanyDoc = operation.CompanyDoc;
                        update.CompanyName = operation.CompanyName;
                        update.Domain = operation.Domain;
                        update.IdOperationType = operation.IdOperationType;
                        update.Image = operation.Image;
                        update.Modified = DateTime.UtcNow;
                        update.Title = operation.Title;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Operação não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
