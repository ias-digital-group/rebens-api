using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ias.Rebens
{
    public class PartnerRepository : IPartnerRepository
    {
        private string _connectionString;
        public PartnerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddAddress(int idPartner, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.PartnerAddress.Any(o => o.IdPartner == idPartner && o.IdAddress == idAddress))
                    {
                        db.PartnerAddress.Add(new PartnerAddress() { IdAddress = idAddress, IdPartner = idPartner });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.AddAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool AddContact(int idPartner, int idContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.PartnerContact.Any(o => o.IdPartner == idPartner && o.IdContact == idContact))
                    {
                        db.PartnerContact.Add(new PartnerContact() { IdContact = idContact, IdPartner = idPartner });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.AddContact", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(Partner partner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    partner.Modified = partner.Created = DateTime.UtcNow;
                    partner.Deleted = false;
                    db.Partner.Add(partner);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.Create", ex.Message, "", ex.StackTrace);
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
                    if (db.Benefit.Any(c => c.IdPartner == id))
                    {
                        ret = false;
                        error = "Esse Parceiro não pode ser excluido pois possui Benefícios associados a ele.";
                    }
                    else
                    {
                        var partner = db.Partner.SingleOrDefault(c => c.Id == id);
                        partner.Deleted = true;
                        partner.Modified = DateTime.UtcNow;

                        db.Partner.Remove(partner);

                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int idPartner, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.PartnerAddress.SingleOrDefault(o => o.IdPartner == idPartner && o.IdAddress == idAddress);
                    db.PartnerAddress.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.DeleteAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteContact(int idPartner, int idContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.PartnerContact.SingleOrDefault(o => o.IdPartner == idPartner && o.IdContact == idContact);
                    var contact = db.Contact.SingleOrDefault(c => c.Id == idContact);
                    db.Contact.Remove(contact);
                    db.PartnerContact.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.DeleteContact", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Partner> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Partner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Partner.Where(p => !p.Deleted && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));
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
                    var total = db.Partner.Count(p => !p.Deleted && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));

                    ret = new ResultPage<Partner>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os parceiros. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Partner Read(int id, out string error)
        {
            Partner ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Partner.Include("StaticText").SingleOrDefault(p => !p.Deleted && p.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o parceiros. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Partner partner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Partner.SingleOrDefault(c => c.Id == partner.Id);
                    if (update != null)
                    {
                        update.Active = partner.Active;
                        update.Name = partner.Name;
                        update.Logo = partner.Logo;
                        update.Modified = DateTime.UtcNow;
                        partner.IdStaticText = update.IdStaticText;

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
                int idLog = logError.Create("PartnerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool SetTextId(int id, int idText, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Partner.SingleOrDefault(c => c.Id == id);
                    if (update != null)
                    {
                        update.Modified = DateTime.UtcNow;
                        update.IdStaticText = idText;

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
                int idLog = logError.Create("PartnerRepository.SetTextId", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
