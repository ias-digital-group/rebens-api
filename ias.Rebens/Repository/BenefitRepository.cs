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
        public BenefitRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool AddAddress(int idBenefit, int idAddress, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.BenefitAddress.Any(o => o.IdBenefit == idBenefit && o.IdAddress == idAddress))
                    {
                        db.BenefitAddress.Add(new BenefitAddress() { IdAddress = idAddress, IdBenefit = idBenefit });
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.addAddress,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = idBenefit,
                            Item = (int)Enums.LogItem.Benefit
                        });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.AddAddress", "", ex);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<Benefit> ListToCheckLinks()
        {
            List<Benefit> ret;
            using (var db = new RebensContext(this._connectionString))
            {
                ret = db.Benefit.Where(b => b.Active && b.Link != "").ToList();
            }
            return ret;
        }

        public bool Create(Benefit benefit, int idAdminUser, out string error)
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

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = benefit.Id,
                        Item = (int)Enums.LogItem.Benefit
                    });
                    db.SaveChanges();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.Create", "", ex);
                error = "Ocorreu um erro ao tentar criar o benefício. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var benefit = db.Benefit.SingleOrDefault(c => c.Id == id);
                    benefit.Deleted = true;
                    benefit.Modified = DateTime.UtcNow;
                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.delete,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = id,
                        Item = (int)Enums.LogItem.Benefit
                    });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.Delete", "", ex);
                error = "Ocorreu um erro ao tentar excluir o beneficio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int idBenefit, int idAddress, int idAdminUser, out string error)
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
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.removeAddress,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = idBenefit,
                            Item = (int)Enums.LogItem.Benefit
                        });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.DeleteAddress", "", ex);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
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
                    if (!int.TryParse(word, out int benefitId))
                        benefitId = 0;
                    var tmpList = db.Benefit.Include("Partner").Where(b => !b.Deleted 
                                    && (string.IsNullOrEmpty(word) || b.Name.Contains(word) || b.Title.Contains(word) || b.Partner.Name.Contains(word) || b.Id == benefitId)
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
                    var total = tmpList.Count();

                    ret = new ResultPage<Benefit>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListPage", "", ex);
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
                        .Include("BenefitCategories")
                        .SingleOrDefault(b => !b.Deleted && b.Id == id);
                    if (ret.Partner != null && ret.Partner.IdStaticText.HasValue)
                        ret.Partner.StaticText = db.StaticText.SingleOrDefault(s => s.Id == ret.Partner.IdStaticText.Value);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.Read", "", ex);
                error = "Ocorreu um erro ao tentar ler o benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Benefit benefit, int idAdminUser, out string error)
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
                        update.TaxAmount = benefit.TaxAmount;
                        update.AvailableCashback = benefit.AvailableCashback;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = benefit.Id,
                            Item = (int)Enums.LogItem.Benefit
                        });

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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.Update", "", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByAddress", $"idAddress: {idAddress}", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByCategory", $"idCategory: {idCategory}", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByOperation", $"idOperation: {idOperation}", ex);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Benefit> ListByOperation(int idOperation, int? idCategory, string benefitTypes, decimal? latitude, decimal? longitude, int page, 
                                    int pageItems, string word, string sort, string idBenefits, string state, string city, out string error)
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

                    List<int> listIds = new List<int>();
                    if (!string.IsNullOrEmpty(idBenefits)) {
                        foreach (var id in idBenefits.Split(',')) listIds.Add(int.Parse(id));
                    }

                    var tmpList = db.Benefit.Include("Partner")
                                    .Where(b => !b.Deleted && !b.Partner.Deleted && ((!b.Exclusive && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)) || (b.Exclusive && b.IdOperation == idOperation)) 
                                        && (string.IsNullOrEmpty(word) || b.Title.Contains(word) || b.Name.Contains(word) || b.Call.Contains(word) || b.Partner.Name.Contains(word))
                                        && (string.IsNullOrEmpty(benefitTypes) || types.Contains(b.IdBenefitType))
                                        && b.Active
                                        && (!idCategory.HasValue || (idCategory.HasValue && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory.Value || bc.Category.IdParent == idCategory.Value)))
                                        && (boundingBox == null || benefitIds.Any(bi => bi == b.Id))
                                        && (string.IsNullOrEmpty(state) || b.BenefitAddresses.Any(a => a.Address.State == state))
                                        && (string.IsNullOrEmpty(city) || b.BenefitAddresses.Any(a => a.Address.City == city))
                                        //&& !listIds.Any(i => i == b.Id)
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

                    var total = tmpList.Count();

                    if (total < pageItems || total < (page * pageItems))
                        page = 0;
                    
                    var list = tmpList.Where(b => !listIds.Any(i => i == b.Id)).Skip((page * pageItems) - listIds.Count).Take(pageItems).ToList();
                   
                    ret = new ResultPage<Benefit>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByOperation", $"idOperation: {idOperation}", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByType", $"idType: {idType}", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByPartner", $"idPartner: {idPartner}", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListByIntegrationType", $"idIntegrationType: {idIntegrationType}", ex);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListPositions", "", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ReadCallAndPartnerLogo", "", ex);
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListActive", "", ex);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SaveCategories(int idBenefit, string categoryIds, int idAdminUser, out string error)
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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.SaveCategories", "", ex);
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

                    List<Benefit> listRandom = db.Benefit.Include("Partner").Where(b => b.Active && !b.Deleted && !b.Partner.Deleted && b.HomeHighlight == 0 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).OrderBy(c => Guid.NewGuid()).Take(8).ToList();
                    List<Benefit> listOthers = db.Benefit.Include("Partner").Where(b => b.Active && !b.Deleted && !b.Partner.Deleted && b.HomeHighlight == -1 && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).OrderBy(c => Guid.NewGuid()).Take(8).ToList();

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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListForHomePortal", $"idOperation: {idOperation}", ex);
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
                    total = listPosition.Select(l => l.HomeBenefitHighlight).Distinct().Count();

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
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListForHomeBenefitPortal", $"idOperation: {idOperation}", ex);
                error = "Ocorreu um erro ao tentar listar os benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Tuple<string, string>> ListStates(int idOperation, out string error)
        {
            List<Tuple<string, string>> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).Select(b => b.Id);
                    var list = db.Address.Where(a => a.BenefitAddresses.Any(b => tmpList.Any(l => l == b.IdBenefit))).Select(a => a.State).Distinct();
                    ret = new List<Tuple<string, string>>();
                    foreach(var state in list)
                    {
                        ret.Add(new Tuple<string, string>(state, state));
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListStates", $"idOperation: {idOperation}", ex);
                error = "Ocorreu um erro ao tentar listar os estados dos benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
        
        public List<Tuple<string, string>> ListCities(int idOperation, out string error, string state = null)
        {
            List<Tuple<string, string>> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Benefit.Where(b => !b.Deleted && !b.Partner.Deleted && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)).Select(b => b.Id);
                    var list = db.Address.Where(a => a.BenefitAddresses.Any(b => tmpList.Any(l => l == b.IdBenefit)) && (string.IsNullOrEmpty(state) || a.State == state)).Select(a => a.City).Distinct();
                    ret = new List<Tuple<string, string>>();
                    foreach (var city in list)
                    {
                        ret.Add(new Tuple<string, string>(city, city));
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ListCities", $"idOperation: {idOperation}", ex);
                error = "Ocorreu um erro ao tentar listar as cidades dos benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool ToggleActive(int id, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Benefit.SingleOrDefault(c => c.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Benefit,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Benefício não encontrado!";
                        ret = false;
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ChangeActive", "", ex);
                error = "Ocorreu um erro ao tentar atualizar o benefício. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Duplicate(int id, out int newId, int idAdminUser, out string error)
        {
            bool ret = false;
            newId = 0;
            try
            {
                Benefit benefit;
                using (var db = new RebensContext(this._connectionString))
                {
                    benefit = db.Benefit.SingleOrDefault(c => c.Id == id);
                }

                if (benefit != null)
                {
                    using (var db = new RebensContext(this._connectionString))
                    {
                        benefit.Id = 0;
                        benefit.Name += " - CÓPIA";
                        benefit.Active = false;
                        benefit.Created = benefit.Modified = DateTime.UtcNow;
                        db.Benefit.Add(benefit);
                        db.SaveChanges();

                        newId = benefit.Id;

                        var addrs = db.BenefitAddress.Where(c => c.IdBenefit == id);
                        foreach (var addr in addrs)
                        {
                            db.BenefitAddress.Add(new BenefitAddress()
                            {
                                IdAddress = addr.IdAddress,
                                IdBenefit = newId
                            });
                        }

                        var categories = db.BenefitCategory.Where(c => c.IdBenefit == id);
                        foreach (var category in categories)
                        {
                            db.BenefitCategory.Add(new BenefitCategory()
                            {
                                IdCategory = category.IdCategory,
                                IdBenefit = newId
                            });
                        }

                        var operations = db.BenefitOperation.Where(c => c.IdBenefit == id);
                        foreach (var operation in operations)
                        {
                            db.BenefitOperation.Add(new BenefitOperation()
                            {
                                IdOperation = operation.IdOperation,
                                IdBenefit = newId,
                                IdPosition = operation.IdPosition,
                                Created = DateTime.UtcNow,
                                Modified = DateTime.UtcNow
                            });
                        }

                        var texts = db.StaticText.Where(c => c.IdBenefit == id);
                        foreach (var text in texts)
                        {
                            db.StaticText.Add(new StaticText()
                            {
                                IdOperation = text.IdOperation,
                                IdBenefit = newId,
                                Title = text.Title,
                                Url = text.Url,
                                Html = text.Html,
                                Style = text.Style,
                                Order = text.Order,
                                IdStaticTextType = text.IdStaticTextType,
                                Active = text.Active,
                                Created = DateTime.UtcNow,
                                Modified = DateTime.UtcNow
                            });
                        }

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.duplicate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Benefit,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();
                        error = null;
                        ret = true;
                    }
                }
                else
                    error = "Curso não encontrado!";
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "CourseRepository.Duplicate", "", ex);
                error = "Ocorreu um erro ao tentar duplicar o curso. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool ConnectOperations(int id, int[] operations, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpOperations = db.BenefitOperation.Where(b => b.IdBenefit == id);
                    db.BenefitOperation.RemoveRange(tmpOperations);
                    db.SaveChanges();

                    foreach (var op in operations)
                        db.BenefitOperation.Add(new BenefitOperation() { IdOperation = op, IdBenefit = id, IdPosition = 1, Created  = DateTime.UtcNow, Modified = DateTime.UtcNow });
                    db.SaveChanges();

                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ConnectOperations", "", ex);
                error = "Ocorreu um erro ao tentar ajustar as conexões das operação. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool ConnectCategories(int id, int[] categories, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpCategories = db.BenefitCategory.Where(b => b.IdBenefit == id);
                    db.BenefitCategory.RemoveRange(tmpCategories);
                    db.SaveChanges();

                    foreach (var op in categories)
                        db.BenefitCategory.Add(new BenefitCategory() { IdCategory = op, IdBenefit = id });
                    db.SaveChanges();

                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = LogErrorHelper.Create(this._connectionString, "BenefitRepository.ConnectCategories", "", ex);
                error = "Ocorreu um erro ao tentar ajustar as conexões das categorias. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
