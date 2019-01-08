using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class IntegrationTypeRepository : IIntegrationTypeRepository
    {
        private string _connectionString;
        public IntegrationTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(IntegrationType integrationType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    integrationType.Modified = integrationType.Created = DateTime.UtcNow;
                    db.IntegrationType.Add(integrationType);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("IntegrationTypeRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o tipo de integração. (erro:" + idLog + ")";
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
                    if (db.Benefit.Any(c => c.IdIntegrationType == id))
                    {
                        ret = false;
                        error = "Esse tipo de integração não pode ser excluido pois possui benefícios associadas a ele.";
                    }
                    else
                    {
                        var type = db.IntegrationType.SingleOrDefault(s => s.Id == id);
                        db.IntegrationType.Remove(type);
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("IntegrationTypeRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o tipo de integração. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<IntegrationType> List(out string error)
        {
            List<IntegrationType> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.IntegrationType.ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("IntegrationTypeRepository.List", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os tipos de integração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<IntegrationType> ListActive(out string error)
        {
            List<IntegrationType> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.IntegrationType.Where(t => t.Active).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("IntegrationTypeRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os tipos de integração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public IntegrationType Read(int id, out string error)
        {
            IntegrationType ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.IntegrationType.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("IntegrationTypeRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o tipo de integração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(IntegrationType integrationType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.IntegrationType.SingleOrDefault(c => c.Id == integrationType.Id);
                    if (update != null)
                    {
                        update.Active = integrationType.Active;
                        update.Modified = DateTime.UtcNow;
                        update.Name = integrationType.Name;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "tipo de integração não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("IntegrationTypeRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o tipo de integração. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
