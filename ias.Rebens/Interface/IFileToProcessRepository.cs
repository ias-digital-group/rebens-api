using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFileToProcessRepository
    {
        bool Create(FileToProcess fileToProcess, out string error);

        FileToProcess Read(int id, out string error);

        FileToProcess ReadByType(int type, int? status, int? idOperation, out string error);

        bool UpdateProcessed(int id, out string error);

        bool UpdateStatus(int id, int status, out string error);
        
        bool UpdateTotal(int id, int total, out string error);
    }
}
