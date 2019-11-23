using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Category
    {
        public Category()
        {
            BenefitCategories = new HashSet<BenefitCategory>();
            Categories = new HashSet<Category>();
        }

        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public int Order { get; set; }
        public int? IdParent { get; set; }
        [MaxLength(500)]
        public string Icon { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<BenefitCategory> BenefitCategories { get; set; }
        public virtual ICollection<FreeCourseCategory> FreeCourseCategories { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
