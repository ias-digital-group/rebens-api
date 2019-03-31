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

        public ResultPage<StaticText> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<StaticText> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.StaticText.Where(p => (!idOperation.HasValue || (idOperation.HasValue && p.IdOperation == idOperation)) 
                                    && (string.IsNullOrEmpty(word) || p.Title.Contains(word)) && p.IdStaticTextType == (int)Enums.StaticTextType.Pages);
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
                    var total = db.StaticText.Count(s => (!idOperation.HasValue || (idOperation.HasValue && s.IdOperation == idOperation)) 
                                    && (string.IsNullOrEmpty(word) || s.Title.Contains(word)) && s.IdStaticTextType == (int)Enums.StaticTextType.Pages);

                    ret = new ResultPage<StaticText>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("PartnerRepository.ListPage", ex.Message, "", ex.StackTrace);
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
                    ret = db.StaticText.SingleOrDefault(c => c.Id == id);
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

        public bool Update(StaticText staticText, out string error)
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

                    if (update != null)
                    {
                        update.Active = staticText.Active;
                        update.Html = staticText.Html;
                        update.IdStaticTextType = staticText.IdStaticTextType;
                        update.Order = staticText.Order;
                        update.Style = staticText.Style;
                        update.Title = staticText.Title;
                        update.Url = staticText.Url;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
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
    }
}