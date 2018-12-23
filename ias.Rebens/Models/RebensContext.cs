using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ias.Rebens
{
    public partial class RebensContext : DbContext
    {
        public RebensContext()
        {
        }

        public RebensContext(DbContextOptions<RebensContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Banner> Banner { get; set; }
        public virtual DbSet<BannerOperation> BannerOperation { get; set; }
        public virtual DbSet<Benefit> Benefit { get; set; }
        public virtual DbSet<BenefitAddress> BenefitAddress { get; set; }
        public virtual DbSet<BenefitCategory> BenefitCategory { get; set; }
        public virtual DbSet<BenefitOperation> BenefitOperation { get; set; }
        public virtual DbSet<BenefitOperationPosition> BenefitOperationPosition { get; set; }
        public virtual DbSet<BenefitType> BenefitType { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
        public virtual DbSet<IntegrationType> IntegrationType { get; set; }
        public virtual DbSet<LogError> LogError { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<OperationType> OperationType { get; set; }
        public virtual DbSet<Partner> Partner { get; set; }
        public virtual DbSet<PartnerAddress> PartnerAddress { get; set; }
        public virtual DbSet<StaticText> StaticText { get; set; }
        public virtual DbSet<StaticTextType> StaticTextTypw { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=IAS-02;Database=Rebens;user id=ias_user;password=k4r0l1n4;");
            }
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

                entity.HasOne(d => d.IdBenefitNavigation)
                    .WithMany(p => p.Banner)
                    .HasForeignKey(d => d.IdBenefit)
                    .HasConstraintName("FK_Banner_Benefit");
            });

            modelBuilder.Entity<BannerOperation>(entity =>
            {
                entity.HasKey(e => new { e.IdBanner, e.IdOperation });

                entity.HasOne(d => d.IdBannerNavigation)
                    .WithMany(p => p.BannerOperation)
                    .HasForeignKey(d => d.IdBanner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BannerOperation_Banner");

                entity.HasOne(d => d.IdOperationNavigation)
                    .WithMany(p => p.BannerOperation)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BannerOperation_Operation");
            });

            modelBuilder.Entity<Benefit>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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

                entity.HasOne(d => d.IdBenefitTypeNavigation)
                    .WithMany(p => p.Benefit)
                    .HasForeignKey(d => d.IdBenefitType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_BenefitType");

                entity.HasOne(d => d.IdIntegrationTypeNavigation)
                    .WithMany(p => p.Benefit)
                    .HasForeignKey(d => d.IdIntegrationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_IntegrationType");

                entity.HasOne(d => d.IdPartnerNavigation)
                    .WithMany(p => p.Benefit)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_Partner");
            });

            modelBuilder.Entity<BenefitAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefit, e.IdAddress });

                entity.HasOne(d => d.IdAddressNavigation)
                    .WithMany(p => p.BenefitAddress)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitAddress_Address");

                entity.HasOne(d => d.IdBenefitNavigation)
                    .WithMany(p => p.BenefitAddress)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitAddress_Benefit");
            });

            modelBuilder.Entity<BenefitCategory>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefit, e.IdCategory });

                entity.HasOne(d => d.IdBenefitNavigation)
                    .WithMany(p => p.BenefitCategory)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitCategory_Benefit");

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.BenefitCategory)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitCategory_Category");
            });

            modelBuilder.Entity<BenefitOperation>(entity =>
            {
                entity.HasKey(e => new { e.IdBenefit, e.IdOperation });

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.IdBenefitNavigation)
                    .WithMany(p => p.BenefitOperation)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitOperation_Benefit");

                entity.HasOne(d => d.IdOperationNavigation)
                    .WithMany(p => p.BenefitOperation)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitOperation_Operation");

                entity.HasOne(d => d.IdPositionNavigation)
                    .WithMany(p => p.BenefitOperation)
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

                entity.HasOne(d => d.IdAddressNavigation)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.IdAddress)
                    .HasConstraintName("FK_Contact_Address");
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

                entity.HasOne(d => d.IdOperationNavigation)
                    .WithMany(p => p.Faq)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Faq_Operation");
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

                entity.HasOne(d => d.IdOperationTypeNavigation)
                    .WithMany(p => p.Operation)
                    .HasForeignKey(d => d.IdOperationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_OperationType");
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

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.IdContactNavigation)
                    .WithMany(p => p.Partner)
                    .HasForeignKey(d => d.IdContact)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Partner_Contact");
            });

            modelBuilder.Entity<PartnerAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdPartner, e.IdAddress });

                entity.HasOne(d => d.IdAddressNavigation)
                    .WithMany(p => p.PartnerAddress)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAddress_Address");

                entity.HasOne(d => d.IdPartnerNavigation)
                    .WithMany(p => p.PartnerAddress)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartnerAddress_Partner");
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
                    .HasMaxLength(50);

                entity.Property(e => e.Url).HasMaxLength(200);

                entity.HasOne(d => d.IdOperationNavigation)
                    .WithMany(p => p.StaticText)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaticText_Operation");

                entity.HasOne(d => d.IdStaticTextTypeNavigation)
                    .WithMany(p => p.StaticText)
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
