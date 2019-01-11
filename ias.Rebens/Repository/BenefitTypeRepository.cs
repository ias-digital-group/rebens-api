﻿using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class BenefitTypeRepository : IBenefitTypeRepository
    {
        private string _connectionString;
        public BenefitTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(BenefitType benefitType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    benefitType.Modified = benefitType.Created = DateTime.UtcNow;
                    db.BenefitType.Add(benefitType);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitTypeRepository.Create", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
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
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitTypeRepository.Delete", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitType.ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitTypeRepository.List", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitType.Where(t => t.Active).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitTypeRepository.ListActive", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitType.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitTypeRepository.Read", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
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
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitTypeRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o tipo de benefício. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}