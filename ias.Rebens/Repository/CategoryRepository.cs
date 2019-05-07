﻿using Microsoft.EntityFrameworkCore;
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

        public bool Create(Category category, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    category.Modified = category.Created = DateTime.UtcNow;
                    db.Category.Add(category);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex) {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CategoryRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a categoria. (erro:" + idLog + ")";
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

        public ResultPage<Category> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null, int? idParent = null)
        {
            ResultPage<Category> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Category.Where(c => (string.IsNullOrEmpty(word) || c.Name.Contains(word))
                                    && (!status.HasValue || (status.HasValue && c.Active == status.Value))
                                    && (!idParent.HasValue || (idParent.HasValue && c.IdParent == idParent.Value)));

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

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Category.Count(c => (string.IsNullOrEmpty(word) || c.Name.Contains(word))
                                    && (!status.HasValue || (status.HasValue && c.Active == status.Value))
                                    && (!idParent.HasValue || (idParent.HasValue && c.IdParent == idParent.Value)));

                    ret = new ResultPage<Category>(list, page, pageItems, total);

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

        public List<Category> ListTree(bool isCustomer, int? idOperation, out string error)
        {
            List<Category> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(isCustomer && idOperation.HasValue)
                    {
                        ret = new List<Category>();
                        var listParent = db.Category.Where(c => !c.IdParent.HasValue && c.Active 
                                && ((
                                    c.BenefitCategories.Count > 0 && c.BenefitCategories.Any(bc => bc.Benefit.BenefitOperations.Any(bo => bo.IdOperation == idOperation.Value))
                                    ) || (
                                    c.Categories.Any(cc => cc.Active && cc.BenefitCategories.Count > 0 && cc.BenefitCategories.Any(bc => bc.Benefit.BenefitOperations.Any(bo => bo.IdOperation == idOperation.Value))))
                                    ))
                                .OrderBy(c => c.Name).ToList();
                        foreach(var parent in listParent)
                        {
                            parent.Categories = db.Category.Where(c => c.IdParent == parent.Id && c.Active && c.BenefitCategories.Count > 0).OrderBy(c => c.Name).ToList();
                            ret.Add(parent);
                        }
                    }
                    else
                        ret = db.Category.Include("Categories").Where(c => !c.IdParent.HasValue && c.Active).OrderBy(c => c.Name).ToList();
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

        public Category Read(int id, out string error)
        {
            Category ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Category.SingleOrDefault(c => c.Id == id);
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

        public bool Update(Category category, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Category.SingleOrDefault(c => c.Id == category.Id);
                    if(update != null)
                    {
                        update.Active = category.Active;
                        update.Icon = category.Icon;
                        update.IdParent = category.IdParent;
                        update.Modified = DateTime.UtcNow;
                        update.Name = category.Name;
                        update.Order = category.Order;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Categoria não encontrada!";
                    }
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
    }
}
