using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ias.Rebens
{
    public class BenefitRepository : IBenefitRepository
    {
        private string _connectionString;
        public BenefitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddAddress(int idBenefit, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.BenefitAddress.Any(o => o.IdBenefit == idBenefit && o.IdAddress == idAddress))
                    {
                        db.BenefitAddress.Add(new BenefitAddress() { IdAddress = idAddress, IdBenefit = idBenefit });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.AddAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool AddOperation(int idBenefit, int idOperation, int idPostion, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.BenefitOperation.Any(o => o.IdBenefit == idBenefit && o.IdOperation == idOperation))
                    {
                        db.BenefitOperation.Add(new BenefitOperation() { IdOperation = idOperation, IdBenefit = idBenefit, Created = DateTime.UtcNow, Modified = DateTime.UtcNow, IdPosition = idPostion });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.AddOperation", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar a operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(Benefit benefit, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    benefit.Modified = benefit.Created = DateTime.UtcNow;
                    db.Benefit.Add(benefit);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o benefício. (erro:" + idLog + ")";
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
                    var benefit = db.Benefit.SingleOrDefault(c => c.Id == id);
                    db.Benefit.Remove(benefit);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o beneficio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int idBenefit, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.BenefitAddress.SingleOrDefault(o => o.IdBenefit == idBenefit && o.IdAddress == idAddress);
                    db.BenefitAddress.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.DeleteAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteOperation(int idBenefit, int idOperation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.BenefitOperation.SingleOrDefault(o => o.IdBenefit == idBenefit && o.IdOperation == idOperation);
                    db.BenefitOperation.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.DeleteOperation", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a Operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Benefit> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Include("BenefitType").Include("IntegrationType").Where(p => string.IsNullOrEmpty(word) || p.Title.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(o => string.IsNullOrEmpty(word) || o.Title.Contains(word));

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefícios. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Benefit Read(int id, out string error)
        {
            Benefit ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Benefit.Include("BenefitOperations").Include("BenefitAddresses").Include("StaticTexts").Include("Partner").SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Benefit benefit, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Benefit.SingleOrDefault(c => c.Id == benefit.Id);
                    if (update != null)
                    {
                        update.Active = benefit.Active;
                        update.Title = benefit.Title;
                        if (!string.IsNullOrEmpty(benefit.Image))
                            update.Image = benefit.Image;
                        update.DueDate = benefit.DueDate;
                        update.WebSite = benefit.WebSite;
                        update.MaxDiscountPercentageOnline = benefit.MaxDiscountPercentageOnline;
                        update.CpvpercentageOnline = benefit.CpvpercentageOnline;
                        update.MaxDiscountPercentageOffline = benefit.MaxDiscountPercentageOffline;
                        update.CpvpercentageOffline = benefit.CpvpercentageOffline;
                        update.Start = benefit.Start;
                        update.End = benefit.End;
                        update.IdBenefitType = benefit.IdBenefitType;
                        update.Exclusive = benefit.Exclusive;
                        update.IdIntegrationType = benefit.IdIntegrationType;
                        update.IdPartner = benefit.IdPartner;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Benefício não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o benefício. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByAddress(int idAddress, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => b.BenefitAddresses.Any(pa => pa.IdAddress == idAddress) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.BenefitAddresses.Any(pa => pa.IdAddress == idAddress) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByAddress", ex.Message, $"idAddress: {idAddress}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByCategory(int idCategory, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => b.BenefitCategories.Any(bc => bc.IdCategory == idCategory) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.BenefitCategories.Any(bc => bc.IdCategory == idCategory) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByCategory", ex.Message, $"idCategory: {idCategory}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => b.BenefitOperations.Any(bo => bo.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.BenefitOperations.Any(bo => bo.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    list.ForEach(i => { i.StaticTexts.Add(db.StaticText.SingleOrDefault(s => s.IdBenefit == i.Id && s.IdStaticTextType == (int)Enums.StaticTextType.BenefitCall)); });

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByOperation(int idOperation, int? idCategory, string benefitTypes, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    List<int> types = new List<int>();
                    if(!string.IsNullOrEmpty(benefitTypes))
                    {
                        var tmp = benefitTypes.Split(',');
                        foreach (var t in tmp)
                        {
                            if(int.TryParse(t, out int i))
                                types.Add(i);
                        }
                    }
                    var tmpList = db.Benefit.Include("BenefitType").Include("Partner")
                                    .Where(b => b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)
                                    && (string.IsNullOrEmpty(word) || b.Title.Contains(word))
                                    && (string.IsNullOrEmpty(benefitTypes) || types.Contains(b.IdBenefitType))
                                    && b.Active
                                    && (!idCategory.HasValue || (idCategory.HasValue && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory.Value || bc.Category.IdParent == idCategory.Value)))
                                    );

                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.BenefitOperations.Any(bo => bo.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    list.ForEach(i => {
                        var text = db.StaticText.SingleOrDefault(s => s.IdBenefit == i.Id && s.IdStaticTextType == (int)Enums.StaticTextType.BenefitCall);
                        if(text != null && text.Id > 0)
                            i.StaticTexts.Add(text);
                    });

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByType(int idType, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => b.IdBenefitType == idType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.IdBenefitType == idType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByType", ex.Message, $"idType: {idType}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => b.IdPartner == idPartner && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.IdPartner == idPartner && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByPartner", ex.Message, $"idPartner: {idPartner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByIntegrationType(int idIntegrationType, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => b.IdIntegrationType == idIntegrationType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => b.IdIntegrationType == idIntegrationType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListByIntegrationType", ex.Message, $"idIntegrationType: {idIntegrationType}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool AddCategory(int idBenefit, int idCategory, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.BenefitCategory.Any(o => o.IdBenefit == idBenefit && o.IdCategory == idCategory))
                    {
                        db.BenefitCategory.Add(new BenefitCategory() { IdCategory = idCategory, IdBenefit = idBenefit });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.AddCategory", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteCategory(int idBenefit, int idCategory, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.BenefitCategory.SingleOrDefault(o => o.IdBenefit == idBenefit && o.IdCategory == idCategory);
                    db.BenefitCategory.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.DeleteCategory", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<BenefitOperationPosition> ListPositions(out string error)
        {
            List<BenefitOperationPosition> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitOperationPosition.OrderBy(a => a.Id).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListPositions", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as posições. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public void ReadCallAndPartnerLogo(int idBenefit, out string call, out string logo, out string error)
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    call = db.StaticText.SingleOrDefault(s => s.IdBenefit == idBenefit && s.IdStaticTextType == (int)Enums.StaticTextType.BenefitCall).Html;
                    logo = db.Partner.SingleOrDefault(p => p.Benefits.Any(b => b.Id == idBenefit)).Logo;
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ReadCallAndPartnerLogo", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a chamada e o logo do parceiro. (erro:" + idLog + ")";
                logo = call = null;
            }
        }

        public List<Benefit> ListActive(out string error)
        {
            List<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Benefit.Where(b => b.Active).OrderBy(b => b.Title).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
