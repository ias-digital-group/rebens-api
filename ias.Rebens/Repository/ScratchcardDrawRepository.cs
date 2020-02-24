using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class ScratchcardDrawRepository : IScratchcardDrawRepository
    {
        private string _connectionString;
        public ScratchcardDrawRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public ResultPage<ScratchcardDraw> ListByCustomer(int idCustomer, int page, int pageItems, out string error)
        {
            ResultPage<ScratchcardDraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ScratchcardDraw.Where(s => s.IdCustomer == idCustomer).OrderByDescending(s => s.Created);
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<ScratchcardDraw>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.ListByCustomer", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<ScratchcardDraw> ListByScratchcard(int idScratchcard, int page, int pageItems, out string error)
        {
            ResultPage<ScratchcardDraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ScratchcardDraw.Where(s => s.IdScratchcard == idScratchcard).OrderBy(s => s.Created);
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<ScratchcardDraw>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.ListByScratchcard", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ScratchcardDraw LoadRandom(int idScratchcard, out string error)
        {
            ScratchcardDraw ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ScratchcardDraw.Where(s => s.IdScratchcard == idScratchcard && !s.IdCustomer.HasValue).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.LoadRandom", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ScratchcardDraw Read(int id, out string error)
        {
            ScratchcardDraw ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ScratchcardDraw.SingleOrDefault(s => s.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
