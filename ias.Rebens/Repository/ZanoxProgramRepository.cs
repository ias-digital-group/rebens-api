using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ias.Rebens
{
    public class ZanoxProgramRepository : IZanoxProgramRepository
    {
        private string _connectionString;
        public ZanoxProgramRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public ResultPage<ZanoxProgram> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<ZanoxProgram> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ZanoxProgram.Where(b => string.IsNullOrEmpty(word) || b.Name.Contains(word));
                    switch (sort.ToLower())
                    {
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        default:
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<ZanoxProgram>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxProgramRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os programas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Save(ZanoxProgram program, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ZanoxProgram.SingleOrDefault(i => i.Id == program.Id);
                    if(update != null)
                    {
                        update.Active = program.Active;
                        update.AdRank = program.AdRank;
                        update.Currency = program.Currency;
                        update.Description = program.Description;
                        update.Image = program.Image;
                        update.LocalDescription = program.LocalDescription;
                        update.MaxCommissionPercent = program.MaxCommissionPercent;
                        update.MinCommissionPercent = program.MinCommissionPercent;
                        update.Modified = DateTime.UtcNow;
                        update.Name = program.Name;
                        update.StartDate = program.StartDate;
                        update.Status = program.Status;
                        update.Terms = program.Terms;
                        update.Url = program.Url;
                    }
                    else
                    {
                        program.Created = program.Modified = DateTime.UtcNow;
                        db.ZanoxProgram.Add(program);
                    }

                    db.SaveChanges();
                    error = null;

                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                error = $"Ocorreu um erro ao tentar salvar o Incnetivo. (erro: {logError.Create("ZanoxIncentiveRepository.Save", ex.Message, "", ex.StackTrace)})";
                ret = false;
            }
            return ret;
        }
    }
}
