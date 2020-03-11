using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class ScratchcardDrawItem : ScratchcardDraw
    {
        public string Instructions { get; set; }

        public ScratchcardDrawItem() { }
        public ScratchcardDrawItem(ScratchcardDraw scratchcardDraw) 
        {
            if (scratchcardDraw != null)
            {
                this.Id = scratchcardDraw.Id;
                this.IdScratchcard = scratchcardDraw.IdScratchcard;
                this.IdScratchcardPrize = scratchcardDraw.IdScratchcardPrize;
                this.Image = scratchcardDraw.Image;
                this.IdCustomer = scratchcardDraw.IdCustomer;
                this.Prize = scratchcardDraw.PlayedDate.HasValue ? scratchcardDraw.Prize : "";
                this.Date = scratchcardDraw.Date;
                this.ExpireDate = scratchcardDraw.ExpireDate;
                this.ValidationCode = scratchcardDraw.ValidationCode;
                this.OpenDate = scratchcardDraw.OpenDate;
                this.PlayedDate = scratchcardDraw.PlayedDate;
                this.ValidationDate = scratchcardDraw.ValidationDate;
                this.Status = scratchcardDraw.Status;
                this.Created = scratchcardDraw.Created;
                this.Modified = scratchcardDraw.Modified;
                this.Scratchcard = scratchcardDraw.Scratchcard;
                this.ScratchcardPrize = scratchcardDraw.ScratchcardPrize;
                this.Customer = scratchcardDraw.Customer;
            }
        }
    }
}
