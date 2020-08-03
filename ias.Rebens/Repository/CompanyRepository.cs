using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class CompanyRepository : ICompanyRepository
    {
        private string _connectionString;
        public CompanyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Company company, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    company.Modified = company.Created = DateTime.UtcNow;
                    db.Company.Add(company);
                    db.SaveChanges();

                    if (idAdminUser > 0)
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.create,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = company.Id,
                            Item = (int)Enums.LogItem.Company
                        });
                        db.SaveChanges();
                    }

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a empresa. (erro:" + idLog + ")";
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
                    var company = db.Company.SingleOrDefault(c => c.Id == id);
                    if(company != null)
                    {
                        company.Deleted = true;
                        company.Modified = DateTime.UtcNow;
                        if (idAdminUser > 0)
                        {
                            db.LogAction.Add(new LogAction()
                            {
                                Action = (int)Enums.LogAction.delete,
                                Created = DateTime.UtcNow,
                                IdAdminUser = idAdminUser,
                                IdItem = company.Id,
                                Item = (int)Enums.LogItem.Company
                            });
                        }
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Empresa não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar a empresa. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<CompanyItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type = null, int? idItem = null, bool? status = null)
        {
            ResultPage<CompanyItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Company.Include("Address").Include("Contact").Where(c => 
                                    (string.IsNullOrEmpty(word) || c.Name.Contains(word) || c.Cnpj.Contains(word) 
                                        || c.Address.Name.Contains(word) || c.Contact.Name.Contains(word))
                                    && (!status.HasValue || (status.HasValue && c.Active == status.Value))
                                    && (!idItem.HasValue || (idItem.HasValue && c.IdItem == idItem.Value))
                                    && (!type.HasValue || (type.HasValue && c.Type == type.Value))
                                    && !c.Deleted);


                    switch (sort.ToLower())
                    {
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(c => c.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(c => c.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(c => c.Id);
                            break;
                        default:
                            tmpList = tmpList.OrderBy(c => c.Name);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    List<CompanyItem> retList = new List<CompanyItem>();
                    foreach(var item in list)
                    {
                        var newItem = new CompanyItem(item);
                        if (newItem.Type == (int)Enums.LogItem.Operation)
                        {
                            var op = db.Operation.SingleOrDefault(o => o.Id == newItem.IdItem);
                            if (op != null)
                            {
                                newItem.ItemName = op.Title;
                                newItem.Logo = op.Image;
                            }
                        }
                        else
                        {
                            var part = db.Partner.SingleOrDefault(o => o.Id == newItem.IdItem);
                            if (part != null)
                            {
                                newItem.ItemName = part.Name;
                                newItem.Logo = part.Logo;
                            }
                        }
                        retList.Add(newItem);
                    }

                    ret = new ResultPage<CompanyItem>(retList, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as empresas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Company Read(int id, out string error)
        {
            Company ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Company.Include("Address").Include("Contact").SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a empresa. (erro:" + idLog + ")";
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
                    var update = db.Company.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Company,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Empresa não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a empresa. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(Company company, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Company.SingleOrDefault(a => a.Id == company.Id);
                    if (update != null)
                    {
                        update.Name = company.Name;
                        update.Cnpj = company.Cnpj;
                        update.Active = company.Active;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Company,
                            IdItem = update.Id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();
                        error = null;
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                        error = "Empresa não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CompanyRepository.Update", ex.Message, $"id:{company.Id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a empresa. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
