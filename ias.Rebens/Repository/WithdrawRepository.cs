using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class WithdrawRepository : IWithdrawRepository
    {
        private string _connectionString;
        public WithdrawRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Withdraw withdraw, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    withdraw.Modified = withdraw.Created = DateTime.UtcNow;
                    db.Withdraw.Add(withdraw);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WithdrawRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o resgate. (erro:" + idLog + ")";
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
                    var item = db.Withdraw.SingleOrDefault(c => c.Id == id);
                    db.Withdraw.Remove(item);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WithdrawRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o resgate. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Withdraw> ListPage(int? idCustomer, int page, int pageItems, string sort, out string error)
        {
            ResultPage<Withdraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Withdraw.Include("BankAccount").Include("BankAccount.Bank").Where(w => !idCustomer.HasValue  || (idCustomer.HasValue && w.IdCustomer == idCustomer));
                    switch (sort.ToLower())
                    {
                        case "date asc":
                            tmpList = tmpList.OrderBy(f => f.Date);
                            break;
                        case "date desc":
                            tmpList = tmpList.OrderByDescending(f => f.Date);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "status asc":
                            tmpList = tmpList.OrderBy(f => f.Status);
                            break;
                        case "status desc":
                            tmpList = tmpList.OrderByDescending(f => f.Status);
                            break;
                        case "amount asc":
                            tmpList = tmpList.OrderBy(f => f.Amount);
                            break;
                        case "amount desc":
                            tmpList = tmpList.OrderByDescending(f => f.Amount);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Withdraw.Count(w => !idCustomer.HasValue || (idCustomer.HasValue && w.IdCustomer == idCustomer));

                    ret = new ResultPage<Withdraw>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WithdrawRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os resgates. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Withdraw Read(int id, out string error)
        {
            Withdraw ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Withdraw.SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WithdrawRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o resgate. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Withdraw withdraw, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Withdraw.SingleOrDefault(c => c.Id == withdraw.Id);
                    if (update != null)
                    {
                        update.IdBankAccount = withdraw.IdBankAccount;
                        update.Date = withdraw.Date;
                        update.Amount = withdraw.Amount;
                        update.Status = withdraw.Status;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Resgate não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("WithdrawRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o resgate. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
