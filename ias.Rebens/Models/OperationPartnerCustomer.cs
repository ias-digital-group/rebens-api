using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class OperationPartnerCustomer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public int? IdCustomer { get; set; }
        public int IdOperationPartner { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public OperationPartner OperationPartner { get; set; }
        public Customer Customer { get; set; }
    }
}
