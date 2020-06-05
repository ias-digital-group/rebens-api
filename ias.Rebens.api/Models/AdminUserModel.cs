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
        /// Id do parceiro
        /// </summary>
        public int? IdPartner { get; set; }
        /// <summary>
        /// Papéis do usuário
        /// </summary>
        [MaxLength(500)]
        public string Roles { get; set; }

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
                this.Email = adminUser.Email;
                this.LastLogin = adminUser.LastLogin;
                this.Active = adminUser.Status == (int)Enums.AdminUserStatus.Active;
                this.IdOperation = adminUser.IdOperation;
                this.Roles = adminUser.Roles;
                this.IdPartner = adminUser.IdPartner;
                this.IdOperationPartner = adminUser.IdOperationPartner;
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
                Email = this.Email,
                LastLogin = this.LastLogin,
                Status = this.Active ? (int)Enums.AdminUserStatus.Active : (int)Enums.AdminUserStatus.Inactive,
                IdOperation = this.IdOperation,
                Roles = this.Roles,
                IdOperationPartner = this.IdOperationPartner,
                IdPartner = this.IdPartner,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
