using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace ias.Rebens.Enums
{
    public enum AdminUserStatus : int
    {
        [Description("Ativo")]
        Active = 1,
        [Description("Inativo")]
        Inactive = 2
    }

    public enum BankAccountType
    {
        [Description("Conta Corrente")]
        debt,
        [Description("Poupança")]
        savings
    }

    public enum BannerShow: int
    {
        [Description("Home não logada")]
        HomeNotLogged = 2,
        [Description("Home logada")]
        HomeLogged = 4,
        [Description("Home de benefícios")]
        Benefit = 8
    }

    public enum BannerType
    {
        [Description("Home Full")]
        Home = 1,
        [Description("Imperdíveis")]
        Unmissable = 3
    }

    public enum BenefitType : int
    {
        [Description("Ecommerce")]
        OnLine = 1,
        [Description("Varejo Local")]
        OffLine = 2,
        [Description("Cashback")]
        Cashback = 3
    }

    public enum BenefitUseStatus : int
    {
        [Description("Reemetir")]
        Reemit = 1,
        [Description("Não Possuí Cashback")]
        NoCashBack = 2,
        [Description("Cashback Disponível")]
        CashbackAvailable = 3,
        [Description("Cashback em Processamento")]
        ProcessingCashback = 4,
        [Description("Cashback Resgatado")]
        Withdraw = 5
    }

    public enum CategoryType : int
    {
        Benefit = 1,
        FreeCourse = 2
    }

    public enum CourseUseStatus : int
    {
        [Description("Gerado")]
        Generated = 1,
        [Description("Não Utilizado")]
        NotUsed = 2,
        [Description("Utilizado")]
        Used = 3,
        [Description("Cancelado")]
        Canceled = 4
    }

    public enum ConfigurationType
    {
        [Description("Cliente")]
        Customer = 1
    }

    public enum CouponStatus : int
    {
        [Description("Novo")]
        pendent = 1,
        [Description("Aberto")]
        opened = 2,
        [Description("Raspado")]
        played = 3,
        [Description("Reivindicado")]
        claimed = 4,
        [Description("Validado")]
        validated = 5,
        [Description("Sem Prêmio")]
        noprize = 6
    }

    public enum CustomerReferalStatus : int
    {
        [Description("Pendente")]
        pending = 1,
        [Description("Assinado")]
        Signed = 2,
        [Description("Cadastrado")]
        SignUp = 3
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
        [Description("Incompleto")]
        Incomplete = 5
    }

    public enum CustomerType : int
    {
        [Description("Cliente")]
        Customer = 1,
        [Description("Indicação")]
        Referal = 2
    }

    public enum DegreeOfKinship : int
    {
        [Description("Pai")]
        Father = 1,
        [Description("Mãe")]
        Mother = 2,
        [Description("Irmão")]
        Brother = 3,
        [Description("Irmã")]
        Sister = 4,
        [Description("Tio(a)")]
        Uncle = 5,
        [Description("Primo(a)")]
        Cousin = 6,
        [Description("Avô(ó)")]
        Granparents = 7
    }

    public enum FileToProcessStatus : int
    {
        [Description("Novo")]
        New = 1,
        [Description("Pronto para processar")]
        Ready = 5,
        [Description("Processando")]
        Processing = 2,
        [Description("Erro")]
        Error = 3,
        [Description("Finalizado")]
        Done = 4
    }

    public enum FileToProcessType : int
    {
        [Description("PréCadastro")]
        OperationCustomer = 1
    }

    public enum IntegrationType : int
    {
        [Description("Rebens")]
        Rebens = 1,
        [Description("Zanox")]
        Zanox = 2,
        [Description("Própria")]
        Own = 3
    }

    public enum LogAction : int
    {
        [Description("Criar")]
        create = 1,
        [Description("Atualizar")]
        update = 2,
        [Description("Excluir")]
        delete = 3,
        [Description("Gerar")]
        generate = 4,
        [Description("Publicar")]
        publish = 5,
    }

    public enum LogItem : int
    {
        [Description("Endereço")]
        Address = 1,
        [Description("Usuário")]
        AdminUser = 2,
        [Description("Banner")]
        Banner = 3,
        [Description("Benefício")]
        Benefit = 4,
        [Description("Categoria")]
        Category = 5,
        [Description("Contato")]
        Contact = 6,
        [Description("Campanha Cupom")]
        CouponCampaign = 7,
        [Description("Cupom")]
        Coupon = 8,
        [Description("Faculdadae")]
        CourseCollege = 9,
        [Description("Tipo Graduação")]
        CourseGraduationType = 10,
        [Description("Modalidade")]
        CourseModality = 11,
        [Description("Período")]
        CoursePeriod = 12,
        [Description("Curso")]
        Course = 13,
        [Description("Cliente do promotor")]
        CustomerPromoter = 14,
        [Description("Cliente")]
        Customer = 15,
        [Description("Sorteio")]
        Draw = 16,
        [Description("Perguntas Frequentes")]
        Faq = 17,
        [Description("Arquivos para processar")]
        FileToProcess = 18,
        [Description("Cursos livres")]
        FreeCourse = 19,
        [Description("Cliente da operação")]
        OperationCustomer = 20,
        [Description("Parceiro da operação")]
        OperationPartner = 21,
        [Description("Operação")]
        Operation = 22,
        [Description("Parceiros")]
        Partner = 23,
        [Description("Perfil")]
        Profile = 24,
        [Description("Textos estáticos")]
        StaticText = 25,
        [Description("Raspadinha")]
        Scratchcard = 26,
        [Description("Prêmio da Raspadinha")]
        ScratchcardPrize = 27,
        [Description("Contato Operação")]
        OperationContact = 28,
        [Description("Endereço Operação")]
        OperationAddress = 29,

    }

    public enum MoipNotificationEvent : int
    {
        [Description("subscription.created")]
        SubscriptionCreated = 1,
        [Description("subscription.updated")]
        SubscriptionUpdated = 2,
        [Description("invoice.created")]
        InvoiceCreated = 3,
        [Description("invoice.status_updated")]
        InvoiceStatusUpdated = 4,
        [Description("payment.created")]
        PaymentCreated = 5,
        [Description("payment.status_updated")]
        PaymentStatusUpdated = 6
    }

    public enum MoipNotificationStatus : int
    {
        [Description("Nova")]
        New = 1,
        [Description("Ignorada")]
        Ignored = 2,
        [Description("Processada")]
        Processed = 3,
        [Description("Erro")]
        Error = 4,
        [Description("Cancelado")]
        Canceled = 5
    }

    public enum MoipPaymentStatus : int
    {
        [Description("Autorizado")]
        authorized = 1,
        [Description("Iniciado")]
        started = 2,
        [Description("Boleto impresso")]
        billet_printed = 3,
        [Description("Concluído")]
        done = 4,
        [Description("Cancelado")]
        canceled = 5,
        [Description("Em análise")]
        in_analises = 6,
        [Description("Estornado")]
        reversed = 7,
        [Description("Reembolsado")]
        refunded = 9,
        [Description("Aguardando")]
        waiting = 10
    }

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

    public enum OperationType : int
    {
        [Description("IES")]
        ies = 1,
        [Description("Clube")]
        club = 2,
        [Description("Franquias")]
        franchise = 3,
        [Description("Aluguel")]
        rent = 4
    }

    public enum PartnerType : int
    {
        Benefit = 1,
        FreeCourse = 2
    }

    public enum PublishStatus : int
    {
        [Description("Incompleto")]
        notvalid = 0,
        [Description("Publicar")]
        publish = 1,
        [Description("Processando")]
        processing = 2,
        [Description("Publicado")]
        done = 3,
        [Description("Erro")]
        error = 4
    }

    public enum ScratchcardStatus : int
    {
        [Description("Rascunho")]
        draft = 1,
        [Description("Gerado")]
        generated = 2,
        [Description("Ativo")]
        active = 3,
        [Description("Inativo")]
        inactive = 4,
        [Description("Processando")]
        generating = 5,
        [Description("Apagado")]
        deleted = 99
    }

    public enum ScratchcardDraw : int
    {
        [Description("Ativo")]
        active = 1,
        [Description("Distribuido")]
        drawn = 2,
        [Description("Raspado")]
        scratched = 3,
        [Description("Validado")]
        validated = 4
    }

    public enum ScratchcardType : int
    {
        [Description("Aberto")]
        opened = 1,
        [Description("Fechado")]
        closed = 2,
        [Description("Fechado + parceiro")]
        closedPartner = 3,
        [Description("Assinatura")]
        subscription = 4
    }

    public enum ScratchcardDistribution : int
    {
        [Description("Diária")]
        daily = 1,
        [Description("Semanal")]
        weekly = 2,
        [Description("Mensal")]
        monthly = 3
    }

    public enum StaticTextType : int
    {
        [Description("Operação Sobre")]
        AboutOperation = 1,
        [Description("Operação como funciona")]
        HowOperationWork = 2,
        [Description("Detalhes")]
        BenefitDetail = 3,
        [Description("Páginas")]
        Pages = 4,
        [Description("Como Utilizar")]
        BenefitHowToUse = 5,
        [Description("Descrição do Parceiro")]
        PartnerDescription = 6,
        [Description("E-mail de recuperação de senha")]
        EmailPasswordRecovery = 7,
        [Description("E-mail de validação de cliente")]
        EmailCustomerValidation = 8,
        [Description("E-mail")]
        Email = 9,
        [Description("Configuração de Operação")]
        OperationConfiguration = 10,
        [Description("Configuração de Operação - Padrão")]
        OperationConfigurationDefault = 11,
        [Description("Páginas - Padrão")]
        PagesDefault = 12,
        [Description("E-mail de recuperação de senha - Padrão")]
        EmailPasswordRecoveryDefault = 13,
        [Description("E-mail de validação de cliente - Padrão")]
        EmailCustomerValidationDefault = 14,
        [Description("E-mail - Padrão")]
        EmailDefault = 15,
        [Description("Descrição do curso")]
        CourseDescription = 16,
        [Description("Cursos - Perguntas Frequentes")]
        CourseFAQ = 17,
        [Description("Cursos - Regulamentos")]
        CourseRegulation = 18,
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

    public enum OperationPartnerCustomerStatus : int
    {
        [Description("Novo")]
        newCustomer = 1,
        [Description("Aprovado")]
        approved = 2,
        [Description("Reprovado")]
        reproved = 3,
        [Description("Cadastrado")]
        signed = 4,
        [Description("Apagado")]
        deleted = 5,
    }

    public enum Roles
    {
        [Description("Master")]
        master,
        [Description("Administrador")]
        administrator,
        [Description("Publicador")]
        publisher,
        [Description("Administrador Rebens")]
        administratorRebens,
        [Description("Publicador Rebens")]
        publisherRebens,
        [Description("Cliente")]
        customer,
        [Description("Admin Parceiro")]
        partnerAdministrator,
        [Description("Aprovador Parceiro")]
        partnerApprover,
        [Description("Promotor")]
        promoter
    }

    public enum GraduationTypeParent : int
    {
        [Description("Graduação")]
        graduation = 1,
        [Description("Pós Graduação")]
        postGradutaion = 2
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
