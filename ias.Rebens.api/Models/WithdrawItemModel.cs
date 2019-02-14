using System;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model de Resgate
    /// </summary>
    public class WithdrawItemModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// IdBankAccount
        /// </summary>
        public int IdBankAccount { get; set; }
        /// <summary>
        /// Conta do resgate
        /// </summary>
        public BankAccount BankAccount { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Valor
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public WithdrawItemModel() { }
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="withdraw"></param>
        public WithdrawItemModel(Withdraw withdraw)
        {
            this.Id = withdraw.Id;
            this.IdBankAccount = withdraw.IdBankAccount;
            this.Date = withdraw.Date;
            this.Amount = withdraw.Amount;
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.WithdrawStatus)withdraw.Status);
            if (withdraw.BankAccount != null)
                this.BankAccount = withdraw.BankAccount;
        }
    }
}
