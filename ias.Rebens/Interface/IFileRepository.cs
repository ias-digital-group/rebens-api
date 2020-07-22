using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFileRepository
    {
        bool Create(File file, int idAdminUser, out string error);
        bool Delete(int id, int idAdminUser, out string error);
        List<Entity.FileListItem> List(int idItem, int itemType, out string error);
    }
}
