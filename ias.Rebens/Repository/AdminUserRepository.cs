using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private string _connectionString;
        public AdminUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
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
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.ChangePassword", ex.Message, $"id:{id}", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    adminUser.Modified = adminUser.Created = DateTime.UtcNow;
                    db.AdminUser.Add(adminUser);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.Create", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    var user = db.AdminUser.SingleOrDefault(s => s.Id == id);
                    db.AdminUser.Remove(user);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.Delete", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<AdminUser> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<AdminUser> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.AdminUser.Where(a => string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(a => a.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(a => a.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(a => a.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(a => a.Id);
                            break;
                        case "email asc":
                            tmpList = tmpList.OrderBy(a => a.Email);
                            break;
                        case "email desc":
                            tmpList = tmpList.OrderByDescending(a => a.Email);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.AdminUser.Count(a => string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Email.Contains(word));

                    ret = new ResultPage<AdminUser>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.List", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.AdminUser.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.Read", ex.Message, $"id:{id}", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.AdminUser.SingleOrDefault(c => c.Email == email);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.ReadByEmail", ex.Message, $"email:{email}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o usuário. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SetLastLogin(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var user = db.AdminUser.SingleOrDefault(s => s.Id == id);
                    user.LastLogin = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.SetLastLogin", ex.Message, $"id:{id}", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.AdminUser.SingleOrDefault(c => c.Id == adminUser.Id);
                    if (update != null)
                    {
                        update.Name = adminUser.Name;
                        update.Email = adminUser.Email;
                        update.LastLogin = adminUser.LastLogin;
                        update.Modified = DateTime.UtcNow;
                        update.Status = adminUser.Status;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "usuário não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
