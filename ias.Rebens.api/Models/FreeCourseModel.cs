using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model de crusos Livres
    /// </summary>
    public class FreeCourseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do usário que fez a última modificação
        /// </summary>
        public int IdAdminUser { get; set; }
        /// <summary>
        /// Id do parceiro
        /// </summary>
        public int IdPartner { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        public int IdOperation { get; set; }
        /// <summary>
        /// Nome para controle 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Título do curso
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Resumo para listagem
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// Descrição 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Como utilizar
        /// </summary>
        public string HowToUse { get; set; }
        /// <summary>
        /// Imagem
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Imagem para listagem
        /// </summary>
        public string ListImage { get; set; }
        /// <summary>
        /// Preço
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Se está ativo ou inativo
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Data da última alteração
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Parceiro
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// Logo Parceiro
        /// </summary>
        public string PartnerImage { get; set; }

        /// <summary>
        /// Descrição do Parceiro
        /// </summary>
        public string PartnerDescription { get; set; }
        

        /// <summary>
        /// Construtor
        /// </summary>
        public FreeCourseModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Course e popula os atributos
        /// </summary>
        /// <param name="course"></param>
        public FreeCourseModel(FreeCourse course)
        {
            this.Id = course.Id;
            this.IdAdminUser = course.IdAdminUser;
            this.IdOperation = course.IdOperation;
            this.IdPartner = course.IdPartner;
            this.Name = course.Name;
            this.Title = course.Title;
            this.Image = course.Image;
            this.ListImage = course.ListImage;
            this.Summary = course.Summary;
            this.Description = course.Description;
            this.HowToUse = course.HowToUse;
            this.Price = course.Price;
            this.Active = course.Active;
            this.Modified = course.Modified;
            if(course.Partner != null)
            {
                this.PartnerName = course.Partner.Name;
                this.PartnerImage = course.Partner.Logo;
                if (course.Partner.StaticText != null)
                    this.PartnerDescription = course.Partner.StaticText.Html;
            }
            
        }

        /// <summary>
        /// Retorna um objeto Course com as informações
        /// </summary>
        /// <returns></returns>
        public FreeCourse GetEntity()
        {
            return new FreeCourse()
            {
                Id = this.Id,
                IdAdminUser = this.IdAdminUser,
                IdOperation = this.IdOperation,
                IdPartner = this.IdPartner,
                Name = this.Name,
                Title = this.Title,
                Summary = this.Summary,
                Image = this.Image,
                ListImage = this.ListImage,
                Description = this.Description,
                HowToUse = this.HowToUse,
                Price = this.Price,
                Active = this.Active,
                Modified = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Model Cursos Livres listagem
    /// </summary>
    public class FreeCourseItemModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do usário que fez a última modificação
        /// </summary>
        public int IdAdminUser { get; set; }
        /// <summary>
        /// Id do parceiro
        /// </summary>
        public int IdPartner { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        public int IdOperation { get; set; }
        /// <summary>
        /// Nome para controle 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Título do curso
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Resumo para listagem
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// Imagem para listagem
        /// </summary>
        public string ListImage { get; set; }
        /// <summary>
        /// Preço
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Parceiro
        /// </summary>
        public Partner Partner { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public FreeCourseItemModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Course e popula os atributos
        /// </summary>
        /// <param name="course"></param>
        public FreeCourseItemModel(FreeCourseItem course)
        {
            this.Id = course.Id;
            this.IdAdminUser = course.IdAdminUser;
            this.IdOperation = course.IdOperation;
            this.IdPartner = course.IdPartner;
            this.Name = course.Name;
            this.Title = course.Title;
            this.ListImage = course.ListImage;
            this.Summary = course.Summary;
            this.Price = course.Price;
            this.Partner = course.Partner;
        }
    }
}
