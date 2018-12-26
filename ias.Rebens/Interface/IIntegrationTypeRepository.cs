using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IIntegrationTypeRepository
    {
        bool Create(IntegrationType integrationType, out string error);

        bool Update(IntegrationType integrationType, out string error);

        bool Delete(int id, out string error);

        IntegrationType Read(int id, out string error);

        List<IntegrationType> ListActive(out string error);

        List<IntegrationType> List(out string error);
    }
}
