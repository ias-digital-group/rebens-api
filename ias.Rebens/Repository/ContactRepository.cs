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

        public bool Create(Contact contact, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    contact.Modified = contact.Created = DateTime.UtcNow;
                    contact.Deleted = false;
                    db.Contact.Add(contact);
                    db.SaveChanges();

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = contact.Id,
                        Item = (int)Enums.LogItem.Contact
                    });
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

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var contact = db.Contact.SingleOrDefault(c => c.Id == id);
                    if (contact != null)
                    {
                        contact.Deleted = true;
                        contact.Modified = DateTime.UtcNow;
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = contact.Id,
                            Item = (int)Enums.LogItem.Contact
                        });
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Contato não encontrado!";
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
                    var tmpList = db.Contact.Include("Address").Where(c => c.Type == (int)Enums.LogItem.Operation && c.IdItem == idOperation 
                                        && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)));
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
                    var total = tmpList.Count();

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
                    var tmpList = db.Contact.Include("Address").Where(c => c.Type == (int)Enums.LogItem.Partner && c.IdItem == idPartner 
                                    && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)));
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
                    var total = tmpList.Count();

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

        public ResultPage<Entity.ContactListItem> ListPage(int page, int pageItems, string word, string sort, out string error, 
                                                int? type = null, int? idItem = null, bool? active = null, int? idOperation = null)
        {
            ResultPage<Entity.ContactListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Contact.Include("Address")
                        .Where(c => !c.Deleted && c.Type.HasValue && c.IdItem.HasValue
                            && (string.IsNullOrEmpty(word) || c.Email.Contains(word) || c.Name.Contains(word) || c.JobTitle.Contains(word)) 
                            && (!type.HasValue || type == c.Type)
                            && (!active.HasValue || active == c.Active)
                            && (!idItem.HasValue || idItem == c.IdItem)
                            && (!idOperation.HasValue || (c.Type == (int)Enums.LogItem.Operation && c.IdItem == idOperation.Value))
                        );
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

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(c => new Entity.ContactListItem() { 
                        Active = c.Active,
                        Address = c.IdAddress.HasValue ? c.Address.Name : "",
                        Cellphone = c.CellPhone,
                        ComercialPhone = c.ComercialPhone,
                        ComercialPhoneBranch = c.ComercialPhoneBranch,
                        Email = c.Email,
                        Id = c.Id,
                        JobTitle = c.JobTitle,
                        Name = c.Name,
                        Phone = c.Phone,
                        RelationshipName = "",
                        Surname = c.Surname,
                        IdItem = c.IdItem,
                        Type = c.Type
                    }).ToList();   

                    list.ForEach(c =>
                    {
                        if (c.Type.HasValue)
                        {
                            switch ((Enums.LogItem)c.Type.Value)
                            {
                                case Enums.LogItem.Company:
                                    var company = db.Company.SingleOrDefault(r => r.Id == c.IdItem);
                                    c.RelationshipName = company != null ? company.Name : "";
                                    break;
                                case Enums.LogItem.Operation:
                                    var operation = db.Operation.SingleOrDefault(r => r.Id == c.IdItem);
                                    c.RelationshipName = operation != null ? operation.Title : "";
                                    break;
                                case Enums.LogItem.Partner:
                                    var partner = db.Partner.SingleOrDefault(r => r.Id == c.IdItem);
                                    c.RelationshipName = partner != null ? partner.Name : "";
                                    break;
                            }
                        } 
                        else
                        {
                            var partner = db.Partner.SingleOrDefault(p => p.IdMainContact == c.Id);
                            if(partner != null)
                                c.RelationshipName = partner.Name;
                            else
                            {
                                var company = db.Company.SingleOrDefault(p => p.IdContact == c.Id);
                                if (company != null)
                                    c.RelationshipName = company.Name;
                                else
                                {
                                    var operation = db.Operation.SingleOrDefault(p => p.IdMainContact == c.Id);
                                    if (operation != null)
                                        c.RelationshipName = operation.Title;
                                }
                            }
                        }
                    });

                    ret = new ResultPage<Entity.ContactListItem>(list, page, pageItems, tmpList.Count());

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
                    ret = db.Contact.SingleOrDefault(c => c.Id == id);
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

        public bool Update(Contact contact, int idAdminUser, out string error)
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
                        update.Surname = contact.Surname;
                        update.ComercialPhone = contact.ComercialPhone;
                        update.ComercialPhoneBranch = contact.ComercialPhoneBranch;
                        update.Active = contact.Active;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = contact.Id,
                            Item = (int)Enums.LogItem.Contact
                        });

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

        public bool ToggleActive(int id, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Contact.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Contact,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Contato não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
