using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFormContactRepository
    {
        bool Create(FormContact formContact, out string error);
    }
}
