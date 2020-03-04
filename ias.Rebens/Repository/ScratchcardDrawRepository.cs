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

        public ResultPage<ScratchcardDraw> ListByCustomer(int idCustomer, int page, int pageItems, out string error)
        {
            ResultPage<ScratchcardDraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ScratchcardDraw.Where(s => s.IdCustomer == idCustomer).OrderByDescending(s => s.Created);
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

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

        public ResultPage<ScratchcardDraw> ListByScratchcard(int idScratchcard, int page, int pageItems, out string error)
        {
            ResultPage<ScratchcardDraw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ScratchcardDraw.Where(s => s.IdScratchcard == idScratchcard).OrderBy(s => s.Created);
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<ScratchcardDraw>(list, page, pageItems, total);
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

        public ScratchcardDraw LoadRandom(int idScratchcard, string path, int idCustomer, DateTime date, DateTime? expireDate, out string error)
        {
            ScratchcardDraw ret;
            try
            {
                var cloudinary = new Integration.CloudinaryHelper();
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ScratchcardDraw.Where(s => s.IdScratchcard == idScratchcard && !s.IdCustomer.HasValue).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

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

                    string fileName = $"scratchcard-{ret.IdScratchcard}-{ret.Id}.png";
                    if (ret.IdScratchcardPrize.HasValue)
                    {
                        var prize = prizes.Single(p => p.Id == ret.IdScratchcardPrize.Value);
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
                    ret.Modified = DateTime.UtcNow;
                    ret.IdCustomer = idCustomer;
                    ret.Date = date;
                    ret.ExpireDate = expireDate;
                    ret.ValidationCode = Helper.SecurityHelper.GenerateCode(20);
                    ret.Status = (int)Enums.ScratchcardDraw.drawn;
                    ret.Image = cloudinaryModel.secure_url;

                    db.SaveChanges();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ScratchcardDrawRepository.LoadRandom", ex.Message, $"idScratchcard: {idScratchcard}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a raspadinha. (erro:" + idLog + ")";
                ret = null;
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
    }
}
