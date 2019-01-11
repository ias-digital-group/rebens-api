﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Parceiro
    /// </summary>
    public class PartnerModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        public ContactModel Contact { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public PartnerModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Partner e popula os atributos
        /// </summary>
        /// <param name="partner"></param>
        public PartnerModel(Partner partner) {
            this.Id = partner.Id;
            this.Name = partner.Name;
            this.Active = partner.Active;
        }

        /// <summary>
        /// retorna um objeto Partner com as informações
        /// </summary>
        /// <returns></returns>
        public Partner GetEntity() {
            return new Partner()
            {
                Id = this.Id,
                Name = this.Name,
                Active = this.Active,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}