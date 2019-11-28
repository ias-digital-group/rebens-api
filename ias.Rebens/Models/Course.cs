using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Course
    {
        public Course()
        {
            CoursePeriods = new HashSet<CourseCoursePeriod>();
            CourseUses = new HashSet<CourseUse>();
            CourseViews = new HashSet<CourseView>();
            CourseAddresses = new HashSet<CourseAddress>();
            CourseCustomerRates = new HashSet<CourseCustomerRate>();
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int IdOperation { get; set; }
        public int IdCollege { get; set; }
        public int IdGraduationType { get; set; }
        public int IdModality { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        public string Duration { get; set; }
        public string Image { get; set; }
        public string ListImage { get; set; }
        public decimal Rating { get; set; }
        public int IdAdminUser { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string VoucherText { get; set; }
        public int? IdDescription { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CourseBegin { get; set; }
        public int IdFaq { get; set; }
        public int IdRegulation { get; set; }

        public string Disclaimer { get; set; }
        public string CourseTypeDescription { get; set; }
        public string BenefitBoxTitle { get; set; }
        public string BenefitBoxDescription { get; set; }
        public string HelpStudentTitle { get; set; }
        public string HelpStudentDescription { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual CourseCollege College { get; set; }
        public virtual CourseGraduationType GraduationType { get; set; }
        public virtual CourseModality Modality { get; set; }
        public virtual AdminUser AdminUser { get; set; }
        public virtual StaticText Description { get; set; }

        public virtual ICollection<CourseCoursePeriod> CoursePeriods { get; set; }
        public virtual ICollection<CourseUse> CourseUses { get; set; }
        public virtual ICollection<CourseView> CourseViews { get; set; }
        public virtual ICollection<CourseAddress> CourseAddresses { get; set; }
        public virtual ICollection<CourseCustomerRate> CourseCustomerRates { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
