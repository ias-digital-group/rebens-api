using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class AdminUserRepository : IAdminUserRepository
    {
        public bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var user = db.AdminUser.SingleOrDefault(s => s.Id == id);
                    user.EncryptedPassword = passwordEncrypted;
                    user.PasswordSalt = passwordSalt;
                    user.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.ChangePassword", $"id:{id}", ex);
                error = $"Ocorreu um erro ao tentar alterar a senha do usuário. (erro:{idLog})";
                ret = false;
            }
            return ret;
        }

        public bool Create(AdminUser adminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    adminUser.Modified = adminUser.Created = DateTime.UtcNow;
                    db.AdminUser.Add(adminUser);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar o usuário. (erro:" + idLog + ")";
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
                    var user = db.AdminUser.SingleOrDefault(s => s.Id == id);
                    db.AdminUser.Remove(user);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.Delete", $"id:{id}", ex);
                error = "Ocorreu um erro ao tentar excluir o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<AdminUser> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<AdminUser> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.AdminUser.OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.AdminUser.Count();

                    ret = new ResultPage<AdminUser>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar os usuários. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public AdminUser Read(int id, out string error)
        {
            AdminUser ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.AdminUser.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.Read", $"id:{id}", ex);
                error = "Ocorreu um erro ao tentar criar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public AdminUser ReadByEmail(string email, out string error)
        {
            AdminUser ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.AdminUser.SingleOrDefault(c => c.Email == email);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.ReadByEmail", $"email:{email}", ex);
                error = "Ocorreu um erro ao tentar criar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<AdminUser> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<AdminUser> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.AdminUser.Where(p => p.Name.Contains(word)).OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.AdminUser.Count(p => p.Name.Contains(word));

                    ret = new ResultPage<AdminUser>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar os usuários. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SetLastLogin(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var user = db.AdminUser.SingleOrDefault(s => s.Id == id);
                    user.LastLogin = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.SetLastLogin", $"id:{id}", ex);
                error = "Ocorreu um erro ao tentar salvar a data de último login do usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(AdminUser adminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var update = db.AdminUser.SingleOrDefault(c => c.Id == adminUser.Id);
                    if (update != null)
                    {
                        update.Name = adminUser.Name;
                        update.Email = adminUser.Email;
                        update.LastLogin = adminUser.LastLogin;
                        update.Modified = DateTime.UtcNow;
                        update.Status = adminUser.Status;
                        update.IdProfile = adminUser.IdProfile;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "usuário não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AdminUserRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
