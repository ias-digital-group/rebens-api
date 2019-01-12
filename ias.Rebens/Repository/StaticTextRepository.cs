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

        public List<StaticText> ListByOperation(int idOperation, out string error)
        {
            List<StaticText> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticText.Where(a => a.IdOperation == idOperation).OrderBy(a => a.Title).ToList();
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

        public ResultPage<StaticText> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<StaticText> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.StaticText.Where(p => string.IsNullOrEmpty(word) || p.Title.Contains(word));
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
                    var total = db.StaticText.Count(o => string.IsNullOrEmpty(word) || o.Title.Contains(word));

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
                    ret = db.StaticText.Include("StaticTextType").SingleOrDefault(c => c.Id == id);
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
                    ret = db.StaticText.Include("StaticTextType").Where(t => t.IdOperation == idOperation && t.IdStaticTextType == idType && t.Active).OrderByDescending(t => t.Modified).FirstOrDefault();
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

        public bool Update(StaticText staticText, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.StaticText.SingleOrDefault(c => c.Id == staticText.Id);
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