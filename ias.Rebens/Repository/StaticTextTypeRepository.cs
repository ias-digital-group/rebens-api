using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class StaticTextTypeRepository : IStaticTextTypeRepository
    {
        private string _connectionString;
        public StaticTextTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(StaticTextType staticTextType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    staticTextType.Modified = staticTextType.Created = DateTime.UtcNow;
                    db.StaticTextType.Add(staticTextType);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextTypeRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o tipo de texto. (erro:" + idLog + ")";
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
                    if (db.StaticText.Any(c => c.IdStaticTextType == id))
                    {
                        ret = false;
                        error = "Esse Tipo de Texto não pode ser excluido pois possui Textos associadas a ele.";
                    }
                    else
                    {
                        var type = db.StaticTextType.SingleOrDefault(s => s.Id == id);
                        db.StaticTextType.Remove(type);
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextTypeRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o tipo de texto. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<StaticTextType> List(out string error)
        {
            List<StaticTextType> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticTextType.ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextTypeRepository.List", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os tipos de textos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<StaticTextType> ListActive(out string error)
        {
            List<StaticTextType> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticTextType.Where(t => t.Active).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextTypeRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os tipos de textos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public StaticTextType Read(int id, out string error)
        {
            StaticTextType ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.StaticTextType.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextTypeRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o tipo de texto. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(StaticTextType staticTextType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.StaticTextType.SingleOrDefault(c => c.Id == staticTextType.Id);
                    if (update != null)
                    {
                        update.Active = staticTextType.Active;
                        update.Modified = DateTime.UtcNow;
                        update.Name = staticTextType.Name;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Tipo de texto não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("StaticTextTypeRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o tipo de texto. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
