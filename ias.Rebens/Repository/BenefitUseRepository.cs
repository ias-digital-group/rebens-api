using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class BenefitUseRepository : IBenefitUseRepository
    {
        private string _connectionString;
        public BenefitUseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(BenefitUse benefitUse, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    while (true)
                    {
                        benefitUse.Code = Helper.SecurityHelper.GenerateCode(12);
                        if (db.BenefitUse.Any(b => b.Code == benefitUse.Code))
                            benefitUse.Code = Helper.SecurityHelper.GenerateCode(12);
                        else
                            break;
                    }

                    benefitUse.Modified = benefitUse.Created = DateTime.UtcNow;
                    db.BenefitUse.Add(benefitUse);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar um uso. (erro:" + idLog + ")";
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
                    var cr = db.BenefitUse.SingleOrDefault(c => c.Id == id);
                    db.BenefitUse.Remove(cr);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o uso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<BenefitUse> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<BenefitUse> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.BenefitUse.Where(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word)) && a.IdCustomer == idCustomer);
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "date asc":
                            tmpList = tmpList.OrderBy(f => f.Created);
                            break;
                        case "date desc":
                            tmpList = tmpList.OrderByDescending(f => f.Created);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.BenefitUse.Count(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word)) && a.IdCustomer == idCustomer);

                    ret = new ResultPage<BenefitUse>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.ListByCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os usos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<BenefitUse> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<BenefitUse> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.BenefitUse.Where(a => string.IsNullOrEmpty(word) || a.Name.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.BenefitUse.Count(a => string.IsNullOrEmpty(word) || a.Name.Contains(word));

                    ret = new ResultPage<BenefitUse>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os usos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public BenefitUse Read(int id, out string error)
        {
            BenefitUse ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitUse.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o uso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public BenefitUse ReadByCode(string code, out string error)
        {
            BenefitUse ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitUse.SingleOrDefault(c => c.Code == code);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.ReadByCode", ex.Message, "{code: "+code+"}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o uso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(BenefitUse benefitUse, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.BenefitUse.SingleOrDefault(c => c.Id == benefitUse.Id);
                    if (update != null)
                    {
                        update.Amount = benefitUse.Amount;
                        update.Comission = benefitUse.Comission;
                        update.IdBenefit = benefitUse.IdBenefit;
                        update.IdBenefitType = benefitUse.IdBenefitType;
                        update.IdCustomer = benefitUse.IdCustomer;
                        update.Modified = DateTime.Now;
                        update.Name = benefitUse.Name;
                        update.Status = benefitUse.Status;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Uso de Benefícios não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o Uso de Benefícios. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public decimal GetCustomerBalance(int idCustomer, out string error)
        {
            decimal ret = 0;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitUse.Where(b => b.Status == (int)Enums.BenefitUseStatus.CashbackAvailable && b.IdCustomer == idCustomer).Sum(b => (decimal?)b.Comission) ?? (decimal)0;
                    ret -= db.Withdraw.Where(w => w.IdCustomer == idCustomer).Sum(w => (decimal?)w.Amount) ?? (decimal)0;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitUseRepository.GetCustomerBalance", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar buscar o saldo do cliente. (erro:{idLog})";
            }
            return ret;
        }

        public bool GetCustomerWithdrawSummary(int idCustomer, out decimal available, out decimal blocked, out string error)
        {
            bool ret = false;
            blocked = available = 0;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    available = db.BenefitUse.Where(b => b.Status == (int)Enums.BenefitUseStatus.CashbackAvailable && b.IdCustomer == idCustomer).Sum(b => (decimal?)b.Comission) ?? (decimal)0;
                    available -= db.Withdraw.Where(w => w.IdCustomer == idCustomer).Sum(w => (decimal?)w.Amount) ?? (decimal)0;
                    blocked = db.BenefitUse.Where(b => b.Status == (int)Enums.BenefitUseStatus.ProcessingCashback && b.IdCustomer == idCustomer).Sum(b => (decimal?)b.Comission) ?? (decimal)0;
                    error = null;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CustomerRepository.GetCustomerWithdrawSummary", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = $"Ocorreu um erro ao tentar buscar o saldo do cliente. (erro:{idLog})";
            }
            return ret;
        }
    }
}
