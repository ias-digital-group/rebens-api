using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ias.Rebens
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private string _connectionString;
        public BankAccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(BankAccount account, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    account.Modified = account.Created = DateTime.UtcNow;
                    db.BankAccount.Add(account);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BankAccountRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a conta. (erro:" + idLog + ")";
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
                    var item = db.BankAccount.SingleOrDefault(c => c.Id == id);
                    if (db.Withdraw.Any(w => w.IdBankAccount == item.Id))
                        item.Active = false;
                    else
                        db.BankAccount.Remove(item);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BankAccountRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a conta. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<Bank> ListBanks(out string error)
        {
            List<Bank> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Bank.ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BankAccountRepository.ListBanks", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os bancos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<BankAccount> ListPage(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<BankAccount> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.BankAccount.Include("Bank").Where(b => idCustomer == b.IdCustomer && b.Active && (string.IsNullOrEmpty(word) || b.Type  == word || b.Bank.Name.Contains(word) || b.Branch.Contains(word) || b.AccountNumber.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Bank.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Bank.Name);
                            break;
                        case "branch asc":
                            tmpList = tmpList.OrderBy(f => f.Branch);
                            break;
                        case "branch desc":
                            tmpList = tmpList.OrderByDescending(f => f.Branch);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "account asc":
                            tmpList = tmpList.OrderBy(f => f.AccountNumber);
                            break;
                        case "account desc":
                            tmpList = tmpList.OrderByDescending(f => f.AccountNumber);
                            break;
                        case "type asc":
                            tmpList = tmpList.OrderBy(f => f.Type);
                            break;
                        case "type desc":
                            tmpList = tmpList.OrderByDescending(f => f.Type);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.BankAccount.Count(b => idCustomer == b.IdCustomer && (string.IsNullOrEmpty(word) || b.Type == word || b.Bank.Name.Contains(word) || b.Branch.Contains(word) || b.AccountNumber.Contains(word)));

                    ret = new ResultPage<BankAccount>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BankAccountRepository.ListPage", ex.Message, $"idCustomer:{idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as contas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public BankAccount Read(int id, out string error)
        {
            BankAccount ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BankAccount.Include("Bank").SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BankAccountRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a conta. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(BankAccount account, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.BankAccount.SingleOrDefault(c => c.Id == account.Id);
                    if (update != null)
                    {
                        update.Active = account.Active;
                        update.IdBank = account.IdBank;
                        update.Type = account.Type;
                        update.Branch = account.Branch;
                        update.AccountNumber = account.AccountNumber;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Conta não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BankAccountRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a conta. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
