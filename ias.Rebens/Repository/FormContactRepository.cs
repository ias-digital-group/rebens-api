using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class FormContactRepository : IFormContactRepository
    {
        private string _connectionString;
        public FormContactRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(FormContact formContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    formContact.Created = DateTime.Now;
                    db.FormContact.Add(formContact);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FormContact.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
