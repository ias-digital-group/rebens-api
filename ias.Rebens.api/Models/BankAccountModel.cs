using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Bank Account Model
    /// </summary>
    public class BankAccountModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id do Banco
        /// </summary>
        [Required]
        public int IdBank { get; set; }
        /// <summary>
        /// Tipo de conta
        /// </summary>
        [Required]
        public string Type { get; set; }
        /// <summary>
        /// Agência
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Branch { get; set; }
        /// <summary>
        /// Conta Corrente
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Nome do Banco
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public BankAccountModel() { }

        /// <summary>
        /// Construtor que recebe um BankAccount e popula os atributos
        /// </summary>
        /// <param name="account"></param>
        public BankAccountModel(BankAccount account) {
            this.Id = account.Id;
            this.IdBank = account.IdBank;
            this.AccountNumber = account.AccountNumber;
            this.Active = account.Active;
            this.Branch = account.Branch;
            this.Type = account.Type;

            if (account.Bank != null)
                this.BankName = account.Bank.Name;
        }

        /// <summary>
        /// Retorna um objeto BankAccount com as informações
        /// </summary>
        /// <returns></returns>
        public BankAccount GetEntity()
        {
            return new BankAccount()
            {
                Id = this.Id,
                AccountNumber = this.AccountNumber,
                Active = this.Active,
                Branch = this.Branch,
                Created = DateTime.Now,
                IdBank = this.IdBank,
                Modified = DateTime.Now,
                Type = this.Type
            };
        }
    }

    /// <summary>
    /// Bank model
    /// </summary>
    public class BankModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Código
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public BankModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Bank e popula os atributos
        /// </summary>
        public BankModel(Bank bank)
        {
            this.Id = bank.Id;
            this.Name = bank.Name;
            this.Code = bank.Code;
        }
    }
}
