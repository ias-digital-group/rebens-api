using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private string _connectionString;
        public ConfigurationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Configuration configuration, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    configuration.Modified = configuration.Created = DateTime.UtcNow;
                    db.Configuration.Add(configuration);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ConfigurationRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a configuração. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public Configuration Read(int id, out string error)
        {
            Configuration ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Configuration.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ConfigurationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a configuração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Configuration ReadByOperation(int idOperation, out string error)
        {
            Configuration ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Configuration.SingleOrDefault(c => c.IdOperation == idOperation);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ConfigurationRepository.ReadByOperation", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a configuração. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Configuration configuration, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Configuration.SingleOrDefault(c => c.Id == configuration.Id);
                    if (update != null)
                    {
                        update.Config = configuration.Config;
                        update.ConfigurationType = configuration.ConfigurationType;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Configuração não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ConfigurationRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a configuração. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
