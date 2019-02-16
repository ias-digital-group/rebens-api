using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class ZanoxSaleRepository : IZanoxSaleRepository
    {
        private string _connectionString;
        public ZanoxSaleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(ZanoxSale zanoxSale, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    zanoxSale.Modified = zanoxSale.Created = DateTime.UtcNow;
                    db.ZanoxSale.Add(zanoxSale);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxSaleRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar um pedidos zanox. (erro:" + idLog + ")";
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
                    var cr = db.ZanoxSale.SingleOrDefault(c => c.Id == id);
                    db.ZanoxSale.Remove(cr);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxSaleRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o pedido. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<ZanoxSale> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<ZanoxSale> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ZanoxSale.Where(a => (string.IsNullOrEmpty(word) || a.ZanoxId.Contains(word) || a.ReviewState.Contains(word)) && a.IdCustomer == idCustomer);
                    switch (sort.ToLower())
                    {
                        case "reviewstate asc":
                            tmpList = tmpList.OrderBy(f => f.ReviewState);
                            break;
                        case "reviewstate desc":
                            tmpList = tmpList.OrderByDescending(f => f.ReviewState);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "zpar asc":
                            tmpList = tmpList.OrderBy(f => f.Zpar);
                            break;
                        case "zpar desc":
                            tmpList = tmpList.OrderByDescending(f => f.Zpar);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.ZanoxSale.Count(a => (string.IsNullOrEmpty(word) || a.ZanoxId.Contains(word) || a.ReviewState.Contains(word)) && a.IdCustomer == idCustomer);

                    ret = new ResultPage<ZanoxSale>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxSaleRepository.ListByCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os pedidos Zanox. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<ZanoxSale> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<ZanoxSale> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ZanoxSale.Where(a => string.IsNullOrEmpty(word) || a.ZanoxId.Contains(word) || a.ReviewState.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "reviewstate asc":
                            tmpList = tmpList.OrderBy(f => f.ReviewState);
                            break;
                        case "reviewstate desc":
                            tmpList = tmpList.OrderByDescending(f => f.ReviewState);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "zpar asc":
                            tmpList = tmpList.OrderBy(f => f.Zpar);
                            break;
                        case "zpar desc":
                            tmpList = tmpList.OrderByDescending(f => f.Zpar);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.ZanoxSale.Count(a => string.IsNullOrEmpty(word) || a.ZanoxId.Contains(word) || a.ReviewState.Contains(word));

                    ret = new ResultPage<ZanoxSale>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os pedidos Zanox. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ZanoxSale Read(int id, out string error)
        {
            ZanoxSale ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ZanoxSale.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxSaleRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o pedido. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(ZanoxSale zanoxSale, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ZanoxSale.SingleOrDefault(c => c.Id == zanoxSale.Id);
                    if (update != null)
                    {
                        update.ZanoxId = zanoxSale.ZanoxId;
                        update.ReviewState = zanoxSale.ReviewState;
                        update.TrackingDate = zanoxSale.TrackingDate;
                        update.ModifiedDate = zanoxSale.ModifiedDate;
                        update.ClickDate = zanoxSale.ClickDate;
                        update.ClickId = zanoxSale.ClickId;
                        update.ClickInId = zanoxSale.ClickInId;
                        update.Amount = zanoxSale.Amount;
                        update.Commission = zanoxSale.Commission;
                        update.Currency = zanoxSale.Currency;
                        update.AdspaceId = zanoxSale.AdspaceId;
                        update.AdspaceValue = zanoxSale.AdspaceValue;
                        update.AdmediumId = zanoxSale.AdmediumId;
                        update.AdmediumValue = zanoxSale.AdmediumValue;
                        update.ProgramId = zanoxSale.ProgramId;
                        update.ProgramValue = zanoxSale.ProgramValue;
                        update.ReviewNote = zanoxSale.ReviewNote;
                        update.Gpps = zanoxSale.Gpps;
                        update.Zpar = zanoxSale.Zpar;
                        update.Status = zanoxSale.Status;
                        update.Modified = DateTime.Now;
                        update.IdCustomer = zanoxSale.IdCustomer;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Pedido não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxSaleRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o pedido. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
