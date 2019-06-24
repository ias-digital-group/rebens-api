using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Faculdade
    /// </summary>
    public class CourseModel
    {
        /// <summary>
        /// Id da Faculdade
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Título do curso
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Id da faculdade
        /// </summary>
        [Required]
        public int IdCollege { get; set; }

        /// <summary>
        /// Id do tipo de graduação
        /// </summary>
        [Required]
        public int IdGraduationType { get; set; }

        /// <summary>
        /// Id da modalidade
        /// </summary>
        [Required]
        public int IdModality { get; set; }

        /// <summary>
        /// Preço original
        /// </summary>
        [Required]
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Desconto %
        /// </summary>
        [Required]
        public decimal Discount { get; set; }

        /// <summary>
        /// Preço Final
        /// </summary>
        [Required]
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Duração do curso
        /// </summary>
        [Required]
        public string Duration { get; set; }

        /// <summary>
        /// Imagem
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Nota
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Validade
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Inicio
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Fim
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Texto do voucher
        /// </summary>
        public string VoucherText { get; set; }

        /// <summary>
        /// Se o curso está ativa ou não
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Status em string "Ativo" ou "Inativo"
        /// </summary>
        public string StatusName { get { return this.Active ? "Ativo" : "Inativo"; } }

        /// <summary>
        /// Endereços da Faculdade
        /// </summary>
        public List<AddressModel> Addresses { get; set; }

        /// <summary>
        /// Endereço da Faculdade
        /// </summary>
        public AddressModel Address { get; set; }

        /// <summary>
        /// Periodos do curso
        /// </summary>
        public List<CoursePeriod> Periods { get; set; }

        /// <summary>
        /// Descrição do curso
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Array com os Id's dos períodos
        /// </summary>
        public int[] PeriodIds { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CourseModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Course e popula os atributos
        /// </summary>
        /// <param name="course"></param>
        public CourseModel(Course course)
        {
            this.Id = course.Id;
            this.Title = course.Title;
            this.Active = course.Active;
            this.IdOperation = course.IdOperation;
            this.IdGraduationType = course.IdGraduationType;
            this.IdCollege = course.IdCollege;
            this.IdModality = course.IdModality;
            this.Image = course.Image;
            this.OriginalPrice = course.OriginalPrice;
            this.Discount = course.Discount;
            this.FinalPrice = course.FinalPrice;
            this.Duration = course.Duration;
            this.Rating = course.Rating;
            this.DueDate = course.DueDate;
            this.StartDate = course.StartDate;
            this.EndDate = course.EndDate;
            this.VoucherText = course.VoucherText;
            if(course.CoursePeriods != null)
                this.PeriodIds = course.CoursePeriods.Select(p => p.IdPeriod).ToArray();            
            else
                this.PeriodIds = new List<int>().ToArray();
            if (course.Description != null)
                this.Description = course.Description.Html;
        }

        /// <summary>
        /// Retorna um objeto Course com as informações
        /// </summary>
        /// <returns></returns>
        public Course GetEntity()
        {
            return new Course()
            {
                Id = this.Id,
                Title = this.Title,
                Active = this.Active,
                IdOperation = this.IdOperation,
                IdGraduationType = this.IdGraduationType,
                IdCollege = this.IdCollege,
                IdModality = this.IdModality,
                Image = this.Image,
                OriginalPrice = this.OriginalPrice,
                Discount = this.Discount,
                FinalPrice = this.FinalPrice,
                Duration = this.Duration,
                Rating = this.Rating,
                DueDate = this.DueDate,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                VoucherText = this.VoucherText,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StaticText GetDescription()
        {
            return new StaticText()
            {
                Active = true,
                Created = DateTime.Now,
                Html = this.Description,
                IdStaticTextType = (int)Enums.StaticTextType.CourseDescription,
                Modified = DateTime.Now,
                Order = 1,
                Title = "Descrição do curso - " + this.Title
            };
        }
    }

    /// <summary>
    /// Relacionamento Curso com Endereço
    /// </summary>
    public class CourseAddressModel
    {
        /// <summary>
        /// Id do curso
        /// </summary>
        [Required]
        public int IdCourse { get; set; }


        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int IdAddress { get; set; }
    }

    /// <summary>
    /// Relacionamento Curso com Periodo
    /// </summary>
    public class CourseCoursePeriodModel
    {
        /// <summary>
        /// Id do curso
        /// </summary>
        [Required]
        public int IdCourse { get; set; }


        /// <summary>
        /// Id do periodo
        /// </summary>
        [Required]
        public int IdPeriod { get; set; }
    }

    /// <summary>
    /// Item para listagem de cursos no portal
    /// </summary>
    public class CourseItemModel
    {
        /// <summary>
        /// Id do curso
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string CollegeImage { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string CollegeName { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string GraduationType { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string Modality { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public string Period { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public decimal FinalPrice { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public decimal Rating { get; set; }
        /// <summary>
        /// Id do curso
        /// </summary>
        public int Evaluations { get; set; }
        /// <summary>
        /// Duração
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// Descrição
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Imagem
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CourseItemModel() { }
        /// <summary>
        /// Construtor
        /// </summary>
        public CourseItemModel(CourseItem course)
        {
            this.Id = course.Id;
            this.CollegeImage = course.CollegeImage;
            this.CollegeName = course.CollegeName;
            this.Title = course.Title;
            this.Address = course.Address;
            this.GraduationType = course.GraduationType;
            this.Modality = course.Modality;
            this.Period = course.Period;
            this.Discount = course.Discount;
            this.OriginalPrice = course.OriginalPrice;
            this.FinalPrice = course.FinalPrice;
            this.Rating = course.Rating;
            this.Evaluations = course.Evaluations;
            this.Duration = course.Duration;
            this.Description = course.Description;
            this.Image = course.Image;
        }

    }
}
