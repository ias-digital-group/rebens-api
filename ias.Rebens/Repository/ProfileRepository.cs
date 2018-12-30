using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class ProfileRepository : IProfileRepository
    {
        private string _connectionString;
        public ProfileRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Profile profile, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    profile.Modified = profile.Created = DateTime.UtcNow;
                    db.Profile.Add(profile);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ProfileRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar o perfil. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, out string error)
        {
            //bool ret = true;
            //try
            //{
            //    using (var db = new RebensContext(this._connectionString))
            //    {
            //        if (db.AdminUser.Any(c => c.IdProfile == id))
            //        {
            //            ret = false;
            //            error = "Esse perfil não pode ser excluido pois possui usuários associadas a ele.";
            //        }
            //        else
            //        {
            //            var type = db.Profile.SingleOrDefault(s => s.Id == id);
            //            db.Profile.Remove(type);
            //            db.SaveChanges();
            //            error = null;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    int idLog = Helper.LogHelper.Add("ProfileRepository.Delete", ex);
            //    error = "Ocorreu um erro ao tentar excluir o perfil. (erro:" + idLog + ")";
            //    ret = false;
            //}
            //return ret;
            throw new NotImplementedException();
        }

        public ResultPage<Profile> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<Profile> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.Profile.OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Profile.Count();

                    ret = new ResultPage<Profile>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ProfileRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar os perfils. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Profile Read(int id, out string error)
        {
            Profile ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Profile.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ProfileRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler o perfil. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Profile> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<Profile> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.Profile.Where(p => p.Name.Contains(word)).OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Profile.Count(p => p.Name.Contains(word));

                    ret = new ResultPage<Profile>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ProfileRepository.List", ex);
                error = "Ocorreu um erro ao tentar listar os perfils. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Profile profile, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Profile.SingleOrDefault(c => c.Id == profile.Id);
                    if (update != null)
                    {
                        update.Active = profile.Active;
                        update.Modified = DateTime.UtcNow;
                        update.Name = profile.Name;
                        update.Permissions = profile.Permissions;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "perfil não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("ProfileRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar o perfil. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
