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

        public CourseItem() { }
        public CourseItem(Course course)
        {
            this.Id = course.Id;
            this.Title = course.Title;
            this.Discount = course.Discount;
            this.OriginalPrice = course.OriginalPrice;
            this.FinalPrice = course.FinalPrice;
            this.Rating = course.Rating;
            this.Duration = course.Duration;
            this.Image = course.Image;
        }
    }
}
