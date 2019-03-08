using System;
using System.Globalization;

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
        public BankAccountModel BankAccount { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Valor
        /// </summary>
        public string Amount { get; set; }
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
            this.Date = withdraw.Date.ToString("dd/MM/yyyy");
            this.Amount = withdraw.Amount.ToString("N", CultureInfo.GetCultureInfo("pt-BR"));
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.WithdrawStatus)withdraw.Status);
            if (withdraw.BankAccount != null)
                this.BankAccount = new BankAccountModel(withdraw.BankAccount);
        }
    }
}
