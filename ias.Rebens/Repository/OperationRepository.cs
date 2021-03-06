using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Remotion.Linq.Clauses;
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

        public string GetName(int id, out string error)
        {
            string name = "";
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    name = db.Operation.Single(o => o.Id == id).Title;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.GetName", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o nome da operação. (erro:" + idLog + ")";
            }
            return name;
        }

        public bool AddAddress(int idOperation, int idAddress, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(!db.OperationAddress.Any(o => o.IdOperation == idOperation && o.IdAddress == idAddress))
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.create,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = idAddress,
                            Item = (int)Enums.LogItem.OperationAddress
                        });

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

        public bool Create(Operation operation, int idAdminUser, out string error)
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

                    var staticText = db.StaticText
                                        .Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfigurationDefault);
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
                    }

                    var pages = db.StaticText
                                    .Where(s => s.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault 
                                        && s.Url != "contract" && s.Active);
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

                    db.Faq.Add(new Faq()
                    {
                        Active = true,
                        Answer = "<p><span style='color: rgb(0, 0, 0);'>Não há limites de compras. O usuário compra de acordo com sua necessidade, interesse, prioridade e possibilidade. Da forma que lhe for mais conveniente: dinheiro, débito automático, cartão de crédito, seguindo as normas de cada estabelecimento comercial.</span></p>",
                        Created = DateTime.UtcNow,
                        IdOperation = operation.Id,
                        Modified = DateTime.UtcNow,
                        Order = 1,
                        Question = "Existe um limite de compras na utilização do Clube?"
                    });
                    db.Faq.Add(new Faq()
                    {
                        Active = true,
                        Answer = "<p><span style='color: rgb(0, 0, 0);'>Os descontos podem ser alterados a qualquer tempo, conforme disponibilidade de nossos parceiros. Importante: Certifique-se de que o desconto ou a promoção que lhe interessam ainda estão válidos; Nas compras realizadas através da internet, ou seja, no ambiente virtual, o usuário deve seguir as instruções do estabelecimento no qual realiza sua transação.</span></p>",
                        Created = DateTime.UtcNow,
                        IdOperation = operation.Id,
                        Modified = DateTime.UtcNow,
                        Order = 2,
                        Question = "Os descontos e as promoções oferecidas pelo Clube de Vantagens Rebens são fixos ou temporários?"
                    });
                    db.Faq.Add(new Faq()
                    {
                        Active = true,
                        Answer = "<p><span style='color: rgb(0, 0, 0);'>O cashback (dinheiro de volta) é a porcentagem que recebe de volta, do valor que gastou, na compra efetuada nos parceiros desta categoria do Clube. Com ele você acumula valores que poderão ser resgatados assim que acumular o valor a partir de R$ 25,00.</span></p>",
                        Created = DateTime.UtcNow,
                        IdOperation = operation.Id,
                        Modified = DateTime.UtcNow,
                        Order = 3,
                        Question = "O que é o cashback (dinheiro de volta)?"
                    });
                    db.SaveChanges();

                    var benefits = db.Benefit.Where(b => b.Active).Select(b => b.Id);
                    foreach(var benefit in benefits)
                    {
                        db.BenefitOperation.Add(new BenefitOperation()
                        {
                            Created = DateTime.UtcNow,
                            IdBenefit = benefit,
                            IdOperation = operation.Id,
                            IdPosition = 1,
                            Modified = DateTime.UtcNow
                        });
                    }
                    db.SaveChanges();

                    foreach (var page in listPages)
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.create,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = page.Id,
                            Item = (int)Enums.LogItem.StaticText
                        });
                    }

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = operation.Id,
                        Item = (int)Enums.LogItem.Operation
                    });
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

        public bool DeleteAddress(int idOperation, int idAddress, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.delete,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = idAddress,
                        Item = (int)Enums.LogItem.OperationAddress
                    });

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

        public ResultPage<Entity.OperationListItem> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null)
        {
            ResultPage<Entity.OperationListItem> ret;
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

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(o => new Entity.OperationListItem() { 
                        Id = o.Id,
                        Active = o.Active,
                        CompanyName = o.CompanyName,
                        Created = o.Created,
                        Image = o.Image,
                        PublishedDate = o.PublishedDate,
                        Title = o.Title,
                        Domain = o.Domain,
                        TemporarySubdomain = o.TemporarySubdomain,
                        TemporaryPublishedDate = o.TemporaryPublishedDate
                    }).ToList();

                    list.ForEach(c =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Operation && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        var modifiedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Operation && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.update)
                                            .OrderByDescending(a => a.Created).FirstOrDefault();
                        var publishedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Operation && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.publish)
                                            .OrderByDescending(a => a.Created).FirstOrDefault();
                        if (createUser != null)
                            c.CreatedUserName = createUser.AdminUser.Name + " " + createUser.AdminUser.Surname;
                        else
                            c.CreatedUserName = " - ";
                        if (modifiedUser != null)
                        {
                            c.ModifiedUserName = modifiedUser.AdminUser.Name + " " + modifiedUser.AdminUser.Surname;
                            c.Modified = modifiedUser.Created;
                        }
                        else
                            c.ModifiedUserName = " - ";
                        if (publishedUser != null)
                            c.PublishedUserName = publishedUser.AdminUser.Name + " " + publishedUser.AdminUser.Surname;
                        else
                            c.PublishedUserName = " - ";
                    });

                    ret = new ResultPage<Entity.OperationListItem>(list, page, pageItems, total);

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
                    ret = db.Operation.Include("MainAddress").Include("MainContact").SingleOrDefault(o => !o.Deleted && o.Id == id);
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
                                    openSignUp = item.Data != "closed" && item.Data != "closed-partner";
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

        public bool Update(Operation operation, int idAdminUser, out string error)
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
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = operation.Id,
                            Item = (int)Enums.LogItem.Operation
                        });

                        bool changePolicy = operation.Active != update.Active;
                        update.Active = operation.Active;
                        update.CashbackPercentage = operation.CashbackPercentage;
                        update.CompanyDoc = operation.CompanyDoc;
                        update.CompanyName = operation.CompanyName;
                        update.IdOperationType = operation.IdOperationType;
                        update.Modified = DateTime.UtcNow;
                        if(operation.IdMainAddress.HasValue)
                            update.IdMainAddress = operation.IdMainAddress;
                        if (operation.IdMainContact.HasValue)
                            update.IdMainContact = operation.IdMainContact; 
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

                        if (changePolicy && !operation.Active && !string.IsNullOrEmpty(operation.Domain))
                        {
                            try
                            {
                                var awsHelper = new Integration.AWSHelper();
                                awsHelper.DisableBucketAsync(operation.Domain);
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

        public bool SavePublishStatus(int id, int idStatus, int idAdminUser, int? idError, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.publish,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = id,
                        Item = (int)Enums.LogItem.Operation
                    });

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
                                            if (item.Data == "signature" 
                                                && !db.StaticText.Any(s => s.IdStaticTextType == (int)Enums.StaticTextType.Pages && s.Url == "join-us" && s.IdOperation == operation.Id))
                                            {
                                                var tmpText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault && s.Url == "join-us");
                                                db.StaticText.Add(new StaticText()
                                                {
                                                    Active = true,
                                                    Created = DateTime.UtcNow,
                                                    Html = tmpText.Html,
                                                    IdOperation = operation.Id,
                                                    IdStaticTextType = (int)Enums.StaticTextType.Pages,
                                                    Modified = DateTime.UtcNow,
                                                    Order = tmpText.Order,
                                                    Style = tmpText.Style,
                                                    Title = tmpText.Title,
                                                    Url = tmpText.Url
                                                });
                                                db.SaveChanges();
                                            }
                                            break;
                                    }
                                }

                                bool wirecard = false;
                                bool wirecardJS = false;
                                bool needWirecard = false;
                                foreach (var item in config.Modules)
                                {
                                    if(db.StaticText.Any(s => s.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault && s.Url == item.Name))
                                    {
                                        if (item.Checked)
                                        {
                                            if (!db.StaticText.Any(s => s.IdStaticTextType == (int)Enums.StaticTextType.Pages && s.Url == item.Name && s.IdOperation == operation.Id))
                                            {
                                                var tmpText = db.StaticText.Single(s => s.IdStaticTextType == (int)Enums.StaticTextType.PagesDefault && s.Url == item.Name);
                                                db.StaticText.Add(new StaticText()
                                                {
                                                    Active = true,
                                                    Created = DateTime.UtcNow,
                                                    Html = tmpText.Html,
                                                    IdOperation = operation.Id,
                                                    IdStaticTextType = (int)Enums.StaticTextType.Pages,
                                                    Modified = DateTime.UtcNow,
                                                    Order = tmpText.Order,
                                                    Style = tmpText.Style,
                                                    Title = tmpText.Title,
                                                    Url = tmpText.Url
                                                });
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            var tmpText = db.StaticText.SingleOrDefault(s => s.IdStaticTextType == (int)Enums.StaticTextType.Pages && s.Url == item.Name && s.IdOperation == operation.Id);
                                            if(tmpText != null)
                                            {
                                                db.StaticText.Remove(tmpText);
                                                db.SaveChanges();
                                            }
                                        }
                                    }


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

                        string Color = "", Favicon = "", contactEmail = "", GoogleAnalytics = "", RegisterType = "";
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
                                WirecardToken = config.Wirecard.Token,
                                WirecardJSToken = config.Wirecard.JsToken,
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

                        var action = db.LogAction.Where(a => a.IdItem == op.Id && a.Item == (int)Enums.LogItem.Operation && a.Action == (int)Enums.LogAction.publish).OrderByDescending(a => a.Created).First();
                        var admin = db.AdminUser.Single(a => a.Id == action.IdAdminUser);
                        var listDestinataries = new Dictionary<string, string>() { { admin.Email, admin.Name } };
                        Helper.EmailHelper.SendAdminEmail(listDestinataries, "[REBENS] - Publicação da operação concluída.", $"O processo de publicação da operação {op.Title}, foi concluído com sucesso.", out _);

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

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.delete,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = id,
                        Item = (int)Enums.LogItem.Operation
                    });

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

        public string LoadModulesNames(int id, out string error)
        {
            string ret = "";
            error = null;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                    if (configuration != null)
                    {
                        var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(configuration.Html);
                        foreach(var mod in config.Modules)
                        {
                            if (mod.Checked)
                                ret += mod.Name + "|";
                        }
                        foreach(var field in config.Fields)
                        {
                            if (field.Name == "register-type")
                                ret += field.Data;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.LoadModulesNames", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar carregar os módulos da operação. (erro:" + idLog + ")";
            }
            return ret;
        }

        public List<Operation> ListWithModule(string module, out string error)
        {
            List<Operation> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.StaticText.Where(s => s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration).ToList();
                    var operationIds = new List<int>();

                    foreach(var op in list)
                    {
                        if (op.Html != null && !string.IsNullOrEmpty(op.Html))
                        {
                            var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(op.Html);
                            foreach (var item in config.Modules)
                            {
                                if (item.Name == module && item.Checked)
                                {
                                    operationIds.Add(op.IdOperation.Value);
                                    break;
                                }
                            }
                        }
                    }

                    ret = db.Operation.Where(o => o.Active && operationIds.Any(id => o.Id == id)).OrderBy(o => o.Title).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ListWithModule", ex.Message, $"module: {module}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as operações que possuem o módulo requisitado. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool ToggleActive(int id, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Operation.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Operation,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();

                        if (update.Active)
                        {
                            var awsHelper = new Integration.AWSHelper();
                            if (!string.IsNullOrEmpty(update.Domain))
                            {
                                try
                                {
                                    awsHelper.DisableBucketAsync(update.Domain).Wait();
                                }
                                catch (Exception ex)
                                {
                                    ret = false;
                                    error = $"Ocorreu um erro ao tentar bloquear o acesso na Amazon. Mensagem: '{ex.Message}'";
                                }
                            }
                            try
                            {
                                awsHelper.DisableBucketAsync($"{update.TemporarySubdomain}.sistemarebens.com.br").Wait();
                            }
                            catch (Exception ex)
                            {
                                ret = false;
                                error = $"Ocorreu um erro ao tentar bloquear o acesso na Amazon. Mensagem: '{ex.Message}'";
                            }
                        }

                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Operação não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a operação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public string GetFromEmail(int id)
        {
            string ret = "";
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var configuration = db.StaticText.SingleOrDefault(s => s.IdOperation == id && s.IdStaticTextType == (int)Enums.StaticTextType.OperationConfiguration);
                    if (configuration != null)
                    {
                        var jObj2 = JObject.Parse(configuration.Html);
                        var listFields = jObj2["fields"].Children();
                        foreach (var item2 in listFields)
                        {
                            if (item2["name"].ToString() == "contact-email")
                            {
                                ret = item2["data"].ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("OperationRepository.GetFromEmail", ex.Message, $"id: {id}", ex.StackTrace);
            }
            return string.IsNullOrEmpty(ret) ? "contato@rebens.com.br" : ret;
        }

        public List<Operation> ListActive()
        {
            List<Operation> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Operation.Where(o => o.Active).ToList();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("OperationRepository.ListActive", ex.Message, "", ex.StackTrace);
                ret = null;
            }
            return ret;
        }
    }
}
