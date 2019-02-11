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
        Inactive = 2
    }

    public enum CustomerType : int
    {
        [Description("Aluno")]
        Student = 1,
        [Description("Indicação")]
        indication = 2
    }

    public enum StaticTextType : int
    {
        [Description("Operação Sobre")]
        AboutOperation = 1,
        [Description("Operação como funciona")]
        HowOperationWork = 2,
        [Description("Teaser do beneficio")]
        BenefitTeaser = 3,
        [Description("Chamada do beneficio")]
        BenefitCall = 4,
        [Description("Descrição de funcionamento online")]
        BenefitOperationOnLine = 5,
        [Description("Descrição de funcionamento offline")]
        BenefitOperationOffLine = 6,
        [Description("Funcionamento Voucher")]
        VoucherOperation = 7
    }

    public enum BankAccountType
    {
        [Description("Conta Corrente")]
        debt,
        [Description("Poupança")]
        savings
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
