using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class PermissionRepository : IPermissionRepository
    {
        private string _connectionString;
        public PermissionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Permission permission, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    permission.Modified = permission.Created = DateTime.UtcNow;
                    db.Permission.Add(permission);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar a permissão. (erro:" + idLog + ")";
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
                    var permission = db.Permission.SingleOrDefault(s => s.Id == id);
                    db.Permission.Remove(permission);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.Delete", ex);
                error = "Ocorreu um erro ao tentar excluir a permissão. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Permission> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<Permission> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.Permission.OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Permission.Count();

                    ret = new ResultPage<Permission>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar as permissões. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Permission> ListTree(out string error)
        {
            List<Permission> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Permission.Include("Permissions").Where(p => !p.IdParent.HasValue).OrderBy(p => p.Name).ToList();
                    error = null;
                }
            }
            catch(Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.ListTree", ex);
                error = "Ocorreu um erro ao tentar listar a árvore de permissões. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Permission Read(int id, out string error)
        {
            Permission ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Permission.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler a permissão. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Permission> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<Permission> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.Permission.Where(p => p.Name.Contains(word)).OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Permission.Count(p => p.Name.Contains(word));

                    ret = new ResultPage<Permission>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar as permissões. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Permission Permission, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Permission.SingleOrDefault(c => c.Id == Permission.Id);
                    if (update != null)
                    {
                        update.Modified = DateTime.UtcNow;
                        update.Name = Permission.Name;
                        update.IdParent = Permission.IdParent;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "permissão não encontrada!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("PermissionRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar a permissão. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
