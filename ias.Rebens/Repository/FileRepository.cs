using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class FileRepository : IFileRepository
    {
        private string _connectionString;
        public FileRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(File file, int idAdminUser, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    file.Created = file.Modified = DateTime.UtcNow;
                    db.File.Add(file);
                    db.SaveChanges();

                    db.LogAction.Add(new LogAction()
                    {
                        Action = (int)Enums.LogAction.create,
                        Created = DateTime.UtcNow,
                        IdAdminUser = idAdminUser,
                        IdItem = file.Id,
                        Item = (int)Enums.LogItem.File
                    });
                    db.SaveChanges();
                    ret = true;
                    error = null;
                }
            }
            catch(Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o arquivo. (erro:" + idLog + ")";
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
                    var file = db.File.SingleOrDefault(f => f.Id == id);
                    if(file != null)
                    {
                        db.File.Remove(file);
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.delete,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = file.Id,
                            Item = (int)Enums.LogItem.File
                        });

                        db.SaveChanges();
                    }
                    
                    ret = true;
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar o arquivo. (erro:" + idLog + ")";
            }
            return ret;
        }

        public List<Entity.FileListItem> List(int idItem, int itemType, out string error)
        {
            List<Entity.FileListItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.File.Where(f => f.IdItem == idItem && f.ItemType == itemType).OrderBy(f => f.Name).Select(f => new Entity.FileListItem()
                    {
                        Id = f.Id,
                        Created = f.Created,
                        FileName = f.FileName,
                        FileUrl = f.FileUrl,
                        IdItem = f.IdItem,
                        ItemType = f.ItemType,
                        Name = f.Name
                    }).ToList();

                    ret.ForEach(c =>
                    {
                        var createUser = db.LogAction.Include("AdminUser").Where(a => a.Item == (int)Enums.LogItem.File && a.IdItem == c.Id && a.Action == (int)Enums.LogAction.create)
                                            .OrderBy(a => a.Created).FirstOrDefault();
                        if (createUser != null)
                            c.CreatedUserName = createUser.AdminUser.Name + " " + createUser.AdminUser.Surname;
                        else
                            c.CreatedUserName = " - ";
                        
                    });
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileRepository.List", ex.Message, $"idItem: {idItem}, itemType: {itemType}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os arquivos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
