﻿using Microsoft.EntityFrameworkCore;
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

        public ResultPage<Faq> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Faq> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Faq.Where(f => string.IsNullOrEmpty(word) || f.Question.Contains(word) || f.Answer.Contains(word));
                    switch (sort)
                    {
                        case "Question ASC":
                            tmpList = tmpList.OrderBy(f => f.Question);
                            break;
                        case "Question DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Question);
                            break;
                        case "Id ASC":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "Id DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "Answer ASC":
                            tmpList = tmpList.OrderBy(f => f.Answer);
                            break;
                        case "Answer DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Answer);
                            break;
                        case "Order ASC":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "Order DESC":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Faq.Count(f => string.IsNullOrEmpty(word) || f.Question.Contains(word) || f.Answer.Contains(word));

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

        public List<Faq> ListByOperation(int idOperation, out string error)
        {
            List<Faq> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Faq.Where(f => f.IdOperation == idOperation && f.Active).OrderBy(f => f.Order).ToList();
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
    }
}
