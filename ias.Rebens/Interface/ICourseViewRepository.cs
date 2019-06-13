using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseViewRepository
    {
        bool SaveView(int idCourse, int idCustomer, out string error);
    }
}
