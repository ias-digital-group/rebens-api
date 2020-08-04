using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class ModuleRepository : IModuleRepository
    {
        private string _connectionString;
        public ModuleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public List<Module> List(out string error)
        {
            List<Module> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Module.OrderBy(m => m.Name).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ModuleRepository.List", ex.Message, null, ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os módulos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Module> ListActive(out string error)
        {
            List<Module> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Module.Where(m => m.Active).OrderBy(m => m.Name).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ModuleRepository.ListActive", ex.Message, null, ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os módulos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
