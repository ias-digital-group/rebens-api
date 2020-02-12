using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IModuleRepository
    {
        List<Module> List(out string error);
    }
}
