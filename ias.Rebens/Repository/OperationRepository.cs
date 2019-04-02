using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class OperationRepository : IOperationRepository
    {
        private string _connectionString;
        public OperationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddAddress(int idOperation, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(!db.OperationAddress.Any(o => o.IdOperation == idOperation && o.IdAddress == idAddress))
                    {
                        db.OperationAddress.Add(new OperationAddress() { IdAddress = idAddress, IdOperation = idOperation });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.AddAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool AddContact(int idOperation, int idContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.OperationContact.Any(o => o.IdOperation == idOperation && o.IdContact == idContact))
                    {
                        db.OperationContact.Add(new OperationContact() { IdContact = idContact, IdOperation = idOperation });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(Operation operation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    operation.Code = Guid.NewGuid();
                    operation.Modified = operation.Created = DateTime.UtcNow;
                    operation.PublishStatus = (int)Enums.PublishStatus.notvalid;
                    db.Operation.Add(operation);
                    db.SaveChanges();

                    var staticText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfigurationDefault);
                    var config = new StaticText()
                    {
                        Active = true, 
                        Created = DateTime.UtcNow,
                        Html = staticText.Html,
                        IdOperation = operation.Id,
                        IdStaticTextType = (int)Enums.StaticTextType.OperationConfiguration,
                        Modified = DateTime.UtcNow,
                        Order = staticText.Order,
                        Style = staticText.Style,
                        Title = "Configuração de Operação",
                        Url = "operation-" + operation.Id
                    };
                    db.StaticText.Add(config);
                    db.SaveChanges();

                    var pages = db.StaticText.Where(s => s.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault);
                    var listPages = new List<StaticText>();
                    foreach(var page in pages)
                    {
                        listPages.Add(new StaticText()
                        {
                            Active = page.Active,
                            Created = DateTime.UtcNow,
                            Html = staticText.Html,
                            IdOperation = operation.Id,
                            IdStaticTextType = (int)Enums.StaticTextType.Pages,
                            Modified = DateTime.UtcNow,
                            Order = staticText.Order,
                            Style = staticText.Style,
                            Title = page.Title,
                            Url = page.Url
                        });
                    }
                    db.StaticText.AddRange(listPages);
                    db.SaveChanges();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a opreação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int idOperation, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.OperationAddress.SingleOrDefault(o => o.IdOperation == idOperation && o.IdAddress == idAddress);
                    db.OperationAddress.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteContact(int idOperation, int idContact, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.OperationContact.SingleOrDefault(o => o.IdOperation == idOperation && o.IdContact == idContact);
                    db.OperationContact.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o contato. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Operation> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Operation> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Operation.Where(o => string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "domain asc":
                            tmpList = tmpList.OrderBy(f => f.Domain);
                            break;
                        case "domain desc":
                            tmpList = tmpList.OrderByDescending(f => f.Domain);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "companyname asc":
                            tmpList = tmpList.OrderBy(f => f.CompanyName);
                            break;
                        case "companyname desc":
                            tmpList = tmpList.OrderByDescending(f => f.CompanyName);
                            break;
                        case "companydoc asc":
                            tmpList = tmpList.OrderBy(f => f.CompanyDoc);
                            break;
                        case "companydoc desc":
                            tmpList = tmpList.OrderByDescending(f => f.CompanyDoc);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Operation.Count(o => string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word));

                    ret = new ResultPage<Operation>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ListPage", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Operation.Include("OperationContacts").SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Operation Read(Guid code, out string error)
        {
            Operation ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Operation.SingleOrDefault(c => c.Code == code);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Operation operation, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Operation.SingleOrDefault(c => c.Id == operation.Id);
                    if (update != null)
                    {
                        update.Active = operation.Active;
                        update.CashbackPercentage = operation.CashbackPercentage;
                        update.CompanyDoc = operation.CompanyDoc;
                        update.CompanyName = operation.CompanyName;
                        update.Domain = operation.Domain;
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
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<BenefitOperationItem> ListByBenefit(int idBenefit, out string error)
        {
            List<BenefitOperationItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = (from o in db.Operation
                           from b in db.BenefitOperation.Where(bo => bo.IdOperation == o.Id && bo.IdBenefit == idBenefit).DefaultIfEmpty()
                           where o.Active
                           select new BenefitOperationItem()
                           {
                               IdBenefit = b.IdBenefit,
                               IdOperation = o.Id,
                               IdPosition = b.IdPosition,
                               OperationName = o.Title
                           }).OrderBy(o => o.OperationName).ToList();
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ListByBenefit", ex.Message, $"idBenefit: {idBenefit}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as operações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Operation> ListByBanner(int idBanner, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Operation> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Operation.Where(o => o.Active && o.BannerOperations.Any(bo => bo.IdBanner == idBanner) && (string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "domain asc":
                            tmpList = tmpList.OrderBy(f => f.Domain);
                            break;
                        case "domain desc":
                            tmpList = tmpList.OrderByDescending(f => f.Domain);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "companyname asc":
                            tmpList = tmpList.OrderBy(f => f.CompanyName);
                            break;
                        case "companyname desc":
                            tmpList = tmpList.OrderByDescending(f => f.CompanyName);
                            break;
                        case "companydoc asc":
                            tmpList = tmpList.OrderBy(f => f.CompanyDoc);
                            break;
                        case "companydoc desc":
                            tmpList = tmpList.OrderByDescending(f => f.CompanyDoc);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Operation.Count(o => o.Active && o.BannerOperations.Any(bo => bo.IdBanner == idBanner) && (string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word)));

                    ret = new ResultPage<Operation>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ListByBanner", ex.Message, $"idBanner: {idBanner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as operações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<BannerOperationItem> ListByBanner(int idBanner, out string error)
        {
            List<BannerOperationItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = (from o in db.Operation
                           from b in db.BannerOperation.Where(bo => bo.IdOperation == o.Id && bo.IdBanner == idBanner).DefaultIfEmpty()
                           where o.Active
                           select new BannerOperationItem()
                           {
                               IdBanner = b.IdBanner,
                               IdOperation = o.Id,
                               OperationName = o.Title
                           }).OrderBy(o => o.OperationName).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ListByBanner", ex.Message, $"idBanner: {idBanner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as operações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SavePublishStatus(int id, int idStatus, int? idError, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Operation.SingleOrDefault(o => o.Id == id);
                    update.PublishStatus = idStatus;
                    update.IdLogError = idError;
                    update.Modified = DateTime.UtcNow;

                    db.SaveChanges();
                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.SavePublishStatus", ex.Message, $"id: {id}, idStatus: {idStatus}, idError: {idError}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar o status de publicação operações. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool ValidateOperation(int id, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var operation = db.Operation.SingleOrDefault(o => o.Id == id);
                    if(operation.PublishStatus == (int)Enums.PublishStatus.notvalid 
                        || operation.PublishStatus == (int)Enums.PublishStatus.valid
                        || operation.PublishStatus == (int)Enums.PublishStatus.error)
                    {
                        if(operation.Title != "" && operation.Domain != "" && !string.IsNullOrEmpty(operation.Image ))
                        {
                            var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                            if(configuration != null)
                            {
                                var jObj = JObject.Parse(configuration.Html);

                                int totalOK = 0;
                                int infoTotal = 2;
                                var list = jObj["fields"].Children();
                                foreach (var item in list)
                                {
                                    switch (item["name"].ToString())
                                    {
                                        case "color":
                                            totalOK += item["data"].ToString() != "" ? 1 : 0;
                                            break;
                                        case "favicon":
                                            totalOK += item["data"].ToString() != "" ? 1 : 0;
                                            break;
                                        case "module-coupon":
                                            if (bool.Parse(item["data"].ToString()))
                                                infoTotal += 2;
                                            break;
                                        case "wirecard-token":
                                            if (infoTotal > 2)
                                                totalOK += item["data"].ToString() != "" ? 1 : 0;
                                            break;
                                        case "wirecard-jstoken":
                                            if (infoTotal > 2)
                                                totalOK += item["data"].ToString() != "" ? 1 : 0;
                                            break;
                                    }
                                }
                                if (totalOK == infoTotal)
                                {
                                    operation.PublishStatus = (int)Enums.PublishStatus.valid;
                                    operation.Modified = DateTime.UtcNow;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }

                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ValidateOperation", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar validar a operação. (erro:" + idLog + ")";
            }
            return ret;
        }

        public object GetPublishData(int id, out string error)
        {
            object ret = null;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var operation = db.Operation.SingleOrDefault(o => o.Id == id);
                    if(operation != null)
                    {
                        if(operation.PublishStatus == (int)Enums.PublishStatus.valid)
                        {
                            string color = "", favicon = "";
                            bool couponEnable = false;
                            var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                            if (configuration != null)
                            {
                                var jObj = JObject.Parse(configuration.Html);
                                var list = jObj["fields"].Children();
                                foreach (var item in list)
                                {
                                    switch (item["name"].ToString())
                                    {
                                        case "color":
                                            color = item["data"].ToString();
                                            break;
                                        case "favicon":
                                            favicon = item["data"].ToString();
                                            break;
                                        case "module-coupon":
                                            couponEnable = bool.Parse(item["data"].ToString());
                                            break;
                                    }
                                }
                                ret = new
                                {
                                    Id = operation.Code,
                                    Color = color,
                                    operation.Title,
                                    Logo = operation.Image,
                                    Favicon = favicon,
                                    CouponEnabled = couponEnable,
                                    operation.Domain
                                };
                                error = null;
                            }
                            else
                                error = "Configuração da operação não encontrada!";
                        }
                        else
                            error = "Operação não válida para publicação!";
                    }
                    else
                        error = "Operação não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.GetPublishData", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar validar a operação. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
