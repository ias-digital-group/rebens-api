using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class ContactRepository
    {
        public bool Create(Contact contact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    contact.Modified = contact.Created = DateTime.UtcNow;
                    db.Contact.Add(contact);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ContactRepository.Create", ex);
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
                using (var db = new RebensContext())
                {
                    if (db.Operation.Any(c => c.IdContact == id))
                    {
                        ret = false;
                        error = "Esso contato não pode ser excluida pois está associado a uma operação.";
                    }
                    else if (db.Partner.Any(c => c.IdContact == id))
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
                int idLog = Helper.LogHelper.Add("ContactRepository.Delete", ex);
                error = "Ocorreu um erro ao tentar excluir o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Contact> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<Contact> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Contact.OrderBy(c => c.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Contact.Count();

                    ret = new ResultPage<Contact>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ContactRepository.ListPage", ex);
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
                using (var db = new RebensContext())
                {
                    ret = db.Contact.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ContactRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler o contato. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Contact> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<Contact> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Contact.Where(c => c.Name.Contains(word)).OrderBy(c => c.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Contact.Count(c => c.Name.Contains(word));

                    ret = new ResultPage<Contact>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ContactRepository.SearchPage", ex);
                error = "Ocorreu um erro ao tentar listar os contatos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Contact contact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
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
                int idLog = Helper.LogHelper.Add("ContactRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
