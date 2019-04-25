using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ias.Rebens.Helper;

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
                    benefit.Deleted = false;
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
                    benefit.Deleted = true;
                    benefit.Modified = DateTime.UtcNow;
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
                    if (tmp != null)
                    {
                        db.BenefitAddress.Remove(tmp);
                        db.SaveChanges();
                    }
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
                    if (tmp != null)
                    {
                        db.BenefitOperation.Remove(tmp);
                        db.SaveChanges();
                    }
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

        public ResultPage<Benefit> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? status = null, int? type = null, bool exclusive = false)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => !b.Deleted 
                                    && (string.IsNullOrEmpty(word) || b.Name.Contains(word) || b.Title.Contains(word) || b.Partner.Name.Contains(word))
                                    && (!status.HasValue || (status.HasValue && b.Active == status.Value))
                                    && (!type.HasValue || (type.HasValue && b.IdBenefitType == type.Value))
                                    && (!idOperation.HasValue || (idOperation.HasValue && 
                                         (
                                            (exclusive && b.Exclusive && b.IdOperation == idOperation) ||
                                            (!exclusive && (b.IdOperation == idOperation || b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)))
                                         ))
                                       )
                                );
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(b => b.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(b => b.Title);
                            break;
                        case "name asc":
                            tmpList = tmpList.OrderBy(b => b.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(b => b.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(b => b.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(b => b.Id);
                            break;
                    }

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted
                                    && (string.IsNullOrEmpty(word) || b.Name.Contains(word) || b.Title.Contains(word) || b.Partner.Name.Contains(word))
                                    && (!status.HasValue || (status.HasValue && b.Active == status.Value))
                                    && (!type.HasValue || (type.HasValue && b.IdBenefitType == type.Value))
                                    && (!idOperation.HasValue || (idOperation.HasValue &&
                                         (
                                            (exclusive && b.Exclusive && b.IdOperation == idOperation) ||
                                            (!exclusive && (b.IdOperation == idOperation || b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)))
                                         ))
                                       ));

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
                    ret = db.Benefit.Include("BenefitOperations").Include("BenefitAddresses")
                        .Include("StaticTexts").Include("Partner")
                        .SingleOrDefault(b => !b.Deleted && b.Id == id);
                    if (ret.Partner != null && ret.Partner.IdStaticText.HasValue)
                        ret.Partner.StaticText = db.StaticText.SingleOrDefault(s => s.Id == ret.Partner.IdStaticText.Value);
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
                        update.Link = benefit.Link;
                        update.MaxDiscountPercentage = benefit.MaxDiscountPercentage;
                        update.CPVPercentage = benefit.CPVPercentage;
                        update.MinDiscountPercentage = benefit.MinDiscountPercentage;
                        update.CashbackAmount = benefit.CashbackAmount;
                        update.Start = benefit.Start;
                        update.End = benefit.End;
                        update.IdBenefitType = benefit.IdBenefitType;
                        update.Exclusive = benefit.Exclusive;
                        update.IdIntegrationType = benefit.IdIntegrationType;
                        update.IdPartner = benefit.IdPartner;
                        update.Modified = DateTime.UtcNow;
                        update.Call = benefit.Call;
                        update.Name = benefit.Name;
                        update.VoucherText = benefit.VoucherText;
                        update.IdOperation = benefit.IdOperation;
                        update.HomeHighlight = benefit.HomeHighlight;
                        update.HomeBenefitHighlight = benefit.HomeBenefitHighlight;

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
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.BenefitAddresses.Any(pa => pa.IdAddress == idAddress) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && b.BenefitAddresses.Any(pa => pa.IdAddress == idAddress) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

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
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

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
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

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

        public ResultPage<Benefit> ListByOperation(int idOperation, int? idCategory, string benefitTypes, decimal? latitude, decimal? longitude, int page, int pageItems, string word, string sort, out string error)
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

                    BoundingBox boundingBox = null;
                    List<int> benefitIds = new List<int>();
                    if (latitude.HasValue && longitude.HasValue)
                    {
                        boundingBox = MapHelper.GetBoundingBox((double)latitude.Value, (double)longitude.Value, 10);

                        var addresses = db.BenefitAddress.Include("Address").Where(ba => !string.IsNullOrEmpty(ba.Address.Latitude) && !string.IsNullOrEmpty(ba.Address.Longitude) && ba.Benefit.Active);

                        foreach(var addr in addresses)
                        {
                            if(double.TryParse(addr.Address.Latitude, out double lat) && double.TryParse(addr.Address.Longitude, out double lon))
                            {
                                if (lat >= boundingBox.MinLatitude && lat <= boundingBox.MaxLatitude
                                    && lon >= boundingBox.MinLongitude && lon <= boundingBox.MaxLongitude)
                                    benefitIds.Add(addr.IdBenefit);
                            }
                        }
                    }

                    var tmpList = db.Benefit.Include("Partner")
                                    .Where(b => !b.Deleted && !b.Partner.Deleted && ((!b.Exclusive && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)) || (b.Exclusive && b.IdOperation == idOperation)) 
                                    && (string.IsNullOrEmpty(word) || b.Title.Contains(word) || b.Name.Contains(word) || b.Call.Contains(word) || b.Partner.Name.Contains(word))
                                    && (string.IsNullOrEmpty(benefitTypes) || types.Contains(b.IdBenefitType))
                                    && b.Active
                                    && (!idCategory.HasValue || (idCategory.HasValue && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory.Value || bc.Category.IdParent == idCategory.Value)))
                                    && (boundingBox == null || benefitIds.Any(bi => bi == b.Id))
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && ((!b.Exclusive && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)) || (b.Exclusive && b.IdOperation == idOperation))
                                    && (string.IsNullOrEmpty(word) || b.Title.Contains(word) || b.Name.Contains(word) || b.Call.Contains(word) || b.Partner.Name.Contains(word))
                                    && (string.IsNullOrEmpty(benefitTypes) || types.Contains(b.IdBenefitType))
                                    && b.Active
                                    && (!idCategory.HasValue || (idCategory.HasValue && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory.Value || bc.Category.IdParent == idCategory.Value)))
                                    );

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
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.IdBenefitType == idType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && b.IdBenefitType == idType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

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
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.IdPartner == idPartner && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && b.IdPartner == idPartner && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

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
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.IdIntegrationType == idIntegrationType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
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

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && b.IdIntegrationType == idIntegrationType && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

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
                    if (tmp != null)
                    {
                        db.BenefitCategory.Remove(tmp);
                        db.SaveChanges();
                    }
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

        public void ReadCallAndPartnerLogo(int idBenefit, out string title, out string call, out string logo, out string error)
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var partner = db.Partner.SingleOrDefault(p => p.Benefits.Any(b => b.Id == idBenefit));
                    logo = partner != null ? partner.Logo : "";
                    var benefit = db.Benefit.SingleOrDefault(s => s.Id == idBenefit);
                    title = benefit != null ? benefit.Title : "";
                    call = benefit != null ? benefit.Call : "";
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ReadCallAndPartnerLogo", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a chamada e o logo do parceiro. (erro:" + idLog + ")";
                title = logo = call = null;
            }
        }

        public List<Benefit> ListActive(out string error)
        {
            List<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.Active).OrderBy(b => b.Title).ToList();
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

        public bool SaveCategories(int idBenefit, string categoryIds, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var categories = db.BenefitCategory.Where(b => b.IdBenefit == idBenefit);
                    db.BenefitCategory.RemoveRange(categories);
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(categoryIds))
                    {
                        var ids = categoryIds.Split(',');
                        if (ids.Length > 0)
                        {
                            foreach (var id in ids)
                                db.BenefitCategory.Add(new BenefitCategory() { IdCategory = int.Parse(id), IdBenefit = idBenefit });

                            db.SaveChanges();
                        }
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.SaveCategories", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar as categorias. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Benefit> ListForHomePortal(int idOperation, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    int total = 0;
                    var listPosition = db.Benefit.Include("Partner").Where(b => b.Active && !b.Deleted && !b.Partner.Deleted && b.HomeHighlight > 0 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation));
                    total = listPosition.Count();

                    List<Benefit> listRandom = null;
                    if (total < 8)
                    {
                        listRandom = db.Benefit.Include("Partner").Where(b => b.Active && !b.Deleted && !b.Partner.Deleted && b.HomeHighlight == 0 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).OrderBy(c => Guid.NewGuid()).Take(8-total).ToList();
                        total += listRandom.Count();
                    }
                    List<Benefit> listOthers = null;
                    if(total < 8)
                    {
                        listOthers = db.Benefit.Include("Partner").Where(b => b.Active && !b.Deleted && !b.Partner.Deleted && b.HomeHighlight == -1 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).OrderBy(c => Guid.NewGuid()).Take(8 - total).ToList();
                        total += listOthers.Count();
                    }

                    var list = new List<Benefit>();
                    int randomIdx = 0;
                    int othersIdx = 0;
                    for (int i = 0; i<8; i++)
                    {
                        if (listPosition.Any(b => b.HomeHighlight == (i + 1)))
                            list.Add(listPosition.First(b => b.HomeHighlight == (i + 1)));
                        else if(listRandom != null && listRandom.Count > randomIdx)
                        {
                            list.Add(listRandom.Skip(randomIdx).First());
                            randomIdx++;
                        }
                        else if(listOthers != null && listOthers.Count > othersIdx)
                        {
                            list.Add(listOthers.Skip(othersIdx).First());
                            othersIdx++;
                        }
                    }

                    ret = new ResultPage<Benefit>(list, 0, 8, list.Count);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListForHomePortal", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListForHomeBenefitPortal(int idOperation, out string error)
        {
            ResultPage<Benefit> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    int total = 0;
                    var listPosition = db.Benefit.Include("Partner").Where(b => !b.Deleted && !b.Partner.Deleted && b.Active && b.HomeBenefitHighlight > 0 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation));
                    total = listPosition.Count();

                    List<Benefit> listRandom = null;
                    if (total < 12)
                    {
                        listRandom = db.Benefit.Include("Partner").Where(b => !b.Deleted && !b.Partner.Deleted && b.Active && b.HomeBenefitHighlight == 0 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).OrderBy(c => Guid.NewGuid()).Take(12 - total).ToList();
                        total += listRandom.Count();
                    }
                    List<Benefit> listOthers = null;
                    if (total < 12)
                    {
                        listOthers = db.Benefit.Include("Partner").Where(b => !b.Deleted && !b.Partner.Deleted && b.Active && b.HomeBenefitHighlight == -1 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).OrderBy(c => Guid.NewGuid()).Take(12 - total).ToList();
                        total += listOthers.Count();
                    }

                    var list = new List<Benefit>();
                    int randomIdx = 0;
                    int othersIdx = 0;
                    for (int i = 0; i < 12; i++)
                    {
                        if (listPosition.Any(b => b.HomeBenefitHighlight == (i + 1)))
                            list.Add(listPosition.First(b => b.HomeBenefitHighlight == (i + 1)));
                        else if (listRandom != null && listRandom.Count > randomIdx)
                        {
                            list.Add(listRandom.Skip(randomIdx).First());
                            randomIdx++;
                        }
                        else if (listOthers != null && listOthers.Count > othersIdx)
                        {
                            list.Add(listOthers.Skip(othersIdx).First());
                            othersIdx++;
                        }
                    }

                    ret = new ResultPage<Benefit>(list, 0, 8, list.Count);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BenefitRepository.ListForHomeBenefitPortal", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
