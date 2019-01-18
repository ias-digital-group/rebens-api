using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IConfigurationRepository
    {
        Configuration Read(int id, out string error);

        Configuration ReadByOperation(int idOperation, out string error);

        bool Create(Configuration configuration, out string error);

        bool Update(Configuration configuration, out string error);
    }
}
