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

        public bool Create(Partner partner, int idAdminUser, out string error)
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

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = partner.Id,
                        Item = (int)Enums.LogItem.Partner
                    });
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

        public bool Delete(int id, int idAdminUser, out string error)
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

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = partner.Id,
                            Item = (int)Enums.LogItem.Partner
                        });

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

        public ResultPage<Entity.PartnerListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type = null, bool? status = null)
        {
            ResultPage<Entity.PartnerListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Partner.Where(p => !p.Deleted && (string.IsNullOrEmpty(word) || p.Name.Contains(word))
                                        && (!status.HasValue || (status.HasValue && p.Active == status.Value))
                                        && (!type.HasValue || (type.HasValue && p.Type == type.Value)));
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

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(p => new Entity.PartnerListItem()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Logo = p.Logo,
                        Active = p.Active,
                        Created = p.Created,
                        Modified = p.Modified,
                        IdType = p.Type
                    }).ToList();


                    var total = tmpList.Count();

                    list.ForEach(c =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Partner 
                                                                                    && a.IdItem == c.Id 
                                                                                    && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        var modifiedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Partner 
                                                                                    && a.IdItem == c.Id 
                                                                                    && a.Action == (int)Enums.LogAction.update)
                                            .OrderByDescending(a => a.Created).FirstOrDefault();
                        if (createUser != null)
                            c.CreatedUserName = createUser.AdminUser.Name + " " + createUser.AdminUser.Surname;
                        else
                            c.CreatedUserName = " - ";
                        if (modifiedUser != null)
                            c.ModifiedUserName = modifiedUser.AdminUser.Name + " " + modifiedUser.AdminUser.Surname;
                        else
                            c.ModifiedUserName = " - ";
                    });

                    ret = new ResultPage<Entity.PartnerListItem>(list, page, pageItems, total);

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
                    ret = db.Partner.Include("StaticText").Include("MainAddress").Include("MainContact")
                                        .SingleOrDefault(p => !p.Deleted && p.Id == id);
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

        public bool Update(Partner partner, int idAdminUser, out string error)
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
                        update.IdStaticText = partner.IdStaticText;
                        update.Cnpj = partner.Cnpj;
                        update.CompanyName = partner.CompanyName;
                        update.IdMainContact = partner.IdMainContact;
                        update.IdMainAddress = partner.IdMainAddress;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = partner.Id,
                            Item = (int)Enums.LogItem.Partner
                        });

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

        public List<Partner> ListFreeCoursePartners(int idOperation, out string error)
        {
            List<Partner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Partner.Where(p => !p.Deleted && p.Active && p.Type == (int)Enums.LogItem.FreeCourse
                                && p.FreeCourses.Any(f => !f.Deleted && f.Active && f.IdOperation == idOperation)).OrderBy(p => p.Name).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.ListFreeCoursePartners", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os parceiros. (erro:" + idLog + ")";
                ret = null;
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
                    var update = db.Partner.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Partner,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Parceiro não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
