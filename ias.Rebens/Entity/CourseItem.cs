using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class CourseItem
    {
        public int Id { get; set; }
        public string CollegeImage { get; set; }
        public string CollegeName { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string GraduationType { get; set; }
        public string Duration { get; set; }
        public string Modality { get; set; }
        public string Period { get; set; }
        public decimal Discount { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal Rating { get; set; }
        public int Evaluations { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ListImage { get; set; }
        public string CourseBegin { get; set; }
        public string AddressShort { get; set; }
        public int IdFaq { get; set; }
        public int IdRegulation { get; set; }

        public string Disclaimer { get; set; }
        public string CourseTypeDescription { get; set; }
        public string BenefitBoxTitle { get; set; }
        public string BenefitBoxDescription { get; set; }
        public string HelpStudentTitle { get; set; }
        public string HelpStudentDescription { get; set; }
        public string HelpStudentLink { get; set; }
        public bool Active { get; set; }

        public CourseItem() { }
        public CourseItem(Course course)
        {
            this.Id = course.Id;
            this.Title = course.Title;
            this.Name = course.Name;
            this.Discount = course.Discount;
            this.OriginalPrice = course.OriginalPrice;
            this.FinalPrice = course.FinalPrice;
            this.Rating = course.Rating;
            this.Duration = course.Duration;
            this.Image = course.Image;
            this.ListImage = course.ListImage;
            this.CourseBegin = course.CourseBegin;
            this.IdFaq = course.IdFaq;
            this.IdRegulation = course.IdRegulation;
            this.Disclaimer = course.Disclaimer;
            this.CourseTypeDescription = course.CourseTypeDescription;
            this.BenefitBoxTitle = course.BenefitBoxTitle;
            this.BenefitBoxDescription = course.BenefitBoxDescription;
            this.HelpStudentTitle = course.HelpStudentTitle;
            this.HelpStudentDescription = course.HelpStudentDescription;
            this.HelpStudentLink = course.HelpStudentLink;
            this.Active = course.Active;
        }
    }
}
