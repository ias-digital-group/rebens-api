using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class FaqRepository : IFaqRepository
    {
        private string _connectionString;
        public FaqRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Faq faq, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    faq.Modified = faq.Created = DateTime.UtcNow;
                    db.Faq.Add(faq);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a pergunta. (erro:" + idLog + ")";
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
                    var item = db.Faq.SingleOrDefault(c => c.Id == id);
                    db.Faq.Remove(item);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a pergunta. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Faq> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<Faq> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Faq.Where(f => (string.IsNullOrEmpty(word) || f.Question.Contains(word) || f.Answer.Contains(word))
                                    && (!idOperation.HasValue || (idOperation.HasValue && f.IdOperation == idOperation.Value)));
                    switch (sort.ToLower())
                    {
                        case "question asc":
                            tmpList = tmpList.OrderBy(f => f.Question);
                            break;
                        case "question desc":
                            tmpList = tmpList.OrderByDescending(f => f.Question);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "answer asc":
                            tmpList = tmpList.OrderBy(f => f.Answer);
                            break;
                        case "answer desc":
                            tmpList = tmpList.OrderByDescending(f => f.Answer);
                            break;
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Faq.Count(f => (string.IsNullOrEmpty(word) || f.Question.Contains(word) || f.Answer.Contains(word))
                                    && (!idOperation.HasValue || (idOperation.HasValue && f.IdOperation == idOperation.Value)));

                    ret = new ResultPage<Faq>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as perguntas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Faq> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Faq> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Faq.Where(f => f.Active && f.IdOperation == idOperation && (string.IsNullOrEmpty(word) || f.Question.Contains(word) || f.Answer.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "question asc":
                            tmpList = tmpList.OrderBy(f => f.Question);
                            break;
                        case "question desc":
                            tmpList = tmpList.OrderByDescending(f => f.Question);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "answer asc":
                            tmpList = tmpList.OrderBy(f => f.Answer);
                            break;
                        case "answer desc":
                            tmpList = tmpList.OrderByDescending(f => f.Answer);
                            break;
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Faq.Count(f => f.Active && f.IdOperation == idOperation && (string.IsNullOrEmpty(word) || f.Question.Contains(word) || f.Answer.Contains(word)));

                    ret = new ResultPage<Faq>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.ListByOperatiion", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as perguntas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Faq Read(int id, out string error)
        {
            Faq ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Faq.SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.Create", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a pergunta. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Faq faq, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Faq.SingleOrDefault(c => c.Id == faq.Id);
                    if (update != null)
                    {
                        update.IdOperation = faq.IdOperation;
                        update.Question = faq.Question;
                        update.Answer = faq.Answer;
                        update.Order = faq.Order;
                        update.Active = faq.Active;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "pergunta não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a pergunta. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<Faq> ListByOperation(Guid operationCode, out string error)
        {
            List<Faq> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var operation = db.Operation.SingleOrDefault(o => o.Code == operationCode);
                    if (operation != null)
                    {
                        ret = db.Faq.Where(f => f.Active && f.IdOperation == operation.Id).OrderBy(f => f.Order).ToList();
                        error = null;
                    }
                    else
                    {
                        error = "Operação não encontrada!";
                        ret = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FaqRepository.ListByOperatiion", ex.Message, $"operationCode: {operationCode}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as perguntas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
