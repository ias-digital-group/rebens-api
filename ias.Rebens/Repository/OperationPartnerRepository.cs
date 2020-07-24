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

        public ResultPage<Entity.OperationPartnerListItem> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null, int? idOperation = null)
        {
            ResultPage<Entity.OperationPartnerListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OperationPartner.Where(p => !p.Deleted 
                                                                && (!idOperation.HasValue || p.IdOperation == idOperation.Value)
                                                                && (!status.HasValue || p.Active == status.Value) 
                                                                && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));
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

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(p => new Entity.OperationPartnerListItem() { 
                            Id = p.Id,
                            Name = p.Name,
                            Active = p.Active,
                            IdOperation = p.IdOperation,
                            Doc = p.Doc,
                            Created = p.Created,
                            Modified = p.Modified
                    }).ToList();

                    list.ForEach(c =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Operation && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        var modifiedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Operation && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.update)
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

                    ret = new ResultPage<Entity.OperationPartnerListItem>(list, page, pageItems, total);
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
                        update.Doc = partner.Doc;
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

        public bool ToggleActive(int id, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartner.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.OperationPartner,
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
                int idLog = logError.Create("OperationPartnerRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
