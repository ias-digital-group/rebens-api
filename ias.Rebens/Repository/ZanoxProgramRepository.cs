using ias.Rebens.Entity;
using ias.Rebens.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public ResultPage<ZanoxProgramListItem> ListPage(int page, int pageItems, string word, out string error)
        {
            ResultPage<ZanoxProgramListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ZanoxProgram.Where(b => string.IsNullOrEmpty(word) || b.Name.Contains(word)).OrderBy(f => f.Name);

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    var retList = new List<ZanoxProgramListItem>();
                    foreach(var p in list)
                    {
                        var updatedUser = db.LogAction.Where(a => a.Item == (int)Enums.LogItem.ZanoxProgram && a.Action == (int)Enums.LogAction.update && a.IdItem == p.Id).OrderByDescending(a => a.Created).FirstOrDefault();
                        retList.Add(new ZanoxProgramListItem()
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Logo = p.Image,
                            Created = p.Created,
                            Modified = p.Modified,
                            LastIntegrationDate = p.LastintegrationDate,
                            Platform = "Zanox",
                            Rank = p.AdRank,
                            StartDate = p.StartDate,
                            ModifiedBy = updatedUser != null ? updatedUser.AdminUser.Name : " - ",
                            Status = p.Status
                        });
                    }
                    

                    ret = new ResultPage<ZanoxProgramListItem>(retList, page, pageItems, total);

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

        public ResultPage<ZanoxProgram> ListPageForPortal(int page, int pageItems, string word, out string error)
        {
            ResultPage<ZanoxProgram> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ZanoxProgram.Where(b => (string.IsNullOrEmpty(word) || b.Name.Contains(word)) 
                                    && b.Published && b.Active
                                    && b.Incentives.Any(i => i.Active && !i.Removed 
                                                    && (!i.Start.HasValue || i.Start.Value <= DateTime.UtcNow)
                                                    && (!i.End.HasValue || i.End.Value >= DateTime.UtcNow)
                                                )
                                    );

                    var list = tmpList.OrderBy(p => p.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<ZanoxProgram>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxProgramRepository.ListPageForPortal", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os programas. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ZanoxProgram Read(int id, out string error)
        {
            ZanoxProgram ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.ZanoxProgram.SingleOrDefault(b => b.Id == id);
                    ret.Incentives = db.ZanoxIncentive.Where(i => i.IdProgram == ret.Id && i.Active && !i.Removed
                                            && (!i.Start.HasValue || i.Start.Value <= DateTime.UtcNow)
                                            && (!i.End.HasValue || i.End.Value >= DateTime.UtcNow)).OrderBy(i => i.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxProgramRepository.Read", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o programa. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Save(ZanoxProgram program, out string error, int? idAdminUser = null)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ZanoxProgram.SingleOrDefault(i => i.Id == program.Id);
                    if(update != null)
                    {
                        if (idAdminUser.HasValue)
                        {
                            update.PublishedDate = DateTime.UtcNow;
                            update.Description = program.Description;
                            update.Image = program.Image;
                            update.LocalDescription = program.LocalDescription;
                            update.Name = program.Name;
                            update.Terms = program.Terms;
                            update.Published = program.Published;

                            db.LogAction.Add(new LogAction()
                            {
                                Action = (int)Enums.LogAction.update,
                                Created = DateTime.UtcNow,
                                IdAdminUser = idAdminUser.Value,
                                IdItem = update.Id,
                                Item = (int)LogItem.ZanoxProgram
                            });
                        }
                        else
                        {
                            if (!update.PublishedDate.HasValue)
                            {
                                update.Description = program.Description;
                                update.Image = program.Image;
                                update.LocalDescription = program.LocalDescription;
                                update.Name = program.Name;
                                update.Terms = program.Terms;
                            }
                            update.AdRank = program.AdRank;
                            update.Currency = program.Currency;
                            update.Modified = DateTime.UtcNow;
                            update.MaxCommissionPercent = program.MaxCommissionPercent;
                            update.MinCommissionPercent = program.MinCommissionPercent;
                            update.Active = program.Active;
                            update.Url = program.Url;
                            update.StartDate = program.StartDate;
                            update.Status = program.Status;
                            if (!idAdminUser.HasValue)
                                update.LastintegrationDate = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        program.Published = false;
                        program.LastintegrationDate = program.Created = program.Modified = DateTime.UtcNow;
                        
                        db.ZanoxProgram.Add(program);
                    }

                    db.SaveChanges();
                    error = null;

                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                error = $"Ocorreu um erro ao tentar salvar o Programa. (erro: {logError.Create("ZanoxProgramRepository.Save", ex.Message, JsonConvert.SerializeObject(program), ex.StackTrace)})";
                ret = false;
            }
            return ret;
        }

        public void SaveView(int id, int idCustomer, out string error)
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.ZanoxProgramView.Add(new ZanoxProgramView() { IdZanoxProgram = id, IdCustomer = idCustomer, Created = DateTime.Now });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxProgramRepository.SaveView", ex.Message, $"IdZanoxProgram: {id}, idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gravar a visualização do programa. (erro:" + idLog + ")";
            }
        }

        public bool TogglePublish(int id, int idAdminUser, out string error)
        {
            bool ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ZanoxProgram.SingleOrDefault(a => a.Id == id);
                    if (update != null)
                    {
                        ret = !update.Published;
                        update.Published = ret;
                        update.Modified = DateTime.UtcNow;
                        update.PublishedDate = DateTime.UtcNow;

                        db.LogAction.Add(new LogAction()
                        {
                            Action = ret ? (int)Enums.LogAction.publish : (int)Enums.LogAction.publish,
                            Created = DateTime.UtcNow,
                            Item = (int)Enums.LogItem.ZanoxProgram,
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
                int idLog = logError.Create("BannerRepository.ToggleActive", ex.Message, $"id:{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o usuário. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
