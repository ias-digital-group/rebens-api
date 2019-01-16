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
        /// Status do usuário (1 = ativo, 2 = inativo) 
        /// </summary>
        [Required]
        public int Status { get; set; }
        /// <summary>
        /// Nome do Status
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// Id da operação do usuário
        /// </summary>
        public int? IdOperation { get; set; }
        /// <summary>
        /// Papéis do usuário
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Roles { get; set; }

        /// <summary>
        /// Construtor que recebe um AdminUser e popula os objetos
        /// </summary>
        /// <param name="adminUser"></param>
        public AdminUserModel(AdminUser adminUser)
        {
            this.Id = adminUser.Id;
            this.Name = adminUser.Name;
            this.Email = adminUser.Email;
            this.LastLogin = adminUser.LastLogin;
            this.Status = adminUser.Status;
            this.StatusName = Enums.EnumHelper.GetEnumDescription((Enums.AdminUserStatus)adminUser.Status);
            this.IdOperation = adminUser.IdOperation;
            this.Roles = adminUser.Roles;
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
                Status = this.Status,
                IdOperation = this.IdOperation,
                Roles = this.Roles,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
