using ias.Rebens.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class BenefitUseListItemModel
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public string BenefitName { get; set; }
        public int IdCustomer { get; set; }
        public int IdBenefit { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCpf { get; set; }
        public string Created { get; set; }
        public bool Used { get; set; }
        public string UsedDate { get; set; }
        public string ApproverName { get; set; }

        public BenefitUseListItemModel() { }
        public BenefitUseListItemModel(BenefitUseListItem item) {
            if (item != null)
            {
                this.Id = item.Id;
                this.PartnerName = item.PartnerName;
                this.BenefitName = item.BenefitName;
                this.IdCustomer = item.IdCustomer;
                this.IdBenefit = item.IdBenefit;
                this.Code = item.Code;
                this.CustomerName = item.CustomerName;
                this.CustomerCpf = item.CustomerCpf;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(item.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.Used = item.UsedDate.HasValue;
                this.UsedDate = item.UsedDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(item.UsedDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider) : "";
                this.ApproverName = item.ApproverName;
            }
        }
    }
}
