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
        public bool Create(Category category, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    category.Modified = category.Created = DateTime.UtcNow;
                    db.Category.Add(category);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex) {
                int idLog = Helper.LogHelper.Add("CategoryRepository.Create", ex);
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
                using (var db = new RebensContext())
                {
                    if (db.Category.Any(c => c.IdParent == id))
                    {
                        ret = false;
                        error = "Essa Categoria não pode ser excluida pois possui Categorias associadas a ela.";
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
                int idLog = Helper.LogHelper.Add("CategoryRepository.Delete", ex);
                error = "Ocorreu um erro ao tentar excluir a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Category> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<Category> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Category.OrderBy(c => c.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Category.Count();

                    ret = new ResultPage<Category>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("CategoryRepository.ListPage", ex);
                error = "Ocorreu um erro ao tentar listar as categorias. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Category> ListTree(out string error)
        {
            List<Category> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.Category.Include("Categories").Where(c => !c.IdParent.HasValue && c.Active).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("CategoryRepository.ListTree", ex);
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
                using (var db = new RebensContext())
                {
                    ret = db.Category.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("CategoryRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler a categoria. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Category> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<Category> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Category.Where(c => c.Name.Contains(word)).OrderBy(c => c.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Category.Count(c => c.Name.Contains(word));

                    ret = new ResultPage<Category>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("CategoryRepository.SearchPage", ex);
                error = "Ocorreu um erro ao tentar listar as categorias. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Category category, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
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
                int idLog = Helper.LogHelper.Add("CategoryRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
