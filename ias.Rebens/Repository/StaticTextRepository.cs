using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class StaticTextRepository : IStaticTextRepository
    {
        private string _connectionString;
        public StaticTextRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(StaticText staticText, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    staticText.Modified = staticText.Created = DateTime.UtcNow;
                    db.StaticText.Add(staticText);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.Create", ex.Message, "", ex.StackTrace);
                if(ex.InnerException != null)
                {
                    logError.Create("StaticTextRepository.Create - InnerException", ex.InnerException.Message, "", ex.InnerException.StackTrace);
                }
                error = "Ocorreu um erro ao tentar criar o texto. (erro:" + idLog + ")";
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
                    var st = db.StaticText.SingleOrDefault(c => c.Id == id);
                    db.StaticText.Remove(st);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o texto. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<StaticText> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<StaticText> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.StaticText.Where(s => s.IdOperation == idOperation && (string.IsNullOrEmpty(word) || s.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.StaticText.Count(s => s.IdOperation == idOperation && (string.IsNullOrEmpty(word) || s.Title.Contains(word)));

                    ret = new ResultPage<StaticText>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os textos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Entity.StaticTextListItem> ListPage(int page, int pageItems, string word, string sort, int idStaticTextType, out string error, int? idOperation = null)
        {
            ResultPage<Entity.StaticTextListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.StaticText.Include("Operation").Where(p => (!idOperation.HasValue || (idOperation.HasValue && p.IdOperation == idOperation)) 
                                    && (string.IsNullOrEmpty(word) || p.Title.Contains(word)) && p.IdStaticTextType == idStaticTextType);
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(s => new Entity.StaticTextListItem() { 
                        Id = s.Id,
                        Title = s.Title,
                        Url = s.Url,
                        Order = s.Order,
                        Html = s.Html,
                        Style =s.Style,
                        IdStaticTextType = s.IdStaticTextType,
                        IdOperation = s.Operation.Id,
                        OperationLogo = s.Operation.Image,
                        OperationName = s.Operation.Title,
                        Active = s.Active,
                        Created = s.Created,
                        Modified = s.Modified,
                        IdBenefit = s.IdBenefit
                    }).ToList();

                    list.ForEach(c =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.StaticText && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        var modifiedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.StaticText && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.update)
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

                    ret = new ResultPage<Entity.StaticTextListItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os textos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticText Read(int id, out string error)
        {
            StaticText ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.Include("Operation").SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o texto. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticText ReadByType(int idOperation, int idType, out string error)
        {
            StaticText ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.Where(t => t.IdOperation == idOperation && t.IdStaticTextType == idType && t.Active).OrderByDescending(t => t.Modified).FirstOrDefault();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o texto. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticText ReadByType(int idType, out string error)
        {
            StaticText ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.Where(t => t.IdStaticTextType == idType && t.Active).OrderByDescending(t => t.Modified).FirstOrDefault();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ReadByType", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o texto. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticText ReadOperationConfiguration(int idOperation, out string error)
        {
            StaticText ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.SingleOrDefault(c => c.IdOperation == idOperation && c.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ReadOperationConfiguration", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a configuração da operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticText ReadText(int idOperation, string page, out string error)
        {
            StaticText ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.SingleOrDefault(c => c.IdOperation == idOperation && c.Url == page && c.IdStaticTextType == (int)Enums.StaticTextType.Pages);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ReadText", ex.Message, $"idOperation:{idOperation}, page:{page}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o texto. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticText ReadText(Guid operationCode, string page, out string error)
        {
            StaticText ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.SingleOrDefault(c => c.Operation.Code == operationCode && c.Url == page && c.IdStaticTextType == (int)Enums.StaticTextType.Pages);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ReadText", ex.Message, $"operationCode:{operationCode}, page:{page}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o texto. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(StaticText staticText, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.StaticText.SingleOrDefault(c => c.Id == staticText.Id);
                    if (update == null && staticText.IdBenefit.HasValue)
                        update = db.StaticText.SingleOrDefault(c => c.IdBenefit == staticText.IdBenefit && c.IdStaticTextType == staticText.IdStaticTextType);
                    if (update == null && staticText.IdOperation.HasValue)
                        update = db.StaticText.SingleOrDefault(c => c.IdOperation == staticText.IdOperation && c.IdStaticTextType == staticText.IdStaticTextType);
                    if(update == null && staticText.IdStaticTextType == (int)Enums.StaticTextType.ScratchcardRegulation)
                        update = db.StaticText.SingleOrDefault(c => c.IdOperation == staticText.IdOperation && c.IdStaticTextType == staticText.IdStaticTextType && c.Url == staticText.Url);

                    if (update != null)
                    {
                        update.Html = staticText.Html;
                        update.Modified = DateTime.UtcNow;

                        if (staticText.IdStaticTextType != (int)Enums.StaticTextType.ScratchcardRegulation)
                        {
                            update.Active = staticText.Active;
                            update.IdStaticTextType = staticText.IdStaticTextType;
                            update.Order = staticText.Order;
                            update.Style = staticText.Style;
                            update.Title = staticText.Title;
                            update.Url = staticText.Url;
                        }

                        if ((staticText.IdStaticTextType == (int)Enums.StaticTextType.CourseRegulation
                            || staticText.IdStaticTextType == (int)Enums.StaticTextType.CourseFAQ)
                            && staticText.IdOperation > 0)
                        {
                            update.IdOperation = staticText.IdOperation;
                        }

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = update.Id,
                            Item = (int)Enums.LogItem.StaticText
                        });

                        db.SaveChanges();
                        error = null;

                        if (update.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration)
                        {
                            var operation = db.Operation.Single(o => o.Id == update.IdOperation);
                            if(operation != null)
                            {
                                if (operation.PublishStatus == (int)Enums.PublishStatus.done)
                                    operation.PublishStatus = (int)Enums.PublishStatus.publish;
                                if (operation.TemporaryPublishStatus == (int)Enums.PublishStatus.done)
                                    operation.TemporaryPublishStatus = (int)Enums.PublishStatus.publish;
                                operation.Modified = DateTime.UtcNow;
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        error = "Texto não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o texto. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool CouponContract(int idOperation, bool enable, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (enable)
                    {
                        
                    }
                    
                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.CouponContract", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar validar o contrato. (erro:" + idLog + ")";
            }
            return ret;
        }

        public List<StaticText> ListByType(int idStaticTextType, out string error, int? idOperation = null)
        {
            List<StaticText> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = (from s in db.StaticText
                           where s.IdStaticTextType == idStaticTextType
                           && (!idOperation.HasValue || s.IdOperation == idOperation.Value)
                           select new StaticText()
                           {
                               Id = s.Id,
                               Title = s.Title
                           }).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextRepository.ListByType", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os textos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}