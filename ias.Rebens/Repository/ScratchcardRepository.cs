using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

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
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.delete,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = id,
                        Item = (int)Enums.LogItem.Scratchcard
                    });

                    var update = db.Scratchcard.SingleOrDefault(s => s.Id == id);
                    update.Modified = DateTime.UtcNow;
                    update.Status = (int)Enums.ScratchcardStatus.deleted;
                    db.SaveChanges();

                    error = null;
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

                        int count = 0;
                        var prizes = db.ScratchcardPrize.Where(s => s.IdScratchcard == id);
                        var images = prizes.Select(p => p.Image).ToList();
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage1))
                            images.Add(scratchcard.NoPrizeImage1);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage2))
                            images.Add(scratchcard.NoPrizeImage2);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage3))
                            images.Add(scratchcard.NoPrizeImage3);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage4))
                            images.Add(scratchcard.NoPrizeImage4);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage5))
                            images.Add(scratchcard.NoPrizeImage5);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage6))
                            images.Add(scratchcard.NoPrizeImage6);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage7))
                            images.Add(scratchcard.NoPrizeImage7);
                        if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage8))
                            images.Add(scratchcard.NoPrizeImage8);

                        var cloudinary = new Integration.CloudinaryHelper();
                        foreach (var prize in prizes)
                        {
                            count += prize.Quantity;
                            for(int i = 0; i<prize.Quantity; i ++)
                            {
                                string fileName = $"prize-{prize.IdScratchcard}-{prize.Id}-{i}.png";
                                Helper.ScratchcardHelper.GeneratePrize(prize.Image, images.Where(img => img != prize.Image).ToList(), path, fileName);
                                var cloudinaryModel = cloudinary.UploadFile(Path.Combine(path, fileName), "Scratchcard");
                                await db.ScratchcardDraw.AddAsync(new ScratchcardDraw()
                                {
                                    Created = DateTime.UtcNow,
                                    IdScratchcard = prize.IdScratchcard,
                                    IdScratchcardPrize = prize.Id,
                                    Image = cloudinaryModel.secure_url,
                                    Modified = DateTime.UtcNow,
                                    Prize = prize.Title,
                                    Status = (int)Enums.ScratchcardDraw.active,
                                    ValidationCode = Helper.SecurityHelper.GenerateCode(20)
                                });
                                File.Delete(Path.Combine(path, fileName));
                            }
                        }
                        await db.SaveChangesAsync();

                        for (var i = 0; i<(scratchcard.Quantity - count); i++)
                        {
                            string fileName = $"noprize-{scratchcard.Id}-{i}.png";
                            Helper.ScratchcardHelper.GenerateNoPrize(images, path, fileName);
                            var cloudinaryModel = cloudinary.UploadFile(Path.Combine(path, fileName), "Scratchcard");
                            await db.ScratchcardDraw.AddAsync(new ScratchcardDraw()
                            {
                                Created = DateTime.UtcNow,
                                IdScratchcard = id,
                                Image = cloudinaryModel.secure_url,
                                Modified = DateTime.UtcNow,
                                Status = (int)Enums.ScratchcardDraw.active,
                                ValidationCode = Helper.SecurityHelper.GenerateCode(20)
                            });
                            File.Delete(Path.Combine(path, fileName));
                        }
                        await db.SaveChangesAsync();
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
                error = "Ocorreu um erro ao tentar atualizar a raspadinha. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
