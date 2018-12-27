using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class OperationRepository : IOperationRepository
    {
        public bool Create(Operation operation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    operation.Modified = operation.Created = DateTime.UtcNow;
                    db.Operation.Add(operation);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar a opreação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Operation> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<Operation> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Operation.OrderBy(c => c.Title).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Operation.Count();

                    ret = new ResultPage<Operation>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationRepository.ListPage", ex);
                error = "Ocorreu um erro ao tentar listar as operações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Operation Read(int id, out string error)
        {
            Operation ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.Operation.Include("Contact").Include("Contact.Address").SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler a operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Operation> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<Operation> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Operation.Where(o => o.Title.Contains(word)).OrderBy(o => o.Title).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Operation.Count(o => o.Title.Contains(word));

                    ret = new ResultPage<Operation>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationRepository.SearchPage", ex);
                error = "Ocorreu um erro ao tentar listar as operações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Operation operation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var update = db.Operation.SingleOrDefault(c => c.Id == operation.Id);
                    if (update != null)
                    {
                        update.Active = operation.Active;
                        update.CashbackPercentage = operation.CashbackPercentage;
                        update.CompanyDoc = operation.CompanyDoc;
                        update.CompanyName = operation.CompanyName;
                        update.Domain = operation.Domain;
                        if(operation.IdContact.HasValue)
                            update.IdContact = operation.IdContact;
                        update.IdOperationType = operation.IdOperationType;
                        update.Image = operation.Image;
                        update.Modified = DateTime.UtcNow;
                        update.Title = operation.Title;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Operação não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar a operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
