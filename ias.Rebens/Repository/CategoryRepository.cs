using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class CategoryRepository : ICategoryRepository
    {
        private string _connectionString;
        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Category category, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (category.IdParent == 0)
                        category.IdParent = null;

                    if(db.Category.Any(c => c.Name == category.Name && c.Type == category.Type))
                    {
                        error = "Esse nome de categoria já está cadastrado no sistema";
                        return false;
                    }

                    category.Modified = category.Created = DateTime.UtcNow;
                    db.Category.Add(category);
                    db.SaveChanges();

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = category.Id,
                        Item = (int)Enums.LogItem.Category
                    });
                    db.SaveChanges();

                    error = null;
                }
            }
            catch (Exception ex) {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a categoria. (erro:" + idLog + ")";
                if(ex.InnerException != null)
                    logError.Create("CategoryRepository.Create", ex.InnerException.Message, "INNER JOIN", ex.InnerException.StackTrace);
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
                    if (db.Category.Any(c => c.IdParent == id))
                    {
                        ret = false;
                        error = "Essa Categoria não pode ser excluida pois possui Categorias associadas a ela.";
                    }
                    else if (db.BenefitCategory.Any(c => c.IdCategory == id))
                    {
                        ret = false;
                        error = "Essa Categoria não pode ser excluida pois possui Benefícios associados a ela.";
                    }
                    else
                    {
                        var cat = db.Category.SingleOrDefault(c => c.Id == id);
                        db.Category.Remove(cat);

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = id,
                            Item = (int)Enums.LogItem.Category
                        });
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Entity.CategoryListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type, 
                                                            bool? status = null, int? idParent = null, int? level = null)
        {
            ResultPage<Entity.CategoryListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Category.Where(c => (string.IsNullOrEmpty(word) || c.Name.Contains(word))
                                    && (!status.HasValue || (status.HasValue && c.Active == status.Value))
                                    && (!idParent.HasValue || (idParent.HasValue && c.IdParent == idParent.Value))
                                    && (!type.HasValue || (type.HasValue && c.Type == type.Value))
                                    && (!level.HasValue || (level.HasValue && (level.Value == 0 && !c.IdParent.HasValue) || (level.Value == 1 && c.IdParent.HasValue))));


                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(c => c.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(c => c.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(c => c.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(c => c.Id);
                            break;
                        case "order asc":
                            tmpList = tmpList.OrderBy(c => c.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(c => c.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(c => new Entity.CategoryListItem() {
                        Id = c.Id,
                        Name = c.Name,
                        Active = c.Active,
                        Created = c.Created,
                        IdParent = c.IdParent,
                        IdType = c.Type,
                        Modified = c.Modified
                                    }).ToList();
                    var total = tmpList.Count();


                    list.ForEach(c =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Category && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        var modifiedUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.Category && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.update)
                                            .OrderByDescending(a => a.Created).FirstOrDefault();
                        if (createUser != null)
                            c.CreatedUserName = createUser.AdminUser.Name + " " + createUser.AdminUser.Surname;
                        else
                            c.CreatedUserName = " - ";
                        if (modifiedUser != null)
                            c.ModifiedUserName = modifiedUser.AdminUser.Name + " " + modifiedUser.AdminUser.Surname;
                        else
                            c.ModifiedUserName = " - ";
                    });

                    ret = new ResultPage<Entity.CategoryListItem>(list, page, pageItems, total);

                    error = null;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as categorias. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Category> ListTree(int type, bool isCustomer, int? idOperation, out string error)
        {
            List<Category> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(isCustomer && idOperation.HasValue)
                    {
                        ret = new List<Category>();
                        if(type == (int)Enums.LogItem.Benefit)
                        {
                            var listParent = db.Category.Where(c => !c.IdParent.HasValue && c.Active && c.Type == (int)Enums.LogItem.Benefit
                                && ((
                                    c.BenefitCategories.Count > 0 && c.BenefitCategories.Any(bc => bc.Benefit.Active && bc.Benefit.BenefitOperations.Any(bo => bo.IdOperation == idOperation.Value))
                                    ) || (
                                    c.Categories.Any(cc => cc.Active && cc.BenefitCategories.Count > 0 && cc.BenefitCategories.Any(bc => bc.Benefit.Active && bc.Benefit.BenefitOperations.Any(bo => bo.IdOperation == idOperation.Value))))
                                    ))
                                .OrderBy(c => c.Name).ToList();
                            foreach (var parent in listParent)
                            {
                                parent.Categories = db.Category.Where(c => c.IdParent == parent.Id && c.Active && c.BenefitCategories.Count > 0 
                                                            && c.BenefitCategories.Any(bc => bc.Benefit.Active) && c.Type == (int)Enums.LogItem.Benefit)
                                                        .OrderBy(c => c.Name).ToList();
                                ret.Add(parent);
                            }
                        }
                        else
                        {
                            var listParent = db.Category.Where(c => !c.IdParent.HasValue && c.Active && c.Type == (int)Enums.LogItem.FreeCourse
                                && ((
                                    c.FreeCourseCategories.Count > 0 && c.FreeCourseCategories.Any(bc => bc.FreeCourse.Active && bc.FreeCourse.IdOperation == idOperation.Value)
                                    ) || (
                                    c.Categories.Any(cc => cc.Active && cc.FreeCourseCategories.Count > 0 && cc.FreeCourseCategories.Any(bc => bc.FreeCourse.Active && bc.FreeCourse.IdOperation == idOperation.Value))
                                    )))
                                .OrderBy(c => c.Name).ToList();
                            foreach (var parent in listParent)
                            {
                                parent.Categories = db.Category.Where(c => c.IdParent == parent.Id && c.Active && c.FreeCourseCategories.Count > 0
                                                            && c.FreeCourseCategories.Any(bc => bc.FreeCourse.Active) && c.Type == (int)Enums.LogItem.FreeCourse)
                                                        .OrderBy(c => c.Name).ToList();
                                ret.Add(parent);
                            }
                        }
                        
                    }
                    else
                        ret = db.Category.Include("Categories").Where(c => !c.IdParent.HasValue && c.Active && c.Type == type).OrderBy(c => c.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.ListTree", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as categorias. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CategoryItem Read(int id, out string error)
        {
            CategoryItem ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var cat = db.Category.SingleOrDefault(c => c.Id == id);
                    ret = new CategoryItem()
                    {
                        Id = cat.Id,
                        Name = cat.Name,
                        Active = cat.Active,
                        Created = cat.Created,
                        IdParent = cat.IdParent,
                        Modified = cat.Modified,
                        Order = cat.Order,
                        Type = cat.Type
                    };
                    ret.HasChild = db.Category.Any(c => c.IdParent == cat.Id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler a categoria. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Category category, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Category.SingleOrDefault(c => c.Id == category.Id);
                    if(update != null)
                    {
                        if(db.Category.Any(c => c.Name == update.Name && c.Type == update.Type && c.Id != update.Id))
                        {
                            error = "Esse nome de categoria já está cadastrado no sistema.";
                            return false;
                        }

                        update.Active = category.Active;
                        if (category.IdParent == 0)
                            update.IdParent = null;
                        else
                            update.IdParent = category.IdParent;
                        update.Modified = DateTime.UtcNow;
                        update.Name = category.Name;
                        update.Order = category.Order;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = category.Id,
                            Item = (int)Enums.LogItem.Category
                        });

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Categoria não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<int> ListByBenefit(int idBenefit, out string error)
        {
            List<int> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.BenefitCategory.Where(bc => bc.IdBenefit == idBenefit).Select(bc => bc.IdCategory).ToList();
                        
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.ListByBenefit", ex.Message, $"idBenefit: {idBenefit}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as categorias vinculadas ao benefício. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Category> ListChildren(int idParent, out string error)
        {
            List<Category> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Category.Where(c => c.IdParent == idParent).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.ListChildren", ex.Message, $"idParent: {idParent}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as categorias. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<int> ListByFreeCourse(int idFreeCourse, out string error)
        {
            List<int> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.FreeCourseCategory.Where(bc => bc.IdFreeCourse == idFreeCourse).Select(bc => bc.IdCategory).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.ListByFreeCourse", ex.Message, $"idFreeCourse: {idFreeCourse}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as categorias vinculadas ao curso livre. (erro:" + idLog + ")";
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
                    var update = db.Category.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.Category,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Categoria não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
