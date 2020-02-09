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
        private Constant constant;
        public OperationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            constant = new Constant();
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
                    operation.Deleted = false;
                    operation.SubdomainCreated = false;
                    db.Operation.Add(operation);
                    db.SaveChanges();

                    var staticText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfigurationDefault);
                    if (staticText != null)
                    {
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
                    }

                    var pages = db.StaticText.Where(s => s.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault && s.Url != "contract");
                    var listPages = new List<StaticText>();
                    foreach(var page in pages)
                    {
                        listPages.Add(new StaticText()
                        {
                            Active = page.Active,
                            Created = DateTime.UtcNow,
                            Html = page.Html,
                            IdOperation = operation.Id,
                            IdStaticTextType = (int)Enums.StaticTextType.Pages,
                            Modified = DateTime.UtcNow,
                            Order = page.Order,
                            Style = page.Style,
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

        public ResultPage<Operation> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null)
        {
            ResultPage<Operation> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Operation.Where(o => !o.Deleted 
                                    && (string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word))
                                    && (!status.HasValue || (status.HasValue && o.Active == status.Value)));
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
                    var total = db.Operation.Count(o => !o.Deleted
                                    && (string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word))
                                    && (!status.HasValue || (status.HasValue && o.Active == status.Value)));

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
            ValidateOperation(id, out error);
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Operation.Include("OperationContacts").SingleOrDefault(o => !o.Deleted && o.Id == id);
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
                    ret = db.Operation.SingleOrDefault(o => !o.Deleted && o.Code == code);
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

        public Operation ReadForSignUp(Guid code, out bool openSignUp, out string error)
        {
            Operation ret;
            openSignUp = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Operation.SingleOrDefault(o => !o.Deleted && o.Code == code);
                    if(ret != null)
                    {
                        var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == ret.Id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                        if(configuration != null)
                        {
                            var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(configuration.Html);
                            foreach (var item in config.Fields)
                            {
                                if(item.Name == "register-type")
                                    openSignUp = item.Data == "open";
                            }
                        }
                    }

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ReadForSignUp", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a operação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Operation operation, out string error)
        {
            bool ret = true;
            bool enablePublish = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Operation.SingleOrDefault(c => c.Id == operation.Id);
                    if (update != null)
                    {

                        bool changePolicy = operation.Active != update.Active;
                        update.Active = operation.Active;
                        update.CashbackPercentage = operation.CashbackPercentage;
                        update.CompanyDoc = operation.CompanyDoc;
                        update.CompanyName = operation.CompanyName;
                        update.IdOperationType = operation.IdOperationType;
                        update.Modified = DateTime.UtcNow;
                        if (update.Image != operation.Image)
                        {
                            update.Image = operation.Image;
                            enablePublish = true;
                        }
                        if (update.Title != operation.Title)
                        {
                            update.Title = operation.Title;
                            enablePublish = true;
                        }
                        if (update.Domain != operation.Domain)
                        {
                            update.Domain = operation.Domain;
                            enablePublish = true;
                        }
                        if (update.TemporarySubdomain != operation.TemporarySubdomain)
                        {
                            enablePublish = true;
                            update.TemporarySubdomain = operation.TemporarySubdomain;
                            update.SubdomainCreated = false;
                        }
                        
                        if(enablePublish)
                        {
                            update.TemporaryPublishStatus = (int)Enums.PublishStatus.notvalid;
                            update.PublishStatus = (int)Enums.PublishStatus.notvalid;
                        }

                        db.SaveChanges();
                        error = null;

                        if (changePolicy)
                        {
                            try
                            {
                                var awsHelper = new Integration.AWSHelper();
                                awsHelper.ChangeBucketPolicy(operation.Domain, operation.Active).Wait();
                            }
                            catch(Exception ex)
                            {
                                ret = false;
                                error = $"Ocorreu um erro ao tentar bloquear o acesso na Amazon. Mensagem: '{ex.Message}'";
                            }
                        }
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

            if (enablePublish)
                ValidateOperation(operation.Id, out error);

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
                           where !o.Deleted && o.Active
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
                    var tmpList = db.Operation.Where(o => !o.Deleted && o.Active && o.BannerOperations.Any(bo => bo.IdBanner == idBanner) && (string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word)));
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
                    var total = db.Operation.Count(o => !o.Deleted && o.Active && o.BannerOperations.Any(bo => bo.IdBanner == idBanner) && (string.IsNullOrEmpty(word) || o.Domain.Contains(word) || o.Title.Contains(word) || o.CompanyName.Contains(word) || o.CompanyDoc.Contains(word)));

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
                           where !o.Deleted && o.Active
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
                    if (idStatus == (int)Enums.PublishStatus.processing)
                    {
                        if (update.PublishStatus == (int)Enums.PublishStatus.publish)
                            update.PublishStatus = idStatus;
                        else if (update.TemporaryPublishStatus == (int)Enums.PublishStatus.publish)
                            update.TemporaryPublishStatus = idStatus;
                    }
                    else if (idStatus == (int)Enums.PublishStatus.done)
                    {
                        if (update.PublishStatus == (int)Enums.PublishStatus.processing)
                        {
                            update.PublishStatus = idStatus;
                            update.PublishedDate = DateTime.UtcNow;
                        }
                        else if (update.TemporaryPublishStatus == (int)Enums.PublishStatus.processing)
                        {
                            update.TemporaryPublishStatus = idStatus;
                            update.TemporaryPublishedDate = DateTime.UtcNow;
                        }
                    }
                    else if(idStatus == (int)Enums.PublishStatus.error)
                    {
                        if (update.PublishStatus == (int)Enums.PublishStatus.processing)
                            update.PublishStatus = idStatus;
                        else if (update.TemporaryPublishStatus == (int)Enums.PublishStatus.processing)
                            update.TemporaryPublishStatus = idStatus;

                        update.IdLogError = idError;
                    }
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
                    if (operation.PublishStatus != (int)Enums.PublishStatus.processing && 
                        operation.TemporaryPublishStatus != (int)Enums.PublishStatus.processing)
                    {
                        if (operation.Title != "" && !string.IsNullOrEmpty(operation.Image))
                        {
                            var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                            if (configuration != null)
                            {
                                var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(configuration.Html);

                                int totalOK = 0;
                                int infoTotal = 3;
                                bool coupon = false;
                                foreach (var item in config.Fields)
                                {
                                    switch (item.Name)
                                    {
                                        case "color":
                                            totalOK += item.Data != "" ? 1 : 0;
                                            break;
                                        case "favicon":
                                            totalOK += item.Data != "" ? 1 : 0;
                                            break;
                                        case "register-type":
                                            totalOK += item.Data != "" ? 1 : 0;
                                            break;
                                    }
                                }

                                bool wirecard = false;
                                bool wirecardJS = false;
                                bool needWirecard = false;
                                foreach (var item in config.Modules)
                                {
                                    if ((item.Name == "course" || item.Name == "freeCourse" || item.Name == "coupon" ) && needWirecard && !wirecard && !wirecardJS && item.Checked)
                                    {
                                        needWirecard = true;
                                        foreach (var field in item.Info.Fields)
                                        {
                                            if (field.Name == "wirecardToken" && !string.IsNullOrEmpty(field.Data))
                                                wirecard = true;
                                            if (field.Name == "wirecardJSToken" && !string.IsNullOrEmpty(field.Data))
                                                wirecardJS = true;
                                        }
                                    }

                                    if(item.Name == "coupon" && item.Checked)
                                    {
                                        var contract = db.StaticText.SingleOrDefault(c => c.IdOperation == id && c.IdStaticTextType == (int)Enums.StaticTextType.Pages && c.Url == "contract");
                                        if (coupon)
                                        {
                                            if (contract == null)
                                            {
                                                contract = db.StaticText.Single(c => c.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault && c.Url == "contract");
                                                db.StaticText.Add(new StaticText()
                                                {
                                                    Active = true,
                                                    Created = DateTime.UtcNow,
                                                    Html = contract.Html,
                                                    IdOperation = id,
                                                    IdStaticTextType = (int)Enums.StaticTextType.Pages,
                                                    Modified = DateTime.UtcNow,
                                                    Order = contract.Order,
                                                    Style = contract.Style,
                                                    Title = contract.Title,
                                                    Url = contract.Url
                                                });
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            if (contract != null)
                                            {
                                                db.StaticText.Remove(contract);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                infoTotal += needWirecard ? 1 : 0;
                                totalOK += wirecard && wirecardJS ? 1 : 0;

                                if (totalOK == infoTotal)
                                {
                                    if (operation.TemporaryPublishStatus == (int)Enums.PublishStatus.done && (
                                            ((operation.PublishStatus == (int)Enums.PublishStatus.done || operation.PublishStatus == (int)Enums.PublishStatus.notvalid) && operation.PublishedDate < operation.TemporaryPublishedDate)
                                            || 
                                            !operation.PublishedDate.HasValue
                                        )
                                    )
                                    {
                                        if (operation.Domain != "")
                                        {
                                            if (!Uri.IsWellFormedUriString(operation.Domain, UriKind.Relative))
                                            {
                                                operation.Domain = operation.Domain.Replace("http://", "").Replace("https://", "");
                                                if (operation.Domain.IndexOf("/") > 0)
                                                    operation.Domain = operation.Domain.Substring(0, operation.Domain.IndexOf("/"));
                                            }
                                            operation.PublishStatus = (int)Enums.PublishStatus.publish;
                                            operation.Modified = DateTime.UtcNow;
                                            ret = true;
                                        }
                                        else
                                        {
                                            operation.PublishStatus = (int)Enums.PublishStatus.notvalid;
                                            operation.Modified = DateTime.UtcNow;
                                        }
                                        
                                    }
                                    else
                                    {
                                        if (operation.TemporarySubdomain != "")
                                        {
                                            char[] arr = operation.TemporarySubdomain.ToCharArray();
                                            arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')));
                                            operation.TemporarySubdomain = new string(arr);
                                            operation.TemporaryPublishStatus = (int)Enums.PublishStatus.publish;
                                            operation.Modified = DateTime.UtcNow;
                                            ret = true;
                                        }
                                        else
                                        {
                                            operation.TemporaryPublishStatus = (int)Enums.PublishStatus.notvalid;
                                            operation.Modified = DateTime.UtcNow;
                                        }
                                    }

                                    db.SaveChanges();
                                    error = null;
                                }
                                else
                                    error = "A configuração da operação está incompleta.";
                            }
                            else
                                error = "Não foi encontrado o arquivo de configuração da operação.";
                        }
                        else
                            error = "Os campos título e o logo do clube são obrigatórios para publicação.";
                    }
                    else
                        error = "A Operação já está publicada.";
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

        public object GetPublishData(int id, out string domain, out string error)
        {
            object ret = null;
            domain = "";
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var operation = db.Operation.SingleOrDefault(o => o.Id == id);
                    if(operation != null)
                    {
                        bool isTemporary = operation.TemporaryPublishStatus == (int)Enums.PublishStatus.publish;

                        string Color = "", Favicon = "", contactEmail = "", GoogleAnalytics = "", WirecardToken = "", WirecardJSToken = "", RegisterType = "";
                        var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                        if (configuration != null)
                        {
                            var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(configuration.Html);

                            foreach (var item in config.Fields)
                            {
                                switch (item.Name)
                                {
                                    case "color":
                                        Color = item.Data;
                                        break;
                                    case "favicon":
                                        Favicon = item.Data;
                                        break;
                                    case "contact-mail":
                                        contactEmail = item.Data;
                                        break;
                                    case "g-analytics":
                                        GoogleAnalytics = item.Data;
                                        break;
                                    case "register-type":
                                        RegisterType = item.Data;
                                        break;
                                }
                            }

                            foreach(var item in config.Modules)
                            {
                                if((item.Name == "course" || item.Name == "freeCourse" || item.Name == "coupon")
                                    && string.IsNullOrEmpty(WirecardToken))
                                {
                                    foreach (var field in item.Info.Fields)
                                    {
                                        if (field.Name == "wirecardToken" && !string.IsNullOrEmpty(field.Data))
                                            WirecardToken = field.Data;
                                        if (field.Name == "wirecardJSToken" && !string.IsNullOrEmpty(field.Data))
                                            WirecardToken = field.Data;
                                    }
                                }
                            }

                            domain = (isTemporary ? operation.TemporarySubdomain + ".sistemarebens.com.br" : operation.Domain);
                            ret = new
                            {
                                Id = operation.Code,
                                Color,
                                operation.Title,
                                Logo = operation.Image,
                                Favicon,
                                GoogleAnalytics,
                                Domain = (isTemporary ? operation.TemporarySubdomain + ".sistemarebens.com.br" :  operation.Domain),
                                constant.AppSettings.App.Environment,
                                WirecardToken,
                                WirecardJSToken,
                                RegisterType, 
                                config.Modules 
                            };

                            var logError = new LogErrorRepository(this._connectionString);
                            int idLog = logError.Create("OperationRepository.GetPublishData", "Envoirement", constant.AppSettings.App.Environment, "");

                            contactEmail = string.IsNullOrEmpty(contactEmail) ? ("contato@" + (isTemporary ? operation.TemporarySubdomain + ".sistemarebens.com.br" :  operation.Domain)) : contactEmail;
                            var staticText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.EmailCustomerValidationDefault);
                            if (staticText != null)
                            {
                                var html = staticText.Html.Replace("##CONFIG_DOMAIN##", operation.Domain);
                                html = html.Replace("##CONFIG_TITLE##", operation.Title);
                                html = html.Replace("##CONFIG_LOGO##", operation.Image);
                                html = html.Replace("##CONFIG_EMAIL##", contactEmail);
                                html = html.Replace("##COLOR##", Color);

                                var update = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)Enums.StaticTextType.EmailCustomerValidation);
                                if (update != null)
                                {
                                    update.Modified = DateTime.UtcNow;
                                    update.Html = html;
                                    update.Style = staticText.Style;
                                    update.Url = staticText.Url;
                                    update.Title = staticText.Title;
                                    update.Order = staticText.Order;
                                }
                                else
                                {
                                    var email1 = new StaticText()
                                    {
                                        Active = true,
                                        Created = DateTime.UtcNow,
                                        Html = html,
                                        IdOperation = operation.Id,
                                        IdStaticTextType = (int)Enums.StaticTextType.EmailCustomerValidation,
                                        Modified = DateTime.UtcNow,
                                        Order = staticText.Order,
                                        Style = staticText.Style,
                                        Title = staticText.Title,
                                        Url = staticText.Url
                                    };
                                    db.StaticText.Add(email1);
                                }
                            }

                            staticText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.EmailPasswordRecoveryDefault);
                            if (staticText != null)
                            {
                                var html = staticText.Html.Replace("##CONFIG_DOMAIN##", operation.Domain);
                                html = html.Replace("##CONFIG_TITLE##", operation.Title);
                                html = html.Replace("##CONFIG_LOGO##", operation.Image);
                                html = html.Replace("##CONFIG_EMAIL##", contactEmail);
                                html = html.Replace("##COLOR##", Color);

                                var update = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)Enums.StaticTextType.EmailPasswordRecovery);
                                if (update != null)
                                {
                                    update.Modified = DateTime.UtcNow;
                                    update.Html = html;
                                    update.Style = staticText.Style;
                                    update.Url = staticText.Url;
                                    update.Title = staticText.Title;
                                    update.Order = staticText.Order;
                                }
                                else
                                {
                                    var email2 = new StaticText()
                                    {
                                        Active = true,
                                        Created = DateTime.UtcNow,
                                        Html = html,
                                        IdOperation = operation.Id,
                                        IdStaticTextType = (int)Enums.StaticTextType.EmailPasswordRecovery,
                                        Modified = DateTime.UtcNow,
                                        Order = staticText.Order,
                                        Style = staticText.Style,
                                        Title = staticText.Title,
                                        Url = staticText.Url
                                    };
                                    db.StaticText.Add(email2);
                                }
                            }

                            staticText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.EmailDefault);
                            if (staticText != null)
                            {
                                var html = staticText.Html.Replace("##CONFIG_DOMAIN##", operation.Domain);
                                html = html.Replace("##CONFIG_TITLE##", operation.Title);
                                html = html.Replace("##CONFIG_LOGO##", operation.Image);
                                html = html.Replace("##CONFIG_EMAIL##", contactEmail);
                                html = html.Replace("##COLOR##", Color);

                                var update = db.StaticText.SingleOrDefault(s => s.IdOperation == operation.Id && s.IdStaticTextType == (int)Enums.StaticTextType.Email);
                                if (update != null)
                                {
                                    update.Modified = DateTime.UtcNow;
                                    update.Html = html;
                                    update.Style = staticText.Style;
                                    update.Url = staticText.Url;
                                    update.Title = staticText.Title;
                                    update.Order = staticText.Order;
                                }
                                else
                                {
                                    var email3 = new StaticText()
                                    {
                                        Active = true,
                                        Created = DateTime.UtcNow,
                                        Html = html,
                                        IdOperation = operation.Id,
                                        IdStaticTextType = (int)Enums.StaticTextType.Email,
                                        Modified = DateTime.UtcNow,
                                        Order = staticText.Order,
                                        Style = staticText.Style,
                                        Title = staticText.Title,
                                        Url = staticText.Url
                                    };
                                    db.StaticText.Add(email3);
                                }
                            }
                            db.SaveChanges();

                            error = null;
                        }
                        else
                            error = "Configuração da operação não encontrada!";
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

        public bool SavePublishDone(Guid code, out string error)
        {
            bool ret = false;
            var logError = new LogErrorRepository(this._connectionString);
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var op = db.Operation.SingleOrDefault(o => o.Code == code);
                    if (op != null)
                    {
                        if (op.PublishStatus == (int)Enums.PublishStatus.processing)
                        {
                            op.PublishStatus = (int)Enums.PublishStatus.done;
                            op.PublishedDate = DateTime.UtcNow;
                        }
                        else if (op.TemporaryPublishStatus == (int)Enums.PublishStatus.processing)
                        {
                            op.TemporaryPublishStatus = (int)Enums.PublishStatus.done;
                            op.TemporaryPublishedDate = DateTime.UtcNow;
                        }
                        op.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Operação não foi encontrada";
                }
            }
            catch(Exception ex)
            {
                int idLog = logError.Create("OperationRepository.SavePublishDone", ex.Message, $"code: {code}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar macar a publicação da operação como concluída. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool Delete(int id, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Operation.SingleOrDefault(o => o.Id == id);
                    update.Deleted = true;
                    update.Modified = DateTime.UtcNow;

                    db.SaveChanges();
                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.Delete", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar a status. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool SetSubdomainCreated(int id, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var op = db.Operation.SingleOrDefault(o => o.Id == id);
                    if (op != null)
                    {
                        op.SubdomainCreated = true;
                        op.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Operação não foi encontrada";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.SetSubdomainCreated", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar macar o subdomínio como criado. (erro:" + idLog + ")";
            }
            return ret;
        }

        public string GetConfigurationOption(int id, string field, out string error)
        {
            string ret = "";
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                    if (configuration != null)
                    {
                        var jObj = JObject.Parse(configuration.Html);
                        var list = jObj["fields"].Children();
                        foreach (var item in list)
                        {
                            if(item["name"].ToString() == field)
                            {
                                ret = item["data"].ToString();
                                break;
                            }
                        }
                        error = null;
                    }
                    else
                        error = "Configuração da operação não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.GetConfigurationOption", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar recuperar a opção da configuração. (erro:" + idLog + ")";
            }
            return ret;
        }

        public int GetId(Guid operationGuid, out string error)
        {
            int ret = 0;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var operation = db.Operation.SingleOrDefault(o => o.Code == operationGuid);
                    if (operation != null)
                        ret = operation.Id;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.GetId", ex.Message, $"operationGuid: \"{operationGuid}\"", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a operação. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool SaveSendingblueListId(int id, int listId, out string error)
        {
            bool ret = false;
            var logError = new LogErrorRepository(this._connectionString);
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var op = db.Operation.SingleOrDefault(o => o.Id == id);
                    if (op != null)
                    {
                        op.SendinblueListId = listId;
                        op.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Operação não foi encontrada";
                }
            }
            catch (Exception ex)
            {
                int idLog = logError.Create("OperationRepository.SaveSendingblueListId", ex.Message, $"id: {id}, listId: {listId}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar o id da lista. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
