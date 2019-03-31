using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class BenefitReportItem : Benefit
    {
        public int TotalUse { get; set; }

        public BenefitReportItem() { }

        public BenefitReportItem(Benefit benefit, int totalUse)
        {
            this.Active = benefit.Active;
            this.Call = benefit.Call;
            this.CashbackAmount = benefit.CashbackAmount;
            this.CPVPercentage = benefit.CPVPercentage;
            this.Created = benefit.Created;
            this.DueDate = benefit.DueDate;
            this.End = benefit.End;
            this.Exclusive = benefit.Exclusive;
            this.HomeBenefitHighlight = benefit.HomeBenefitHighlight;
            this.HomeHighlight = benefit.HomeHighlight;
            this.Id = benefit.Id;
            this.IdAdminUser = benefit.IdAdminUser;
            this.IdBenefitType = benefit.IdBenefitType;
            this.IdIntegrationType = benefit.IdIntegrationType;
            this.IdOperation = benefit.IdOperation;
            this.IdPartner = benefit.IdPartner;
            this.Image = benefit.Image;
            this.Link = benefit.Link;
            this.MaxDiscountPercentage = benefit.MaxDiscountPercentage;
            this.MinDiscountPercentage = benefit.MinDiscountPercentage;
            this.Modified = benefit.Modified;
            this.Name = benefit.Name;
            this.Start = benefit.Start;
            this.Title = benefit.Title;
            this.TotalUse = totalUse;
        }
    }
}
