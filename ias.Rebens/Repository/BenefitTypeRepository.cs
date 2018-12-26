using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class BenefitTypeRepository : IBenefitTypeRepository
    {
        public bool Create(BenefitType benefitType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    benefitType.Modified = benefitType.Created = DateTime.UtcNow;
                    db.BenefitType.Add(benefitType);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("BenefitTypeRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar o tipo de benefício. (erro:" + idLog + ")";
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
                    if (db.Benefit.Any(c => c.IdBenefitType == id))
                    {
                        ret = false;
                        error = "Esse tipo de benefício não pode ser excluido pois possui benefícios associadas a ele.";
                    }
                    else
                    {
                        var type = db.BenefitType.SingleOrDefault(s => s.Id == id);
                        db.BenefitType.Remove(type);
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("BenefitTypeRepository.Delete", ex);
                error = "Ocorreu um erro ao tentar excluir o tipo de benefício. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<BenefitType> List(out string error)
        {
            List<BenefitType> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.BenefitType.ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("BenefitTypeRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar os tipos de benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<BenefitType> ListActive(out string error)
        {
            List<BenefitType> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.BenefitType.Where(t => t.Active).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("BenefitTypeRepository.ListActive", ex);
                error = "Ocorreu um erro ao tentar listar os tipos de benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public BenefitType Read(int id, out string error)
        {
            BenefitType ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.BenefitType.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("BenefitTypeRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler o tipo de benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(BenefitType benefitType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var update = db.BenefitType.SingleOrDefault(c => c.Id == benefitType.Id);
                    if (update != null)
                    {
                        update.Active = benefitType.Active;
                        update.Modified = DateTime.UtcNow;
                        update.Name = benefitType.Name;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "tipo de benefício não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("BenefitTypeRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar o tipo de benefício. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
