using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class BannerRepository : IBannerRepository
    {
        private string _connectionString;
        public BannerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Banner banner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    banner.Modified = banner.Created = DateTime.UtcNow;
                    db.Banner.Add(banner);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o banner. (erro:" + idLog + ")";
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
                    var operations = db.BannerOperation.Where(b => b.IdBanner == id);
                    db.BannerOperation.RemoveRange(operations);
                    db.SaveChanges();

                    var item = db.Banner.SingleOrDefault(c => c.Id == id);
                    db.Banner.Remove(item);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o banner. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<Banner> ListByBenefit(int idBenefit, out string error)
        {
            List<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Banner.Where(b => b.IdBenefit == idBenefit && b.Active).OrderBy(f => f.Order).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListByBenefit", ex.Message, $"idBenefit: {idBenefit}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<Banner> ListByOperation(int idOperation, out string error)
        {
            List<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Banner.Where(b => b.BannerOperations.Any( o=>o.IdOperation == idOperation) && b.Active).OrderBy(f => f.Order).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Banner> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Banner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Banner.Where(b => string.IsNullOrEmpty(word) || b.Name.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "order asc":
                            tmpList = tmpList.OrderBy(f => f.Order);
                            break;
                        case "order desc":
                            tmpList = tmpList.OrderByDescending(f => f.Order);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Banner.Count(f => string.IsNullOrEmpty(word) || f.Name.Contains(word));

                    ret = new ResultPage<Banner>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os Banners. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Banner Read(int id, out string error)
        {
            Banner ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Banner.SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o banner. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Banner banner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Banner.SingleOrDefault(c => c.Id == banner.Id);
                    if (update != null)
                    {
                        update.Active = banner.Active;
                        update.BackgroundColor = banner.BackgroundColor;
                        update.End = banner.End;
                        update.IdBenefit = banner.IdBenefit;
                        update.Image = banner.Image;
                        update.Link = banner.Link;
                        update.Name = banner.Name;
                        update.Order = banner.Order;
                        update.Start = banner.Start;
                        update.Type = banner.Type;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Banner não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("BannerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o banner. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
