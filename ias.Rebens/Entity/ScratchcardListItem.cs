using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class ScratchcardListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OperationName { get; set; }
        public int IdOperation { get; set; }
        public int? Quantity { get; set; }
        public int Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int Status { get; set; }
    }
}
