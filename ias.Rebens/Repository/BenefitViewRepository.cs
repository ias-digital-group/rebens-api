using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class BenefitViewRepository : IBenefitViewRepository
    {
        private string _connectionString;
        public BenefitViewRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool SaveView(int idBenefit, int idCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.BenefitView.Add(new BenefitView() { IdBenefit = idBenefit, IdCustomer = idCustomer, Created = DateTime.Now });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitViewRepository.SaveView", ex.Message, $"idBeneficio: {idBenefit}, idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gravar a visualização do benefício. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
