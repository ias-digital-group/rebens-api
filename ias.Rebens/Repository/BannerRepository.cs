using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ias.Rebens
{
    public class BannerRepository : IBannerRepository
    {
        private string _connectionString;
        public BannerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddOperation(int idBanner, int idOperation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.BannerOperation.Any(o => o.IdBanner == idBanner && o.IdOperation == idOperation))
                    {
                        db.BannerOperation.Add(new BannerOperation() { IdOperation = idOperation, IdBanner = idBanner });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.AddOperation", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar a operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool ConnectOperations(int id, int[] operations, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpOperations = db.BannerOperation.Where(b => b.IdBanner == id);
                    db.BannerOperation.RemoveRange(tmpOperations);
                    db.SaveChanges();

                    foreach(var op in operations)
                        db.BannerOperation.Add(new BannerOperation() { IdOperation = op, IdBanner = id });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ConnectOperations", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ajustar as conexões das operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(Banner banner, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    banner.Modified = banner.Created = DateTime.UtcNow;
                    banner.Deleted = false;
                    db.Banner.Add(banner);
                    db.SaveChanges();

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = banner.Id,
                        Item = (int)Enums.LogItem.Banner
                    });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o banner. (erro:" + idLog + ")";
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
                    var item = db.Banner.SingleOrDefault(c => c.Id == id);
                    item.Deleted = true;
                    item.Modified = DateTime.UtcNow;
                    
                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.delete,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = id,
                        Item = (int)Enums.LogItem.Banner
                    });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o banner. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteOperation(int idBanner, int idOperation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.BannerOperation.SingleOrDefault(o => o.IdBanner == idBanner && o.IdOperation == idOperation);
                    if (tmp != null)
                    {
                        db.BannerOperation.Remove(tmp);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.DeleteOperation", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a Operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Banner> ListByBenefit(int idBenefit, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Banner.Where(b => !b.Deleted && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted && b.IdBenefit == idBenefit && b.Active && (string.IsNullOrEmpty(word) || b.Name.Contains(word)));
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
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Banner.Count(b => !b.Deleted && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted && b.IdBenefit == idBenefit && b.Active && (string.IsNullOrEmpty(word) || b.Name.Contains(word)));

                    ret = new ResultPage<Banner>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListByBenefit", ex.Message, $"idBenefit: {idBenefit}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Banner> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Banner.Where(b => !b.Deleted && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted)) && 
                                    b.BannerOperations.Any(o => o.IdOperation == idOperation) && b.Active && (string.IsNullOrEmpty(word) || b.Name.Contains(word)));
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
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Banner.Count(b => !b.Deleted && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted && b.BannerOperations.Any(o => o.IdOperation == idOperation) && b.Active && (string.IsNullOrEmpty(word) || b.Name.Contains(word)));

                    ret = new ResultPage<Banner>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Banner> ListByTypeAndOperation(Guid operationCode, int type, int idBannerShow, out string error)
        {
            List<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Banner.Where(b => !b.Deleted && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted)) && 
                                b.BannerOperations.Any(o => o.Operation.Code == operationCode) && b.Type == type &&
                                b.Active && b.Start < DateTime.Now && b.End > DateTime.Now && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && b.Benefit.Active))
                                && ((b.IdBannerShow & idBannerShow) == idBannerShow))
                                .OrderBy(b => b.Order).ToList();
                   
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListByTypeAndOperation", ex.Message, $"operationCode: {operationCode}, type: {type}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Banner> ListByTypeAndOperation(int idOperation, int type, int idBannerShow, out string error)
        {
            List<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Banner.Where(b => !b.Deleted && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted)) &&
                                b.BannerOperations.Any(o => o.IdOperation == idOperation) && b.Type == type &&
                                b.Active && b.Start < DateTime.Now && b.End > DateTime.Now && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && b.Benefit.Active))
                                && ((b.IdBannerShow & idBannerShow) == idBannerShow))
                                .OrderBy(b => b.Order).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListByTypeAndOperation", ex.Message, $"idOperation: {idOperation}, type: {type}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Entity.BannerListItem> ListPage(int page, int pageItems, string word, string sort, out string error, 
                                                            int? idOperation = null, bool? status = null, int? type = null, string where = null)
        {
            ResultPage<Entity.BannerListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Banner.Where(b => !b.Deleted && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted))  
                                    && (string.IsNullOrEmpty(word) || b.Name.Contains(word)) 
                                    && (!idOperation.HasValue || (idOperation.HasValue && b.BannerOperations.Any(o => o.IdOperation == idOperation.Value)))
                                    && (!status.HasValue || (status.HasValue && b.Active == status.Value))
                                    && (!type.HasValue || (type.HasValue && b.Type == type.Value))
                                    && (where == null || (where == "H" && ((b.IdBannerShow & (int)Enums.BannerShow.HomeNotLogged) == (int)Enums.BannerShow.HomeNotLogged))
                                        || (where == "HL" && ((b.IdBannerShow & (int)Enums.BannerShow.HomeLogged) == (int)Enums.BannerShow.HomeLogged))
                                        || (where == "HB" && ((b.IdBannerShow & (int)Enums.BannerShow.Benefit) == (int)Enums.BannerShow.Benefit)))
                                    );
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
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(b => new Entity.BannerListItem() { 
                                    Id = b.Id,
                                    Name = b.Name,
                                    Active = b.Active,
                                    Created = b.Created,
                                    Modified = b.Modified,
                                    IdBannerShow = b.IdBannerShow,
                                    Image = b.Image,
                                    Order = b.Order,
                                    Type = b.Type
                                }).ToList();
                    var total = tmpList.Count();

                    list.ForEach(b =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Banner && a.IdItem == b.Id && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        var modifiedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Banner && a.IdItem == b.Id && a.Action == (int)Enums.LogAction.update)
                                            .OrderByDescending(a => a.Created).FirstOrDefault();
                        var operation = db.Operation.Where(o => o.BannerOperations.Any(bo => bo.IdOperation == o.Id && bo.IdBanner == b.Id)).OrderBy(o => o.Title).FirstOrDefault();
                        if (createUser != null)
                            b.AdminUserCreated = createUser.AdminUser.Name + " " + createUser.AdminUser.Surname;
                        else
                            b.AdminUserCreated = " - ";
                        if (modifiedUser != null)
                            b.AdminUserModified = modifiedUser.AdminUser.Name + " " + modifiedUser.AdminUser.Surname;
                        else
                            b.AdminUserModified = " - ";
                        if (operation != null)
                            b.OperationName = operation.Title;
                        else
                            b.OperationName = " - ";
                    });

                    ret = new ResultPage<Entity.BannerListItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os Banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Banner Read(int id, out string error)
        {
            Banner ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Banner.Include("BannerOperations").SingleOrDefault(b => !b.Deleted 
                                && (!b.IdBenefit.HasValue || (b.IdBenefit.HasValue && !b.Benefit.Deleted && !b.Benefit.Partner.Deleted)) 
                                && b.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o banner. (erro:" + idLog + ")";
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
                    var update = db.Banner.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Banner,
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
                int idLog = logError.Create("BannerRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(Banner banner, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Banner.SingleOrDefault(c => c.Id == banner.Id);
                    if (update != null)
                    {
                        update.Active = banner.Active;
                        update.Target = banner.Target;
                        update.End = banner.End;
                        update.IdBenefit = banner.IdBenefit;
                        update.Image = banner.Image;
                        update.Link = banner.Link;
                        update.Name = banner.Name;
                        update.Order = banner.Order;
                        update.Start = banner.Start;
                        update.Type = banner.Type;
                        update.IdBannerShow = banner.IdBannerShow;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = banner.Id,
                            Item = (int)Enums.LogItem.Banner
                        });

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Banner não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o banner. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
