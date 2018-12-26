using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class OperationTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public OperationTypeModel() { }
        public OperationTypeModel(OperationType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
