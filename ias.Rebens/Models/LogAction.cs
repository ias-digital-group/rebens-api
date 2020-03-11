using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class LogAction
    {
        public int Id { get; set; }
        public int IdAdminUser { get; set; }
        public int Item { get; set; }
        public int IdItem { get; set; }
        public int Action { get; set; }
        public string Extra { get; set; }
        public DateTime Created { get; set; }

        public virtual AdminUser AdminUser { get; set; }

    }
}
