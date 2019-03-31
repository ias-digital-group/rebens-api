using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class ContactRepository : IContactRepository
    {
        private string _connectionString;
        public ContactRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Contact contact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    contact.Modified = contact.Created = DateTime.UtcNow;
                    db.Contact.Add(contact);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o contato. (erro:" + idLog + ")";
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
                    if (db.OperationContact.Any(c => c.IdContact == id))
                    {
                        ret = false;
                        error = "Esso contato não pode ser excluida pois está associado a uma operação.";
                    }
                    else if (db.PartnerContact.Any(c => c.IdContact == id))
                    {
                        ret = false;
                        error = "Esso contato não pode ser excluida pois está associado a um parceiro.";
                    }
                    else
                    {
                        var contact = db.Contact.SingleOrDefault(c => c.Id == id);
                        db.Contact.Remove(contact);
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Contact> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Contact> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Contact.Include("Address").Where(c => c.OperationContacts.Any(oc => oc.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(c => c.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(c => c.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(c => c.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(c => c.Id);
                            break;
                        case "email asc":
                            tmpList = tmpList.OrderBy(c => c.Email);
                            break;
                        case "email desc":
                            tmpList = tmpList.OrderByDescending(c => c.Email);
                            break;
                        case "jobtitle asc":
                            tmpList = tmpList.OrderBy(c => c.JobTitle);
                            break;
                        case "jobtitle desc":
                            tmpList = tmpList.OrderByDescending(c => c.JobTitle);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Contact.Count(c => c.OperationContacts.Any(oc => oc.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)));

                    ret = new ResultPage<Contact>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os contatos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Contact> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Contact> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Contact.Include("Address").Where(c => c.PartnerContacts.Any(pc => pc.IdPartner == idPartner) && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(c => c.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(c => c.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(c => c.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(c => c.Id);
                            break;
                        case "email asc":
                            tmpList = tmpList.OrderBy(c => c.Email);
                            break;
                        case "email desc":
                            tmpList = tmpList.OrderByDescending(c => c.Email);
                            break;
                        case "jobtitle asc":
                            tmpList = tmpList.OrderBy(c => c.JobTitle);
                            break;
                        case "jobtitle desc":
                            tmpList = tmpList.OrderByDescending(c => c.JobTitle);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Contact.Count(c => c.PartnerContacts.Any(pc => pc.IdPartner == idPartner) && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)));

                    ret = new ResultPage<Contact>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.ListByPartner", ex.Message, $"idPartner: {idPartner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os contatos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Contact> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<Contact> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Contact.Include("Address")
                        .Where(c => (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)) 
                        && (!idOperation.HasValue || (idOperation.HasValue && c.OperationContacts.Any(o => o.IdOperation == idOperation.Value))));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(c => c.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(c => c.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(c => c.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(c => c.Id);
                            break;
                        case "email asc":
                            tmpList = tmpList.OrderBy(c => c.Email);
                            break;
                        case "email desc":
                            tmpList = tmpList.OrderByDescending(c => c.Email);
                            break;
                        case "jobtitle asc":
                            tmpList = tmpList.OrderBy(c => c.JobTitle);
                            break;
                        case "jobtitle desc":
                            tmpList = tmpList.OrderByDescending(c => c.JobTitle);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Contact.Count(c => string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)
                                    && (!idOperation.HasValue || c.OperationContacts.Any(o => o.IdOperation == idOperation.Value)));

                    ret = new ResultPage<Contact>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os contatos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Contact Read(int id, out string error)
        {
            Contact ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Contact.Include("Address").SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o contato. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Contact contact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Contact.SingleOrDefault(c => c.Id == contact.Id);
                    if (update != null)
                    {
                        update.CellPhone = contact.CellPhone;
                        update.Email = contact.Email;
                        if(contact.IdAddress.HasValue)
                            update.IdAddress = contact.IdAddress;
                        update.JobTitle = contact.JobTitle;
                        update.Modified = DateTime.UtcNow;
                        update.Name = contact.Name;
                        update.Phone = contact.Phone;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Contato não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ContactRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
