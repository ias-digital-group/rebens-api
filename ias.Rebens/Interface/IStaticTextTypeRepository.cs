using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IStaticTextTypeRepository
    {
        bool Create(StaticTextType staticTextType, out string error);

        bool Update(StaticTextType staticTextType, out string error);

        bool Delete(int id, out string error);

        StaticTextType Read(int id, out string error);

        List<StaticTextType> ListActive(out string error);

        List<StaticTextType> List(out string error);
    }
}
