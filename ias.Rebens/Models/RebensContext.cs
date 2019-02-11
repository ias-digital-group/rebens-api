 using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;


namespace ias.Rebens
{
    public partial class RebensContext : DbContext
    {
        private readonly string connectionString;

        public RebensContext(string connection)
        {
            this.connectionString = connection;
        }

        public RebensContext(DbContextOptions<RebensContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<Banner> Banner { get; set; }
        public virtual DbSet<BannerOperation> BannerOperation { get; set; }
        public virtual DbSet<Benefit> Benefit { get; set; }
        public virtual DbSet<BenefitAddress> BenefitAddress { get; set; }
        public virtual DbSet<BenefitCategory> BenefitCategory { get; set; }
        public virtual DbSet<BenefitOperation> BenefitOperation { get; set; }
        public virtual DbSet<BenefitOperationPosition> BenefitOperationPosition { get; set; }
        public virtual DbSet<BenefitType> BenefitType { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
        public virtual DbSet<FormContact> FormContact { get; set; }
        public virtual DbSet<FormEstablishment> FormEstablishment { get; set; }
        public virtual DbSet<IntegrationType> IntegrationType { get; set; }
        public virtual DbSet<LogError> LogError { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<OperationAddress> OperationAddress { get; set; }
        public virtual DbSet<OperationContact> OperationContact { get; set; }
        public virtual DbSet<OperationType> OperationType { get; set; }
        public virtual DbSet<Partner> Partner { get; set; }
        public virtual DbSet<PartnerAddress> PartnerAddress { get; set; }
        public virtual DbSet<PartnerContact> PartnerContact { get; set; }
        public virtual DbSet<StaticText> StaticText { get; set; }
        public virtual DbSet<StaticTextType> StaticTextType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.City).HasMaxLength(200);

                entity.Property(e => e.Complement).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(200);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasMaxLength(50);

                entity.Property(e => e.Longitude).HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Neighborhood).HasMaxLength(200);

                entity.Property(e => e.Number).HasMaxLength(50);

                entity.Property(e => e.State).HasMaxLength(200);

                entity.Property(e => e.Street).HasMaxLength(400);

                entity.Property(e => e.Zipcode).HasMaxLength(50);
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(300);

                entity.Property(e => e.EncryptedPassword).IsRequired().HasMaxLength(300);

                entity.Property(e => e.PasswordSalt).IsRequired().HasMaxLength(300);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");
            });

            modelBuilder.Entity<Bank>(entity => {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Initials).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Created).HasColumnType("datetime");
            });

            modelBuilder.Entity<BankAccount>(entity => {
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Branch).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.BankAccounts)
                    .HasForeignKey(d => d.IdBank)
                    .HasConstraintName("FK_BankAccount_Bank");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BankAccounts)
                    .HasForeignKey(d => d.IdCustomer)
                    .HasConstraintName("FK_BankAccount_Customer");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.Property(e => e.BackgroundColor).HasMaxLength(50);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.IdBenefit)
                    .HasConstraintName("FK_Banner_Benefit");
            });

            modelBuilder.Entity<BannerOperation>(entity =>
            {
                entity.HasKey(e => new { e.IdBanner, e.IdOperation });

                entity.HasOne(d => d.Banner)
                    .WithMany(p => p.BannerOperations)
                    .HasForeignKey(d => d.IdBanner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BannerOperation_Banner");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.BannerOperations)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BannerOperation_Operation");
            });

            modelBuilder.Entity<Benefit>(entity =>
            {
                entity.Property(e => e.CpvpercentageOffline)
                    .HasColumnName("CPVPercentageOffline")
                    .HasColumnType("money");

                entity.Property(e => e.CpvpercentageOnline)
                    .HasColumnName("CPVPercentageOnline")
                    .HasColumnType("money");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.MaxDiscountPercentageOffline).HasColumnType("money");

                entity.Property(e => e.MaxDiscountPercentageOnline).HasColumnType("money");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.WebSite).HasMaxLength(500);

                entity.HasOne(d => d.BenefitType)
                    .WithMany(p => p.Benefits)
                    .HasForeignKey(d => d.IdBenefitType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_BenefitType");

                entity.HasOne(d => d.IntegrationType)
                    .WithMany(p => p.Benefits)
                    .HasForeignKey(d => d.IdIntegrationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_IntegrationType");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Benefits)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_Partner");
            });

            modelBuilder.Entity<BenefitAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefit, e.IdAddress });

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.BenefitAddresses)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitAddress_Address");

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.BenefitAddresses)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitAddress_Benefit");
            });

            modelBuilder.Entity<BenefitCategory>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefit, e.IdCategory });

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.BenefitCategories)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitCategory_Benefit");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.BenefitCategories)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitCategory_Category");
            });

            modelBuilder.Entity<BenefitOperation>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefit, e.IdOperation });

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.BenefitOperations)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitOperation_Benefit");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.BenefitOperations)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitOperation_Operation");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.BenefitOperations)
                    .HasForeignKey(d => d.IdPosition)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitOperation_BenefitOperationPosition");
            });

            modelBuilder.Entity<BenefitOperationPosition>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<BenefitType>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.IdParent)
                    .HasConstraintName("FK_Category_Category");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.CellPhone).HasMaxLength(50);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(400);

                entity.Property(e => e.JobTitle).HasMaxLength(200);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.IdAddress)
                    .HasConstraintName("FK_Contact_Address");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);

                entity.Property(e => e.Gender).IsRequired().HasMaxLength(1);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Email).IsRequired().HasMaxLength(300);

                entity.Property(e => e.Cpf).HasMaxLength(50);

                entity.Property(e => e.RG).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Cellphone).HasMaxLength(50);

                entity.Property(e => e.EncryptedPassword).HasMaxLength(300);

                entity.Property(e => e.PasswordSalt).HasMaxLength(300);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");


                entity.HasOne(e => e.Operation)
                    .WithMany(e => e.Customers)
                    .HasForeignKey(e => e.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Operation");

                entity.HasOne(e => e.Address)
                    .WithMany(e => e.Customers)
                    .HasForeignKey(e => e.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Address");

            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.Faqs)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Faq_Operation");
            });

            modelBuilder.Entity<FormContact>(entity => {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.FormContacts)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FormContact_Operation");
            });

            modelBuilder.Entity<FormEstablishment>(entity => {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Establishment).HasMaxLength(500);
                entity.Property(e => e.WebSite).HasMaxLength(500);
                entity.Property(e => e.Responsible).HasMaxLength(300);
                entity.Property(e => e.ResponsibleEmail).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(300);
                entity.Property(e => e.State).HasMaxLength(50);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.FormEstablishments)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FormEstablishment_Operation");
            });

            modelBuilder.Entity<IntegrationType>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<LogError>(entity =>
            {
                entity.Property(e => e.Complement).HasMaxLength(500);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Reference)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StackTrace).HasColumnType("text");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(e => e.Parent)
                   .WithMany(e => e.Permissions)
                   .HasForeignKey(d => d.IdParent)
                   .HasConstraintName("FK_Permission_Permission");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.Property(e => e.CashbackPercentage).HasColumnType("money");

                entity.Property(e => e.CompanyDoc).HasMaxLength(50);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Domain)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(d => d.OperationType)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.IdOperationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_OperationType");
            });

            modelBuilder.Entity<OperationAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdOperation, e.IdAddress });

                entity.HasOne(d => d.Operations)
                    .WithMany(p => p.OperationAddresses)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationAddress_Operation");

                entity.HasOne(d => d.Addresses)
                    .WithMany(p => p.OperationAddresses)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationAddress_Address");
            });

            modelBuilder.Entity<OperationContact>(entity =>
            {
                entity.HasKey(e => new { e.IdOperation, e.IdContact });

                entity.HasOne(d => d.Operations)
                    .WithMany(p => p.OperationContacts)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationContact_Operation");

                entity.HasOne(d => d.Contacts)
                    .WithMany(p => p.OperationContacts)
                    .HasForeignKey(d => d.IdContact)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationContact_Contact");
            });

            modelBuilder.Entity<OperationType>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Logo)
                   .IsRequired()
                   .HasMaxLength(1000);
            });

            modelBuilder.Entity<PartnerAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdPartner, e.IdAddress });

                entity.HasOne(d => d.Addresses)
                    .WithMany(p => p.PartnerAddresses)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAddress_Address");

                entity.HasOne(d => d.Partners)
                    .WithMany(p => p.PartnerAddresses)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAddress_Partner");
            });

            modelBuilder.Entity<PartnerContact>(entity =>
            {
                entity.HasKey(e => new { e.IdPartner, e.IdContact });

                entity.HasOne(d => d.Contacts)
                    .WithMany(p => p.PartnerContacts)
                    .HasForeignKey(d => d.IdContact)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerContact_Contact");

                entity.HasOne(d => d.Partners)
                    .WithMany(p => p.PartnerContacts)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerContact_Partner");
            });

            modelBuilder.Entity<StaticText>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Html)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Style).HasMaxLength(4000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Url).HasMaxLength(500);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.StaticTexts)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaticText_Operation");

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.StaticTexts)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaticText_Benefit");

                entity.HasOne(d => d.StaticTextType)
                    .WithMany(p => p.StaticTexts)
                    .HasForeignKey(d => d.IdStaticTextType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaticText_StaticTextType");
            });

            modelBuilder.Entity<StaticTextType>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });
        }
    }
}
