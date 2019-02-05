using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFormEstablishmentRepository
    {
        bool Create(FormEstablishment formEstablishment, out string error);
    }
}
