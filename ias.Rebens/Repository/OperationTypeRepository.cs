using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class OperationTypeRepository : IOperationTypeRepository
    {
        public bool Create(OperationType operationType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    operationType.Modified = operationType.Created = DateTime.UtcNow;
                    db.OperationType.Add(operationType);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationTypeRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar o tipo de operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    if (db.Operation.Any(c => c.IdOperationType == id))
                    {
                        ret = false;
                        error = "Esse tipo de operação não pode ser excluido pois possui benefícios associadas a ele.";
                    }
                    else
                    {
                        var type = db.OperationType.SingleOrDefault(s => s.Id == id);
                        db.OperationType.Remove(type);
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationTypeRepository.Delete", ex);
                error = "Ocorreu um erro ao tentar excluir o tipo de operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<OperationType> List(out string error)
        {
            List<OperationType> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.OperationType.ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationTypeRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar os tipos de operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<OperationType> ListActive(out string error)
        {
            List<OperationType> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.OperationType.Where(t => t.Active).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationTypeRepository.ListActive", ex);
                error = "Ocorreu um erro ao tentar listar os tipos de operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public OperationType Read(int id, out string error)
        {
            OperationType ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.OperationType.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationTypeRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler o tipo de operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(OperationType operationType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var update = db.OperationType.SingleOrDefault(c => c.Id == operationType.Id);
                    if (update != null)
                    {
                        update.Active = operationType.Active;
                        update.Modified = DateTime.UtcNow;
                        update.Name = operationType.Name;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "tipo de operação não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("OperationTypeRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar o tipo de operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
