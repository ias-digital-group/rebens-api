using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class OperationCustomerRepository : IOperationCustomerRepository
    {
        private string _connectionString;
        public OperationCustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public OperationCustomer ReadByCpf(string cpf, out string error)
        {
            OperationCustomer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (cpf.Length == 11)
                        cpf = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9);
                    ret = db.OperationCustomer.SingleOrDefault(o => o.CPF == cpf);
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.CheckUser", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }

            return ret;
        }

        public bool SetSigned(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationCustomer.SingleOrDefault(o => o.Id == id);
                    update.Signed = true;
                    update.Modified = DateTime.Now;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationCustomerRepository.SetSigned", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar. (erro:" + idLog + ")";
                ret = false;
            }

            return ret;
        }
    }
}
