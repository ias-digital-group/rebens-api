using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class ProductValidateItem
    {
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public int? IdAdminUser { get; set; }
        public int IdOperation { get; set; }
        public int IdCustomer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCpf { get; set; }
        public string ItemName { get; set; }
        public string Voucher { get; set; }
        public string OperationName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Used { get; set; }
        public string AdminUserName { get; set; }
    }
}
