using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace ias.Rebens
{
    public class ScratchcardRepository : IScratchcardRepository
    {
        private string _connectionString;
        public ScratchcardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Scratchcard scratchcard, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    scratchcard.Modified = scratchcard.Created = DateTime.UtcNow;
                    scratchcard.Status = (int)Enums.ScratchcardStatus.draft;
                    db.Scratchcard.Add(scratchcard);
                    db.SaveChanges();

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = scratchcard.Id,
                        Item = (int)Enums.LogItem.Scratchcard
                    });
                    db.SaveChanges();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar a raspadinha. (erro:" + idLog + ")";
                ret = false;
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
                    var update = db.Scratchcard.SingleOrDefault(s => s.Id == id);
                    if (update.Status == (int)Enums.ScratchcardStatus.draft || !db.ScratchcardDraw.Any(s => s.IdCustomer.HasValue))
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = id,
                            Item = (int)Enums.LogItem.Scratchcard
                        });

                        update.Modified = DateTime.UtcNow;
                        update.Status = (int)Enums.ScratchcardStatus.deleted;
                        db.SaveChanges();

                        ret = true;
                        error = null;
                    }
                    else
                        error = "A campanha não pode ser apagada pois possui raspadinhas distribuidas";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar a raspadinha. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public async Task<bool> GenerateScratchcards(int id, int idAdminUser, string path)
        {
            bool ret = false;
            try
            {
                using(var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.UtcNow;
                    var scratchcard = db.Scratchcard.SingleOrDefault(s => s.Id == id && s.Status == (int)Enums.ScratchcardStatus.draft);
                    
                    if(scratchcard != null)
                    {
                        scratchcard.Status = (int)Enums.ScratchcardStatus.generating;
                        scratchcard.Modified = DateTime.UtcNow;
                        await db.LogAction.AddAsync(new LogAction()
                        {
                            Action = (int)Enums.LogAction.generate,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = id,
                            Item = (int)Enums.LogItem.Scratchcard
                        });
                        await db.SaveChangesAsync();

                        var timeout = db.Database.GetCommandTimeout();
                        db.Database.SetCommandTimeout(600);
                        await db.Database.ExecuteSqlCommandAsync($"dbo.GenerateScratchcardDraw {id}");
                        db.Database.SetCommandTimeout(timeout);


                        scratchcard.Status = (int)Enums.ScratchcardStatus.generated;
                        scratchcard.Modified = DateTime.UtcNow;
                        await db.SaveChangesAsync();

                        var admin = db.AdminUser.Single(a => a.Id == idAdminUser);
                        var listDestinataries = new Dictionary<string, string>() { { admin.Email, admin.Name } };
                        Helper.EmailHelper.SendAdminEmail(listDestinataries, "[REBENS] - Geração das raspadinhas concluído.", $"O processo de geração das raspadinhas {scratchcard.Name}, foi concluído com sucesso.", out _);
                    }
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("ScratchcardRepository.GenerateScratchcards", ex.Message, $"id: {id}", ex.StackTrace);
                
            }
            return ret;
        }

        public ResultPage<Scratchcard> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation)
        {
            ResultPage<Scratchcard> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Scratchcard.Where(s => s.Status != (int)Enums.ScratchcardStatus.deleted 
                                    && (string.IsNullOrWhiteSpace(word) || s.Name.Contains(word))
                                    && (!idOperation.HasValue || s.IdOperation == idOperation.Value));
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
                        case "start asc":
                            tmpList = tmpList.OrderBy(f => f.Start);
                            break;
                        case "start desc":
                            tmpList = tmpList.OrderByDescending(f => f.Start);
                            break;
                        case "end asc":
                            tmpList = tmpList.OrderBy(f => f.End);
                            break;
                        case "end desc":
                            tmpList = tmpList.OrderByDescending(f => f.End);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<Scratchcard>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardRepository.ListPage", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Scratchcard Read(int id, out string error)
        {
            Scratchcard ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Scratchcard.SingleOrDefault(s => s.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Scratchcard scratchcard, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Scratchcard.SingleOrDefault(s => s.Id == scratchcard.Id);
                    if(update != null)
                    {
                        update.Modified = DateTime.UtcNow;
                        update.End = scratchcard.End;
                        update.Name = scratchcard.Name;
                        update.NoPrizeImage1 = scratchcard.NoPrizeImage1;
                        update.NoPrizeImage2 = scratchcard.NoPrizeImage2;
                        update.NoPrizeImage3 = scratchcard.NoPrizeImage3;
                        update.NoPrizeImage4 = scratchcard.NoPrizeImage4;
                        update.NoPrizeImage5 = scratchcard.NoPrizeImage5;
                        update.NoPrizeImage6 = scratchcard.NoPrizeImage6;
                        update.NoPrizeImage7 = scratchcard.NoPrizeImage7;
                        update.NoPrizeImage8 = scratchcard.NoPrizeImage8;
                        update.Type = scratchcard.Type;
                        update.DistributionType = scratchcard.DistributionType;
                        update.DistributionQuantity = scratchcard.DistributionQuantity;
                        update.ScratchcardExpire = scratchcard.ScratchcardExpire;
                        update.Quantity = scratchcard.Quantity;
                        update.Start = scratchcard.Start;
                        if(scratchcard.Status != 0)
                            update.Status = scratchcard.Status;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.update,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = scratchcard.Id,
                            Item = (int)Enums.LogItem.Scratchcard
                        });
                        db.SaveChanges();
                    }

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardRepository.Update", ex.Message, "", ex.StackTrace);
                if(ex.InnerException != null)
                    logError.Create("ScratchcardRepository.Update - INNER", ex.InnerException.Message, "", ex.InnerException.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a raspadinha. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public Scratchcard Read(int id, out bool canPublish, out string error)
        {
            Scratchcard ret;
            canPublish = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Scratchcard.SingleOrDefault(s => s.Id == id);

                    if (ret != null)
                    {
                        canPublish = ret.Status == (int)Enums.ScratchcardStatus.draft;
                        if (string.IsNullOrEmpty(ret.NoPrizeImage1))
                            canPublish = false;
                        if (!db.ScratchcardPrize.Any(p => p.IdScratchcard == id))
                            canPublish = false;
                        if (ret.Quantity <= 0)
                            canPublish = false;
                        if (ret.DistributionType != (int)Enums.ScratchcardDistribution.daily && ret.DistributionQuantity <= 0)
                            canPublish = false;
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
