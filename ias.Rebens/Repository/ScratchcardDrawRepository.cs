using ias.Rebens.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class ScratchcardDrawRepository : IScratchcardDrawRepository
    {
        private string _connectionString;
        public ScratchcardDrawRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }


        public ResultPage<ScratchcardDraw> ListByScratchcard(int idScratchcard, int page, int pageItems, out string error)
        {
            ResultPage<ScratchcardDraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.ScratchcardDraw.Where(s => s.IdScratchcard == idScratchcard)
                                    .OrderBy(s => s.Created).ToList();

                    ret = new ResultPage<ScratchcardDraw>(list, page, list.Count, list.Count);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.ListByScratchcard", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<ScratchcardDraw> ListByCustomer(int idCustomer, int page, int pageItems, out string error)
        {
            ResultPage<ScratchcardDraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.ScratchcardDraw.Include("Scratchcard").Include("ScratchcardPrize")
                                        .Where(s => s.IdCustomer == idCustomer)
                                        .OrderByDescending(s => s.Date)
                                        .Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.ScratchcardDraw.Count(s => s.IdCustomer == idCustomer);

                    ret = new ResultPage<ScratchcardDraw>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.ListByCustomer", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }


        public bool SaveRandom(int idScratchcard, string path, int idCustomer, DateTime date, DateTime? expireDate, out string error)
        {
            bool ret = false;
            try
            {
                var cloudinary = new Integration.CloudinaryHelper();
                using (var db = new RebensContext(this._connectionString))
                {
                    var item = db.ScratchcardDraw.Where(s => s.IdScratchcard == idScratchcard && !s.IdCustomer.HasValue).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                    var scratchcard = db.Scratchcard.Single(s => s.Id == idScratchcard);
                    var prizes = db.ScratchcardPrize.Where(s => s.IdScratchcard == idScratchcard).ToList();
                    var images = prizes.Select(p => p.Image).ToList();
                    var noPrizeImages = new List<string>();
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage1))
                        noPrizeImages.Add(scratchcard.NoPrizeImage1);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage2))
                        noPrizeImages.Add(scratchcard.NoPrizeImage2);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage3))
                        noPrizeImages.Add(scratchcard.NoPrizeImage3);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage4))
                        noPrizeImages.Add(scratchcard.NoPrizeImage4);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage5))
                        noPrizeImages.Add(scratchcard.NoPrizeImage5);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage6))
                        noPrizeImages.Add(scratchcard.NoPrizeImage6);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage7))
                        noPrizeImages.Add(scratchcard.NoPrizeImage7);
                    if (!string.IsNullOrEmpty(scratchcard.NoPrizeImage8))
                        noPrizeImages.Add(scratchcard.NoPrizeImage8);

                    string fileName = $"scratchcard-{item.IdScratchcard}-{item.Id}.png";
                    if (item.IdScratchcardPrize.HasValue)
                    {
                        var prize = prizes.Single(p => p.Id == item.IdScratchcardPrize.Value);
                        Helper.ScratchcardHelper.GeneratePrize(prize.Image, images.Where(img => img != prize.Image).ToList(), noPrizeImages, path, fileName);
                    } 
                    else
                    {
                        images.AddRange(noPrizeImages);
                        if (images.Count < 9)
                        {
                            for (int j = images.Count; j < 10; j++)
                                images.Add(noPrizeImages.First());
                        }
                        Helper.ScratchcardHelper.GenerateNoPrize(images, path, fileName);
                    }
                    var cloudinaryModel = cloudinary.UploadFile(Path.Combine(path, fileName), "Scratchcard");
                    item.Modified = DateTime.UtcNow;
                    item.IdCustomer = idCustomer;
                    item.Date = date;
                    item.ExpireDate = expireDate;
                    item.ValidationCode = Helper.SecurityHelper.GenerateCode(20);
                    item.Status = (int)Enums.ScratchcardDraw.drawn;
                    item.Image = cloudinaryModel.secure_url;

                    db.SaveChanges();

                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.LoadRandom", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
            }
            return ret;
        }

        public ScratchcardDraw Read(int id, out string error)
        {
            ScratchcardDraw ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ScratchcardDraw.SingleOrDefault(s => s.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SetOpened(int id, int idCustomer)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ScratchcardDraw.SingleOrDefault(s => s.Id == id);
                    if(update != null && update.IdCustomer == idCustomer)
                    {
                        if (!update.OpenDate.HasValue)
                        {
                            update.OpenDate = DateTime.UtcNow;
                            update.Modified = DateTime.UtcNow;
                            db.SaveChanges();
                        }
                        ret = true;
                    }
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("ScratchcardDrawRepository.SetOpened", ex.Message, $"id: {id}, idCustomer: {idCustomer}", ex.StackTrace);
            }
            return ret;
        }

        public bool SetPlayed(int id, int idCustomer, int idOperation)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ScratchcardDraw.SingleOrDefault(s => s.Id == id);
                    if (update != null && update.IdCustomer == idCustomer)
                    {
                        update.PlayedDate = DateTime.UtcNow;
                        update.Status = (int)Enums.ScratchcardDraw.scratched;
                        update.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        if (update.IdScratchcardPrize.HasValue)
                        {
                            var staticText = db.StaticText.Where(t => t.IdOperation == idOperation && t.IdStaticTextType == (int)Enums.StaticTextType.Email && t.Active).OrderByDescending(t => t.Modified).FirstOrDefault();
                            if (staticText != null)
                            {
                                var customer = db.Customer.Single(c => c.Id == idCustomer);
                                var scratchcard = db.Scratchcard.Single(s => s.Id == update.IdScratchcard);

                                string message = $"<p>Olá {customer.Name},</p><p>Você acabou de ganhar {update.Prize} na raspadinha premiada ({update.ValidationCode}).</p>";
                                message += $"<img src=\"{update.Image}\" alt=\"{update.ValidationCode}\" /><br /><br />";
                                if(!string.IsNullOrEmpty(scratchcard.Instructions))
                                    message += $"<p><b>Instruções para o resgate:</b><br />{scratchcard.Instructions}</p>";
                                string body = staticText.Html.Replace("###BODY###", message);
                                Helper.EmailHelper.SendDefaultEmail(customer.Email, customer.Name, "contato@rebens.com.br", "Rebens", "Raspadinha Premiada", body, out _);

                                if (scratchcard.GetNotifications) 
                                {
                                    var admin = db.AdminUser.Single(a => a.Id == scratchcard.IdAdminUser);
                                    message = $"<p>Tivemos uma raspadinha premiada hoje: <b>{scratchcard.Instructions}</b></p><img src=\"{update.Image}\" alt=\"{update.ValidationCode}\" />";
                                    body = staticText.Html.Replace("###BODY###", message);
                                    Helper.EmailHelper.SendDefaultEmail(admin.Email, admin.Name, "contato@rebens.com.br", "Rebens", "Raspadinha Premiada", body, out _);
                                }
                            }
                        }
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("ScratchcardDrawRepository.SetPlayed", ex.Message, $"id: {id}, idCustomer: {idCustomer}", ex.StackTrace);
            }
            return ret;
        }

        public bool Validate(int id, int idCustomer)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ScratchcardDraw.SingleOrDefault(s => s.Id == id);
                    if (update != null && update.IdCustomer == idCustomer)
                    {
                        update.ValidationDate = DateTime.UtcNow;
                        update.Status = (int)Enums.ScratchcardDraw.validated;
                        update.Modified = DateTime.UtcNow;
                        db.SaveChanges();

                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("ScratchcardDrawRepository.Validate", ex.Message, $"id: {id}, idCustomer: {idCustomer}", ex.StackTrace);
            }
            return ret;
        }

        public ResultPage<ScratchcardDrawListItem> ScratchedWithPrizeListPage(int page, int pageItems, string searchWord, int? idOperation, int? idScratchcard, out string error)
        {
            ResultPage<ScratchcardDrawListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmplist = db.ScratchcardDraw.Include("Scratchcard").Where(s => (string.IsNullOrEmpty(searchWord)
                                                                    || s.Customer.Name.Contains(searchWord)
                                                                    || s.Customer.Surname.Contains(searchWord)
                                                                    || s.Customer.Email.Contains(searchWord)
                                                                    || s.Customer.Cpf.Contains(searchWord)
                                                                    )
                                                            && (!idOperation.HasValue || s.Scratchcard.IdOperation == idOperation)
                                                            && (!idScratchcard.HasValue || s.IdScratchcard == idScratchcard)
                                                            && s.IdCustomer.HasValue
                                                            && s.IdScratchcardPrize.HasValue
                                                            && s.Status == (int)Enums.ScratchcardDraw.scratched
                                                            && s.PlayedDate.HasValue
                                                       );

                    var total = tmplist.Count();
                    var list = tmplist.OrderByDescending(s => s.IdScratchcardPrize)
                                        .ThenByDescending(s => s.IdCustomer)
                                        .Skip(page * pageItems)
                                        .Take(pageItems);
                    var result = new List<ScratchcardDrawListItem>();
                    foreach (var item in list)
                    {
                        var billet = new ScratchcardDrawListItem()
                        {
                            Id = item.Id,
                            Date = item.Date,
                            IdCustomer = item.IdCustomer,
                            IdScratchcard = item.IdScratchcard,
                            IdPrize = item.IdScratchcardPrize,
                            ValidationCode = item.ValidationCode,
                            ValidationDate = item.ValidationDate,
                            CampaignName = item.Scratchcard.Name,
                            IdOperation = item.Scratchcard.IdOperation
                        };
                        billet.OperationName = db.Operation.Single(o => o.Id == billet.IdOperation).Title;
                        var customer = db.Customer.Single(c => c.Id == billet.IdCustomer.Value);
                        billet.CustomerName = customer.Name + " " + customer.Surname;
                        billet.CustomerCpf = customer.Cpf;
                        billet.CustomerEmail = customer.Email;
                        billet.Prize = db.ScratchcardPrize.Single(p => p.Id == billet.IdPrize.Value).Title;

                        result.Add(billet);
                    }


                    ret = new ResultPage<ScratchcardDrawListItem>(result, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.ListScratchedWithPrize", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<ScratchcardDrawListItem> ListPage(int page, int pageItems, string searchWord, out string error, int? idOperation, int? idScratchcard)
        {
            ResultPage<ScratchcardDrawListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmplist = db.ScratchcardDraw.Include("Scratchcard").Where(s =>  (string.IsNullOrEmpty(searchWord) 
                                                                    || s.Customer.Name.Contains(searchWord) 
                                                                    || s.Customer.Surname.Contains(searchWord))
                                                            && (!idOperation.HasValue || s.Scratchcard.IdOperation == idOperation)
                                                            && (!idScratchcard.HasValue || s.IdScratchcard == idScratchcard)
                                                       );

                    var total = tmplist.Count();
                    var list = tmplist.OrderByDescending(s => s.IdScratchcardPrize)
                                        .ThenByDescending(s => s.IdCustomer)
                                        .Skip(page * pageItems)
                                        .Take(pageItems);
                    var result = new List<ScratchcardDrawListItem>();
                    foreach(var item in list)
                    {
                        var billet = new ScratchcardDrawListItem()
                        {
                            Id = item.Id,
                            Date = item.Date,
                            IdCustomer = item.IdCustomer,
                            IdScratchcard = item.IdScratchcard,
                            IdPrize = item.IdScratchcardPrize,
                            ValidationCode = item.ValidationCode,
                            ValidationDate = item.ValidationDate,
                            CampaignName = item.Scratchcard.Name,
                            IdOperation = item.Scratchcard.IdOperation
                        };
                        billet.OperationName = db.Operation.Single(o => o.Id == billet.IdOperation).Title;
                        if (billet.IdCustomer.HasValue)
                        {
                            var customer = db.Customer.Single(c => c.Id == billet.IdCustomer.Value);
                            billet.CustomerName = customer.Name + " " + customer.Surname;
                            billet.CustomerCpf = customer.Cpf;
                            billet.CustomerEmail = customer.Email;
                        }
                        if (billet.IdPrize.HasValue)
                            billet.Prize = db.ScratchcardPrize.Single(p => p.Id == billet.IdPrize.Value).Title;

                        result.Add(billet);
                    }


                    ret = new ResultPage<ScratchcardDrawListItem>(result, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.ListScratchedWithPrize", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as raspadinhas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
