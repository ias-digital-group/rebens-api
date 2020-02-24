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
                    prize.Created = prize.Modified = DateTime.UtcNow;
                    db.ScratchcardPrize.Add(prize);
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
