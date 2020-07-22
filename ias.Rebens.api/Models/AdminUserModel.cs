using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class AdminUserModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Name { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Email { get; set; }
        /// <summary>
        /// Data Hora do último login do usuário
        /// </summary>
        public DateTime? LastLogin { get; set; }
        /// <summary>
        /// Status do usuário 
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Id da operação do usuário
        /// </summary>
        public int? IdOperation { get; set; }
        /// <summary>
        /// Id do parceiros da operação 
        /// </summary>
        public int? IdOperationPartner { get; set; }
        /// <summary>
        /// Retorna o nome do clube que esse usuário faz parte
        /// </summary>
        public string Operation { get; }
        /// <summary>
        /// Retorna o nome do parceiro que esse usuário faz parte
        /// </summary>
        public string OperationPartner { get; }
        /// <summary>
        /// Papéis do usuário
        /// </summary>
        [MaxLength(500)]
        public string Roles { get; set; }
        /// <summary>
        /// Foto do usuário (100x100)
        /// </summary>
        [MaxLength(500)]
        public string Picture { get; set; }
        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Surname { get; set; }
        /// <summary>
        /// CPF do usuário
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Doc { get; set; }
        /// <summary>
        /// Telefone
        /// </summary>
        [MaxLength(50)]
        public string PhoneMobile { get; set; }
        /// <summary>
        /// Telefone Comercial
        /// </summary>
        [MaxLength(50)]
        public string PhoneComercial { get; set; }
        /// <summary>
        /// Celular comercial
        /// </summary>
        [MaxLength(50)]
        public string PhoneComercialMobile { get; set; }
        /// <summary>
        /// Ramal do telefone comercial
        /// </summary>
        [MaxLength(50)]
        public string PhoneComercialBranch { get; set; }
        /// <summary>
        /// Nome amigável do Papel do usuário
        /// </summary>
        public string RoleName { get; }
        /// <summary>
        /// Retorna as iniciais do ususário
        /// </summary>
        public string Initials { get; }

        /// <summary>
        /// Construtor
        /// </summary>
        public AdminUserModel() { }

        /// <summary>
        /// Construtor que recebe um AdminUser e popula os objetos
        /// </summary>
        /// <param name="adminUser"></param>
        public AdminUserModel(AdminUser adminUser)
        {
            if (adminUser != null)
            {
                this.Id = adminUser.Id;
                this.Name = adminUser.Name;
                this.Surname = adminUser.Surname;
                this.Email = adminUser.Email;
                this.LastLogin = adminUser.LastLogin;
                this.Active = adminUser.Active;
                this.IdOperation = adminUser.IdOperation;
                this.Roles = adminUser.Roles;
                this.IdOperationPartner = adminUser.IdOperationPartner;
                this.Picture = adminUser.Picture;
                this.Doc = adminUser.Doc;
                this.PhoneComercial = adminUser.PhoneComercial;
                this.PhoneComercialBranch = adminUser.PhoneComercialBranch;
                this.PhoneComercialMobile = adminUser.PhoneComercialMobile;
                this.PhoneMobile = adminUser.PhoneMobile;
                this.Initials = adminUser.Name.Substring(0, 1) + (string.IsNullOrEmpty(adminUser.Surname) ? "" : adminUser.Surname.Substring(0, 1));
                if (Enum.TryParse(adminUser.Roles, out Enums.Roles role))
                    this.RoleName = Enums.EnumHelper.GetEnumDescription(role);
                if (adminUser.IdOperation.HasValue && adminUser.Operation != null)
                    this.Operation = adminUser.Operation.Title;
                else if(this.Roles == Enums.Roles.master.ToString()
                        || this.Roles == Enums.Roles.administratorRebens.ToString()
                        || this.Roles == Enums.Roles.publisherRebens.ToString())
                {
                    this.Operation = "Todos";
                }
                if (adminUser.IdOperationPartner.HasValue && adminUser.OperationPartner != null)
                    this.OperationPartner = adminUser.OperationPartner.Name;
                else if (this.Roles == Enums.Roles.master.ToString()
                        || this.Roles == Enums.Roles.administratorRebens.ToString()
                        || this.Roles == Enums.Roles.publisherRebens.ToString())
                {
                    this.OperationPartner = "Rebens";
                }
                else if (this.Roles == Enums.Roles.administrator.ToString()
                      || this.Roles == Enums.Roles.publisher.ToString())
                {
                    this.OperationPartner = this.Operation;
                }
            }
        }

        /// <summary>
        /// Retorna um objeto AdminUser com as informações
        /// </summary>
        /// <returns></returns>
        public AdminUser GetEntity()
        {
            return new AdminUser()
            {
                Id = this.Id,
                Name = this.Name,
                Surname = this.Surname,
                Email = this.Email,
                LastLogin = this.LastLogin,
                Active = this.Active,
                IdOperation = this.IdOperation,
                Roles = this.Roles,
                Doc = this.Doc,
                Picture = this.Picture,
                PhoneMobile = this.PhoneMobile,
                PhoneComercial = this.PhoneComercial,
                PhoneComercialBranch = this.PhoneComercialBranch,
                PhoneComercialMobile = this.PhoneComercialMobile,
                IdOperationPartner = this.IdOperationPartner,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
