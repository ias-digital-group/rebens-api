using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class FileToProcessRepository : IFileToProcessRepository
    {
        private string _connectionString;
        public FileToProcessRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public FileToProcessRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Create(FileToProcess fileToProcess, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    fileToProcess.Modified = fileToProcess.Created = DateTime.UtcNow;
                    db.FileToProcess.Add(fileToProcess);
                    db.SaveChanges();
                    error = null;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileToProcessRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o arquivo. (erro:" + idLog + ")";
            }
            return ret;
        }

        public FileToProcess Read(int id, out string error)
        {
            FileToProcess ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.FileToProcess.SingleOrDefault(f => f.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileToProcessRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o arquivo. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public FileToProcess ReadByType(int type, int? status, int? idOperation, out string error)
        {
            FileToProcess ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.FileToProcess.SingleOrDefault(f => f.Type == type && (!status.HasValue || status == f.Status) && (!idOperation.HasValue || idOperation == f.IdOperation));
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileToProcessRepository.ReadByType", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o arquivo. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool UpdateProcessed(int id, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.FileToProcess.SingleOrDefault(f => f.Id == id);
                    update.Processed++; 
                    update.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileToProcessRepository.UpdateProcessed", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o arquivo. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool UpdateStatus(int id, int status, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.FileToProcess.SingleOrDefault(f => f.Id == id);
                    update.Status = status;
                    update.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileToProcessRepository.UpdateStatus", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o arquivo. (erro:" + idLog + ")";
            }
            return ret;
        }

        public bool UpdateTotal(int id, int total, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.FileToProcess.SingleOrDefault(f => f.Id == id);
                    update.Total = total;
                    update.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FileToProcessRepository.UpdateTotal", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o arquivo. (erro:" + idLog + ")";
            }
            return ret;
        }

        public FileToProcess GetNextFile()
        {
            FileToProcess ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.FileToProcess.Where(f => f.Status == (int)Enums.FileToProcessStatus.Ready).OrderBy(f => f.Created).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("FileToProcessRepository.GetNextFile", ex.Message, "", ex.StackTrace);
                ret = null;
            }
            return ret;
        }
    }
}
