using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace ias.Rebens.Enums
{
    public enum Months : int
    {
        [Description("Janeiro")]
        January = 1,
        [Description("Fevereiro")]
        February = 2,
        [Description("Março")]
        March = 3,
        [Description("Abril")]
        April = 4,
        [Description("Maio")]
        May = 5,
        [Description("Junho")]
        June = 6,
        [Description("Julho")]
        July = 7,
        [Description("Agosto")]
        August = 8,
        [Description("Setembro")]
        September = 9,
        [Description("Outubro")]
        October = 10,
        [Description("Novembro")]
        November = 11,
        [Description("Dezembro")]
        December = 12
    }

    public enum ConfigurationType
    {
        [Description("Cliente")]
        Customer = 1
    }

    public enum BenefitUseStatus : int
    {
        [Description("Reemetir")]
        Reemit = 1,
        [Description("Não Possuí Cashback")]
        NoCashBack = 2,
        [Description("Cashback Disponível")]
        Cashback = 3,
        [Description("Cashback em Processamento")]
        ProcessingCashback = 4
    }

    public enum BannerType
    {
        [Description("Home Full")]
        Home = 1,
        [Description("Categoria")]
        Category = 2,
        [Description("Imperdíveis")]
        Unmissable = 3
    }

    public enum AdminUserStatus: int
    {
        [Description("Ativo")]
        Active = 1,
        [Description("Inativo")]
        Inactive = 2
    }

    public enum CustomerStatus : int
    {
        [Description("Ativo")]
        Active = 1,
        [Description("Inativo")]
        Inactive = 2,
        [Description("Validação")]
        Validation = 3,
        [Description("Trocar Senha")]
        ChangePassword = 4,
        [Description("Imcompleto")]
        Incomplete = 5
    }

    public enum CustomerType : int
    {
        [Description("Aluno")]
        Student = 1,
        [Description("Indicação")]
        indication = 2
    }

    public enum CustomerReferalStatus : int
    {
        [Description("Pendente")]
        pending = 1,
        [Description("Assinado")]
        Signed = 2
    }

    public enum StaticTextType : int
    {
        [Description("Operação Sobre")]
        AboutOperation = 1,
        [Description("Operação como funciona")]
        HowOperationWork = 2,
        [Description("Detalhes")]
        BenefitDetail = 3,
        [Description("Chamada do beneficio")]
        BenefitCall = 4,
        [Description("Como Utilizar")]
        BenefitHowToUse = 5,
        [Description("Descrição do Parceiro")]
        PartnerDescription = 6,
        [Description("E-mail de recuperação de senha")]
        EmailPasswordRecovery = 7,
        [Description("E-mail de validação de cliente")]
        EmailCustomerValidation = 8
    }

    public enum BankAccountType
    {
        [Description("Conta Corrente")]
        debt,
        [Description("Poupança")]
        savings
    }

    public enum WithdrawStatus : int
    {
        [Description("Novo")]
        New = 1,
        [Description("Pendente")]
        Pendent = 2,
        [Description("Finalizado")]
        Done = 3
    }

    public enum ZanoxState : int
    {
        [Description("Confirmado")]
        confirmed = 1,
        [Description("Aberto")]
        open = 2,
        [Description("Rejeitado")]
        rejected = 3,
        [Description("Aprovado")]
        approved = 4
    }

    public enum ZanoxStatus : int
    {
        [Description("Pendent")]
        pendent = 1,
        [Description("Tratado")]
        treat = 2
    }

    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }
    }

    public class StyleClass : Attribute
    {
        private string _value;
        public StyleClass(string value)
        {
            _value = value;
        }
        public string Value
        {
            get { return _value; }
        }
    }

    public class ObjectName : System.Attribute
    {
        private string _value;
        public ObjectName(string value)
        {
            _value = value;
        }
        public string Value
        {
            get { return _value; }
        }
    }
}
