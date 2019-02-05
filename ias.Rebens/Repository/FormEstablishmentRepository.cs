using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class FormEstablishmentRepository : IFormEstablishmentRepository
    {
        private string _connectionString;
        public FormEstablishmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(FormEstablishment formEstablishment, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    formEstablishment.Created = DateTime.Now;
                    db.FormEstablishment.Add(formEstablishment);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FormEstablishment.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
