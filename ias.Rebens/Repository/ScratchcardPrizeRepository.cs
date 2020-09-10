using ias.Rebens.Entity;
using ias.Rebens.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class ScratchcardPrizeRepository : IScratchcardPrizeRepository
    {
        private string _connectionString;
        public ScratchcardPrizeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(ScratchcardPrize prize, int idAdminUser, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var campaign = db.Scratchcard.SingleOrDefault(c => c.Id == prize.IdScratchcard);
                    if(campaign != null)
                    {
                        if (campaign.Status == (int)Enums.ScratchcardStatus.draft || campaign.Status == (int)Enums.ScratchcardStatus.hasPrize)
                        {
                            prize.Created = prize.Modified = DateTime.UtcNow;
                            db.ScratchcardPrize.Add(prize);
                            campaign.Status = (int)Enums.ScratchcardStatus.hasPrize;
                            db.SaveChanges();

                            db.LogAction.Add(new LogAction()
                            {
                                Action = (int)Enums.LogAction.create,
                                Created = DateTime.UtcNow,
                                IdAdminUser = idAdminUser,
                                IdItem = prize.Id,
                                Item = (int)Enums.LogItem.ScratchcardPrize
                            });
                            db.SaveChanges();

                            ret = true;
                            error = null;
                        }
                        else
                            error = "Não é possível criar prêmio para esta campanha!";
                        
                    }
                    else
                        error = "Campanha não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardPrizeRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o prêmio. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ScratchcardPrize.SingleOrDefault(p => p.Id == id);
                    if (update != null)
                    {
                        db.ScratchcardPrize.Remove(update);
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = update.Id,
                            Item = (int)Enums.LogItem.ScratchcardPrize
                        });
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Prêmio não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardPrizeRepository.Delete", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar o prêmio. (erro:" + idLog + ")";
            }
            return ret;
        }

        public List<ScratchcardPrize> List(int idScratchcard, out string error)
        {
            List<ScratchcardPrize> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ScratchcardPrize.Where(p => p.IdScratchcard == idScratchcard).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardPrizeRepository.List", ex.Message, $"idScratchcard:{idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os prêmios. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<ScratchcardPrizeListItem> ListPage(int page, int pageItems, string searchWord, out string error, int? idOperation = null, int? idScratchcard = null)
        {
            ResultPage<ScratchcardPrizeListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmplist = db.ScratchcardPrize.Include("Scratchcard").Where(s => (string.IsNullOrEmpty(searchWord)
                                                                    || s.Title.Contains(searchWord)
                                                                    || s.Name.Contains(searchWord)
                                                                    || s.Description.Contains(searchWord))
                                                            && (!idOperation.HasValue || s.Scratchcard.IdOperation == idOperation)
                                                            && (!idScratchcard.HasValue || s.IdScratchcard == idScratchcard)
                                                       );
                    var total = tmplist.Count();
                    var list = tmplist.OrderBy(s => s.Title)
                                        .Skip(page * pageItems)
                                        .Take(pageItems);

                    var result = new List<ScratchcardPrizeListItem>();
                    foreach (var item in list)
                    {
                        var prize = new ScratchcardPrizeListItem()
                        {
                            Id = item.Id,
                            Created = item.Created,
                            IdScratchcard = item.IdScratchcard,
                            CampaignName = item.Scratchcard.Name,
                            IdOperation = item.Scratchcard.IdOperation,
                            Prize = item.Title,
                            Quantity = item.Quantity,
                            CanEdit = item.Scratchcard.Status == (int)ScratchcardStatus.draft || item.Scratchcard.Status == (int)ScratchcardStatus.hasPrize
                        };
                        prize.OperationName = db.Operation.Single(o => o.Id == prize.IdOperation).Title;
                        var createdUser = db.LogAction.Include("AdminUser").SingleOrDefault(a => a.IdItem == item.Id
                                                && a.Action == (int)Enums.LogAction.create
                                                && a.Item == (int)LogItem.ScratchcardPrize);
                        if (createdUser != null && createdUser.AdminUser != null)
                            prize.CreatedBy = createdUser.AdminUser.Name + " " + createdUser.AdminUser.Surname;
                        result.Add(prize);
                    }

                    ret = new ResultPage<ScratchcardPrizeListItem>(result, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardPrizeRepository.ListPage", ex.Message, $"idScratchcard:{idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os prêmios. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ScratchcardPrize Read(int id, out string error)
        {
            ScratchcardPrize ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ScratchcardPrize.Include("Scratchcard").SingleOrDefault(p => p.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardPrizeRepository.Read", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os prêmios. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(ScratchcardPrize prize, int idAdminUser, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ScratchcardPrize.SingleOrDefault(p => p.Id == prize.Id);
                    if (update != null)
                    {
                        update.Description = prize.Description;
                        update.Image = prize.Image;
                        update.Modified = DateTime.UtcNow;
                        update.Name = prize.Name;
                        update.Quantity = prize.Quantity;
                        update.Title = prize.Title;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = update.Id,
                            Item = (int)Enums.LogItem.ScratchcardPrize
                        });
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Prêmio não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardPrizeRepository.Update", ex.Message, $"id:{prize.Id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o prêmio. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
