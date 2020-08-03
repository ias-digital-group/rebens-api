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

        public bool Create(AdminUser adminUser, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (db.AdminUser.Any(a => a.Email == adminUser.Email || a.Doc == adminUser.Doc))
                    {
                        error = "Este Cpf ou e-mail já está cadastrado na nossa base.";
                        ret = false;
                    }
                    else
                    {
                        adminUser.Modified = adminUser.Created = DateTime.UtcNow;
                        adminUser.EncryptedPassword = adminUser.PasswordSalt = "";
                        adminUser.Deleted = false;
                        adminUser.Active = true;

                        db.AdminUser.Add(adminUser);
                        db.SaveChanges();

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.create,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = adminUser.Id,
                            Item = (int)Enums.LogItem.AdminUser
                        });
                        db.SaveChanges();

                        error = null;
                    }
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

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var user = db.AdminUser.SingleOrDefault(s => s.Id == id);
                    user.Deleted = true;
                    user.Modified = DateTime.UtcNow;

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.delete,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = id,
                        Item = (int)Enums.LogItem.AdminUser
                    });

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

        public ResultPage<AdminUser> ListPage(string userRole, int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? status = null, string role = null, int? idOperationPartner = null)
        {
            ResultPage<AdminUser> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.AdminUser.Include("Operation").Include("OperationPartner").Where(a => !a.Deleted
                                && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Surname.Contains(word) || a.Email.Contains(word))
                                && (!idOperation.HasValue || (idOperation.HasValue && a.IdOperation == idOperation.Value))
                                && (!status.HasValue || (status.HasValue && a.Active == status.Value))
                                && (!idOperationPartner.HasValue || a.IdOperationPartner == idOperationPartner.Value)
                                && (string.IsNullOrEmpty(role) || a.Roles == role)
                                && (userRole == "master" || a.Roles != "master")
                                ); ;
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(a => a.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(a => a.Name);
                            break;
                        case "surname asc":
                            tmpList = tmpList.OrderBy(a => a.Surname);
                            break;
                        case "surname desc":
                            tmpList = tmpList.OrderByDescending(a => a.Surname);
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
                    var total = tmpList.Count();

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
                    ret = db.AdminUser.SingleOrDefault(a => a.Id == id && !a.Deleted);
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

        public AdminUser ReadFull(int id, out string error)
        {
            AdminUser ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.AdminUser.Include("Operation").Include("OperationPartner").SingleOrDefault(a => a.Id == id && !a.Deleted);
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
                    ret = db.AdminUser.SingleOrDefault(a => a.Email == email && !a.Deleted);
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

        public bool ToggleActive(int id, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.AdminUser.SingleOrDefault(a => a.Id == id);
                    if(update != null)
                    {
                        ret = !update.Active;
                        update.Active = ret;
                        update.Modified = DateTime.UtcNow;
                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.activate : (int)Enums.LogAction.inactivate,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.AdminUser,
                            IdItem = id,
                            IdAdminUser = idAdminUser
                        });
                        db.SaveChanges();
                        error = null;   
                    }
                    else
                    {
                        ret = false;
                        error = "Usuário não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(AdminUser adminUser, int idAdminUser, out string error)
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
                        update.Surname = adminUser.Surname;
                        update.Email = adminUser.Email;
                        update.Modified = DateTime.UtcNow;
                        update.Active = adminUser.Active;
                        update.Doc = adminUser.Doc;
                        update.Picture = adminUser.Picture;
                        update.PhoneComercial = adminUser.PhoneComercial;
                        update.PhoneComercialBranch = adminUser.PhoneComercialBranch;
                        update.PhoneComercialMobile = adminUser.PhoneComercialMobile;
                        update.PhoneMobile = adminUser.PhoneMobile;

                        if(!string.IsNullOrEmpty(adminUser.Roles))
                            update.Roles = adminUser.Roles;
                        if (adminUser.Roles == "partnerApprover" || adminUser.Roles == "partnerAdministrator")
                            update.IdOperationPartner = adminUser.IdOperationPartner;
                        else
                            update.IdOperationPartner = null;

                        if (adminUser.Roles == Enums.Roles.couponChecker.ToString())
                            update.IdPartner = adminUser.IdPartner;
                        else
                            update.IdPartner = null;

                        if (adminUser.Roles != Enums.Roles.master.ToString() && adminUser.Roles != Enums.Roles.administratorRebens.ToString() &&
                            adminUser.Roles != Enums.Roles.publisherRebens.ToString() && adminUser.Roles != Enums.Roles.customer.ToString())
                            update.IdOperation = adminUser.IdOperation;
                        else
                            update.IdOperation = null;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = adminUser.Id,
                            Item = (int)Enums.LogItem.AdminUser
                        });

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

        public bool CheckCPF(int id, string cpf, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (db.AdminUser.Any(a => a.Doc == cpf && a.Id != id))
                    {
                        ret = false;
                        error = "Este CPF já está cadastrado em nossa base";
                    }
                    else
                    {
                        ret = true;
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.CheckCPF", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar verificar se o cpf já está cadastrado na base. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool CheckEmail(int id, string email, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (db.AdminUser.Any(a => a.Email == email && a.Id != id))
                    {
                        ret = false;
                        error = "Este E-mail já está cadastrado em nossa base";
                    }
                    else
                    {
                        ret = true;
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AdminUserRepository.CheckEmail", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar verificar se o Email já está cadastrado na base. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

    }
}
