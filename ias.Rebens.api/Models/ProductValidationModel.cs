using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ProductValidationModel
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
        public string Created { get; set; }
        public string UsedDate { get; set; }
        public bool Used { get; set; }
        public string AdminUserName { get; set; }

        public ProductValidationModel() { }
        public ProductValidationModel(Entity.ProductValidateItem item) {
            if (item != null)
            {
                this.Id = item.Id;
                this.IdOrder = item.IdOrder;
                this.IdAdminUser = item.IdAdminUser;
                this.IdOperation = item.IdOperation;
                this.IdCustomer = item.IdCustomer;
                this.CustomerName = item.CustomerName;
                this.CustomerCpf = item.CustomerCpf;
                this.ItemName = item.ItemName;
                this.Voucher = item.Voucher;
                this.OperationName = item.OperationName;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(item.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.Used = item.Used.HasValue;
                this.UsedDate = this.Used ? TimeZoneInfo.ConvertTimeFromUtc(item.Used.Value, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider) : "";
                this.AdminUserName = item.AdminUserName;
            }
        }
    }
}
