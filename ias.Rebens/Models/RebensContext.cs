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
        public virtual DbSet<FreeCourseCategory> FreeCourseCategory { get; set; }
        public virtual DbSet<BenefitCategory> BenefitCategory { get; set; }
        public virtual DbSet<BenefitOperation> BenefitOperation { get; set; }
        public virtual DbSet<BenefitOperationPosition> BenefitOperationPosition { get; set; }
        public virtual DbSet<BenefitUse> BenefitUse { get; set; }
        public virtual DbSet<BenefitView> BenefitView { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Coupon> Coupon { get; set; }
        public virtual DbSet<CouponCampaign> CouponCampaign { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseAddress> CourseAddress { get; set; }
        public virtual DbSet<CourseCollege> CourseCollege { get; set; }
        public virtual DbSet<CourseCollegeAddress> CourseCollegeAddress { get; set; }
        public virtual DbSet<CourseCustomerRate> CourseCustomerRate { get; set; }
        public virtual DbSet<CourseCoursePeriod> CourseCoursePeriod { get; set; }
        public virtual DbSet<CourseGraduationType> CourseGraduationType { get; set; }
        public virtual DbSet<CourseModality> CourseModality { get; set; }
        public virtual DbSet<CoursePeriod> CoursePeriod { get; set; }
        public virtual DbSet<CourseUse> CourseUse { get; set; }
        public virtual DbSet<CourseView> CourseView { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerLog> CustomerLog { get; set; }
        public virtual DbSet<Draw> Draw { get; set; }
        public virtual DbSet<DrawItem> DrawItem { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<FileToProcess> FileToProcess { get; set; }
        public virtual DbSet<FormContact> FormContact { get; set; }
        public virtual DbSet<FormEstablishment> FormEstablishment { get; set; }
        public virtual DbSet<FreeCourse> FreeCourse { get; set; }
        public virtual DbSet<LogAction> LogAction { get; set; }
        public virtual DbSet<LogError> LogError { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<MoipInvoice> MoipInvoice { get; set; }
        public virtual DbSet<MoipNotification> MoipNotification { get; set; }
        public virtual DbSet<MoipPayment> MoipPayment { get; set; }
        public virtual DbSet<MoipSignature> MoipSignature { get; set; }
        public virtual DbSet<Operation> Operation { get; set; }
        public virtual DbSet<OperationAddress> OperationAddress { get; set; }
        public virtual DbSet<OperationPartner> OperationPartner { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<Partner> Partner { get; set; }
        public virtual DbSet<PartnerAddress> PartnerAddress { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<Scratchcard> Scratchcard { get; set; }
        public virtual DbSet<ScratchcardDraw> ScratchcardDraw { get; set; }
        public virtual DbSet<ScratchcardPrize> ScratchcardPrize { get; set; }
        public virtual DbSet<StaticText> StaticText { get; set; }
        public virtual DbSet<WirecardPayment> WirecardPayment { get; set; }
        public virtual DbSet<Withdraw> Withdraw { get; set; }
        public virtual DbSet<ZanoxIncentive> ZanoxIncentive { get; set; }
        public virtual DbSet<ZanoxIncentiveClick> ZanoxIncentiveClick { get; set; }
        public virtual DbSet<ZanoxProgram> ZanoxProgram { get; set; }
        public virtual DbSet<ZanoxProgramView> ZanoxProgramView { get; set; }
        public virtual DbSet<ZanoxSale> ZanoxSale { get; set; }

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
                
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(300);
                
                entity.Property(e => e.Doc).IsRequired().HasMaxLength(50);

                entity.Property(e => e.EncryptedPassword).IsRequired().HasMaxLength(300);

                entity.Property(e => e.PasswordSalt).IsRequired().HasMaxLength(300);
                
                entity.Property(e => e.Picture).HasMaxLength(500);
                entity.Property(e => e.PhoneMobile).HasMaxLength(50);
                entity.Property(e => e.PhoneComercial).HasMaxLength(50);
                entity.Property(e => e.PhoneComercialBranch).HasMaxLength(50);
                entity.Property(e => e.PhoneComercialMobile).HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(e => e.OperationPartner)
                    .WithMany(e => e.AdminUsers)
                    .HasForeignKey(e => e.IdOperationPartner)
                    .HasConstraintName("FK_AdminUser_OperationPartner");

                entity.HasOne(e => e.Operation)
                    .WithMany(e => e.AdminUsers)
                    .HasForeignKey(e => e.IdOperation)
                    .HasConstraintName("FK_AdminUser_Operation");
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
                entity.Property(e => e.Target).HasMaxLength(50);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Image).IsRequired().HasMaxLength(500);

                entity.Property(e => e.Link).IsRequired().HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.HasOne(d => d.Benefit).WithMany(p => p.Banners).HasForeignKey(d => d.IdBenefit).HasConstraintName("FK_Banner_Benefit");
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
                entity.Property(e => e.CPVPercentage).HasColumnType("money");

                entity.Property(e => e.CashbackAmount).HasColumnType("money");

                entity.Property(e => e.CPVPercentage).HasColumnType("money");

                entity.Property(e => e.CashbackAmount).HasColumnType("money");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.MaxDiscountPercentage).HasColumnType("money");

                entity.Property(e => e.MinDiscountPercentage).HasColumnType("money");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired().HasMaxLength(400);

                entity.Property(e => e.Link).HasMaxLength(500);

                entity.Property(e => e.Call).IsRequired().HasMaxLength(500);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Benefits)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_Partner");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.Benefits)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Benefit_Operation");
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

            modelBuilder.Entity<BenefitUse>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.BenefitUses)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitUse_Benefit");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BenefitUses)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitUse_Customer");
            });

            modelBuilder.Entity<BenefitView>(entity => {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.BenefitViews)
                    .HasForeignKey(d => d.IdBenefit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitView_Benefit");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BenefitViews)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BenefitView_Customer");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.IdParent)
                    .HasConstraintName("FK_Category_Category");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.IdItem).IsRequired();
                entity.Property(e => e.IdAddress).IsRequired();
                entity.Property(e => e.IdContact).IsRequired();

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.IdAddress)
                    .HasConstraintName("FK_Company_Address");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.IdContact)
                    .HasConstraintName("FK_Company_Contact");
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

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.Property(e => e.ValidationCode).IsRequired().HasMaxLength(50);

                entity.Property(e => e.Campaign).IsRequired().HasMaxLength(50);

                entity.Property(e => e.SingleUseCode).HasMaxLength(50);

                entity.Property(e => e.SingleUseUrl).HasMaxLength(300);

                entity.Property(e => e.WidgetValidationCode).HasMaxLength(50);

                entity.Property(e => e.OpenDate).HasColumnType("datetime");

                entity.Property(e => e.PlayedDate).HasColumnType("datetime");

                entity.Property(e => e.ClaimDate).HasColumnType("datetime");

                entity.Property(e => e.ClaimType).HasMaxLength(50);

                entity.Property(e => e.ValidationDate).HasColumnType("datetime");

                entity.Property(e => e.ValidationValue).HasMaxLength(500);

                entity.Property(e => e.ValidationDate).HasColumnType("datetime");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.Property(e => e.VerifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.CouponCampaign)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.IdCouponCampaign)
                    .HasConstraintName("FK_Coupon_CouponCampaign");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.IdCustomer)
                    .HasConstraintName("FK_Coupon_Customer");
            });

            modelBuilder.Entity<CouponCampaign>(entity =>
            {
                entity.Property(e => e.CampaignId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.OriginalPrice).IsRequired().HasColumnType("money");
                entity.Property(e => e.Discount).IsRequired().HasColumnType("money");
                entity.Property(e => e.FinalPrice).IsRequired().HasColumnType("money");
                entity.Property(e => e.Duration).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Image).HasMaxLength(500);
                entity.Property(e => e.Rating).IsRequired().HasColumnType("decimal");
                entity.Property(e => e.DueDate).HasColumnType("datetime");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.VoucherText).HasMaxLength(500);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_Operation");

                entity.HasOne(d => d.College)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdCollege)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_CourseCollege");

                entity.HasOne(d => d.GraduationType)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdGraduationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_CourseGraduationType");

                entity.HasOne(d => d.Modality)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdModality)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_CourseModality");

                entity.HasOne(d => d.AdminUser)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdAdminUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_AdminUser");

                entity.HasOne(d => d.Description)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdDescription)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Course_StaticText");

            });

            modelBuilder.Entity<CourseAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdCourse, e.IdAddress });

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseAddresses)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseAddress_Course");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.CourseAddresses)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseAddress_Address");
            });

            modelBuilder.Entity<CourseCollege>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Logo).HasMaxLength(500);


                entity.HasOne(d => d.Address)
                    .WithMany(p => p.CourseColleges)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCollege_Address");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.CourseColleges)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCollege_Operation");

            });

            modelBuilder.Entity<CourseCollegeAddress>(entity =>
            {
                entity.HasKey(e => new { e.IdCollege, e.IdAddress});

                entity.HasOne(d => d.College)
                    .WithMany(p => p.CollegeAddresses)
                    .HasForeignKey(d => d.IdCollege)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCollegeAddress_CourseCollege");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.CourseCollegeAddresses)
                    .HasForeignKey(d => d.IdAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCollegeAddress_Address");
            });

            modelBuilder.Entity<CourseCoursePeriod>(entity =>
            {
                entity.HasKey(e => new { e.IdCourse, e.IdPeriod });

                entity.HasOne(d => d.CoursePeriod)
                    .WithMany(p => p.CoursePeriods)
                    .HasForeignKey(d => d.IdPeriod)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCoursePeriod_CoursePeriod");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CoursePeriods)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCoursePeriod_Course");
            });

            modelBuilder.Entity<CourseCustomerRate>(entity =>
            {
                entity.HasKey(e => new { e.IdCourse, e.IdCustomer });

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseCustomerRates)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCustomerRate_Course");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CourseCustomerRates)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseCustomerRate_Customer");
            });

            modelBuilder.Entity<CourseGraduationType>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.CourseGraduationTypes)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseGraduationType_Operation");
            });

            modelBuilder.Entity<CourseModality>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.CourseModalities)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseModality_Operation");
            });

            modelBuilder.Entity<CoursePeriod>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.CoursePeriods)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CoursePeriod_Operation");
            });

            modelBuilder.Entity<CourseUse>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.OriginalPrice).IsRequired().HasColumnType("money");
                entity.Property(e => e.Discount).IsRequired().HasColumnType("money");
                entity.Property(e => e.FinalPrice).IsRequired().HasColumnType("money");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseUses)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseUse_Course");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CourseUses)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseUse_Customer");
            });

            modelBuilder.Entity<CourseView>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Course)
                                    .WithMany(p => p.CourseViews)
                                    .HasForeignKey(d => d.IdCourse)
                                    .OnDelete(DeleteBehavior.ClientSetNull)
                                    .HasConstraintName("FK_CourseView_Course");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CourseViews)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseView_Customer");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(300);

                entity.Property(e => e.Surname).HasMaxLength(300);

                entity.Property(e => e.Gender).HasMaxLength(1);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(300);

                entity.Property(e => e.Cpf).HasMaxLength(50);

                entity.Property(e => e.RG).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Cellphone).HasMaxLength(50);

                entity.Property(e => e.EncryptedPassword).HasMaxLength(300);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.PasswordSalt).HasMaxLength(300);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");
                
                entity.Property(e => e.SignUpDate).HasColumnType("datetime");

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

                entity.HasOne(e => e.CustomerReferer)
                    .WithMany(e => e.CustomerReferals)
                    .HasForeignKey(e => e.IdCustomerReferer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_CustomerReferer");

                entity.HasOne(e => e.OperationPartner)
                    .WithMany(e => e.Customers)
                    .HasForeignKey(e => e.IdOperationPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_OperationPartner");

                entity.HasOne(e => e.Promoter)
                    .WithMany(e => e.Customers)
                    .HasForeignKey(e => e.IdPromoter)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Promoter");
                
            });

            modelBuilder.Entity<CustomerLog>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(e => e.Customer)
                    .WithMany(e => e.CustomerLogs)
                    .HasForeignKey(e => e.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerLog_Customer");
            });

            modelBuilder.Entity<Draw>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(e => e.Operation)
                    .WithMany(e => e.Draws)
                    .HasForeignKey(e => e.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Draw_Operation");

            });

            modelBuilder.Entity<DrawItem>(entity =>
            {
                entity.Property(e => e.LuckyNumber).IsRequired().HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(e => e.Draw)
                    .WithMany(e => e.DrawItems)
                    .HasForeignKey(e => e.IdDraw)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DrawItem_Draw");

                entity.HasOne(e => e.Customer)
                    .WithMany(e => e.DrawItems)
                    .HasForeignKey(e => e.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DrawItem_Customer");

            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.Property(e => e.Answer)
                    .IsRequired();

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

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FileUrl)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<FileToProcess>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.FileToProcesses)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileToProcess_Operation");
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

            modelBuilder.Entity<FreeCourse>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Price).IsRequired().HasColumnType("money");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Summary).HasMaxLength(500);
                entity.Property(e => e.Image).HasMaxLength(300);
                entity.Property(e => e.ListImage).HasMaxLength(300);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.FreeCourses)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreeCourse_Operation");

                entity.HasOne(d => d.AdminUser)
                    .WithMany(p => p.FreeCourses)
                    .HasForeignKey(d => d.IdAdminUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreeCourse_AdminUser");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.FreeCourses)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreeCourse_Partner");
            });

            modelBuilder.Entity<FreeCourseCategory>(entity =>
            {
                entity.HasKey(e => new { e.IdFreeCourse, e.IdCategory });

                entity.HasOne(d => d.FreeCourse)
                    .WithMany(p => p.FreeCourseCategories)
                    .HasForeignKey(d => d.IdFreeCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreeCourseCategory_FreeCourse");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.FreeCourseCategories)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreeCourseCategory_Category");
            });

            modelBuilder.Entity<LogAction>(entity =>
            {
                entity.Property(e => e.Extra).HasMaxLength(500);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.AdminUser)
                       .WithMany(p => p.LogActions)
                       .HasForeignKey(d => d.IdAdminUser)
                       .OnDelete(DeleteBehavior.ClientSetNull)
                       .HasConstraintName("FK_LogAction_AdminUser");
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

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.Created).IsRequired().HasColumnType("datetime");

                entity.Property(e => e.Modified).IsRequired().HasColumnType("datetime");
            });

            modelBuilder.Entity<MoipInvoice>(entity =>
            {
                entity.Property(e => e.Status).IsRequired().HasMaxLength(500);

                entity.Property(e => e.Amount).IsRequired().HasColumnType("money");

                entity.Property(e => e.Created).IsRequired().HasColumnType("datetime");

                entity.Property(e => e.Modified).IsRequired().HasColumnType("datetime");

                entity.HasOne(d => d.MoipSignature)
                       .WithMany(p => p.Invoices)
                       .HasForeignKey(d => d.IdMoipSignature)
                       .OnDelete(DeleteBehavior.ClientSetNull)
                       .HasConstraintName("FK_MoipInvoice_MoipSignature");
            });

            modelBuilder.Entity<MoipNotification>(entity =>
            {
                entity.Property(e => e.Event).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Envoirement).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Resources).IsRequired().HasColumnType("text");

                entity.Property(e => e.Created).IsRequired().HasColumnType("datetime");

                entity.Property(e => e.Modified).IsRequired().HasColumnType("datetime");

            });

            modelBuilder.Entity<MoipPayment>(entity =>
            {
                entity.Property(e => e.MoipId).IsRequired().HasMaxLength(50);

                entity.Property(e => e.Amount).IsRequired().HasColumnType("money");

                entity.Property(e => e.Status).IsRequired().HasMaxLength(500);

                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

                entity.Property(e => e.Brand).HasMaxLength(50);

                entity.Property(e => e.HolderName).HasMaxLength(300);

                entity.Property(e => e.FirstSixDigits).HasMaxLength(50);

                entity.Property(e => e.LastFourDigits).HasMaxLength(50);

                entity.Property(e => e.Vault).HasMaxLength(50);

                entity.Property(e => e.Created).IsRequired().HasColumnType("datetime");

                entity.Property(e => e.Modified).IsRequired().HasColumnType("datetime");

                entity.HasOne(d => d.Invoice)
                       .WithMany(p => p.Payments)
                       .HasForeignKey(d => d.IdMoipInvoice)
                       .OnDelete(DeleteBehavior.ClientSetNull)
                       .HasConstraintName("FK_MoipPayment_MoipInvoice");

                entity.HasOne(d => d.Signature)
                       .WithMany(p => p.Payments)
                       .HasForeignKey(d => d.IdMoipSignature)
                       .OnDelete(DeleteBehavior.ClientSetNull)
                       .HasConstraintName("FK_MoipPayment_MoipSignature");
            });

            modelBuilder.Entity<MoipSignature>(entity =>
            {
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);

                entity.Property(e => e.PlanCode).IsRequired().HasMaxLength(50);

                entity.Property(e => e.PlanName).IsRequired().HasMaxLength(200);

                entity.Property(e => e.CreationDate).HasColumnType("date");

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.NextInvoiceDate).HasColumnType("date");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Amount).IsRequired().HasColumnType("money");

                entity.Property(e => e.Created).IsRequired().HasColumnType("datetime");

                entity.Property(e => e.Modified).IsRequired().HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                   .WithMany(p => p.Signatures)
                   .HasForeignKey(d => d.IdCustomer)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_MoipSignature_Customer");
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

                entity.Property(e => e.TemporarySubdomain)
                    .HasMaxLength(200);

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(o => o.LogError)
                    .WithMany(o => o.Operations)
                    .HasForeignKey(o => o.IdLogError)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_LogError");

                entity.HasOne(o => o.MainAddress)
                    .WithMany(o => o.Operations)
                    .HasForeignKey(o => o.IdMainAddress)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_MainAddress");

                entity.HasOne(o => o.MainContact)
                    .WithMany(o => o.Operations)
                    .HasForeignKey(o => o.IdMainContact)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_MainContact");
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

            modelBuilder.Entity<OperationPartner>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.OperationPartners)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OperationPartner_Operation");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Subtotal).IsRequired().HasColumnType("money");
                entity.Property(e => e.Discount).IsRequired().HasColumnType("money");
                entity.Property(e => e.Total).IsRequired().HasColumnType("money");

                entity.Property(e => e.DispId)
                    .IsRequired()
                    .HasMaxLength(50);


                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Customer");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Operation");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");
                
                entity.Property(e => e.UsedDate).HasColumnType("datetime");

                entity.Property(e => e.Price).IsRequired().HasColumnType("money");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Code)
                   .HasMaxLength(200);


                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.IdOrder)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_Order");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_Course");

                entity.HasOne(d => d.FreeCourse)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.IdFreeCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_FreeCourse");
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

                entity.HasOne(d => d.StaticText)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.IdStaticText)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Partner_StaticText");

                entity.HasOne(d => d.MainAddress)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.IdMainAddress)
                    .HasConstraintName("FK_Partner_MainAddress");

                entity.HasOne(d => d.MainContact)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.IdMainContact)
                    .HasConstraintName("FK_Partner_MainContact");
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

            modelBuilder.Entity<Scratchcard>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Start).HasColumnType("datetime");
                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.NoPrizeImage1).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage2).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage3).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage4).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage5).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage6).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage7).HasMaxLength(500);
                entity.Property(e => e.NoPrizeImage8).HasMaxLength(500);

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.Scratchcards)
                    .HasForeignKey(d => d.IdOperation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Scratchcard_Operation");
            });

            modelBuilder.Entity<ScratchcardPrize>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Image).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);

                entity.HasOne(d => d.Scratchcard)
                    .WithMany(p => p.Prizes)
                    .HasForeignKey(d => d.IdScratchcard)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScratchcardPrize_Scratchcard");
            });

            modelBuilder.Entity<ScratchcardDraw>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.OpenDate).HasColumnType("datetime");
                entity.Property(e => e.ValidationDate).HasColumnType("datetime");
                entity.Property(e => e.PlayedDate).HasColumnType("datetime");
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.Prize).HasMaxLength(200);
                entity.Property(e => e.ValidationDate).HasMaxLength(50);
                entity.Property(e => e.Image).HasMaxLength(500);

                entity.HasOne(d => d.Scratchcard)
                    .WithMany(p => p.Draws)
                    .HasForeignKey(d => d.IdScratchcard)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScratchcardDraw_Scratchcard");

                entity.HasOne(d => d.ScratchcardPrize)
                    .WithMany(p => p.Draws)
                    .HasForeignKey(d => d.IdScratchcardPrize)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScratchcardDraw_ScratchcardPrize");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Draws)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScratchcardDraw_Customerb");
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
            });

            modelBuilder.Entity<WirecardPayment>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Amount).IsRequired().HasColumnType("money");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.WirecardPayments)
                    .HasForeignKey(d => d.IdOrder)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WirecardPayment_Order");
            });

            modelBuilder.Entity<Withdraw>(entity => {
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                   .WithMany(p => p.Withdraws)
                   .HasForeignKey(d => d.IdCustomer)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Withdraw_Customer");

                entity.HasOne(d => d.BankAccount)
                   .WithMany(p => p.Withdraws)
                   .HasForeignKey(d => d.IdBankAccount)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Withdraw_BankAccount");
            });

            modelBuilder.Entity<ZanoxIncentive>(entity => {
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.ZanoxCreated).HasColumnType("datetime");
                entity.Property(e => e.ZanoxModified).HasColumnType("datetime");
                entity.Property(e => e.Start).HasColumnType("datetime");
                entity.Property(e => e.End).HasColumnType("datetime");
                
                entity.Property(e => e.IdProgram).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Url).HasMaxLength(300);

                entity.Property(e => e.Amount).HasColumnType("money");


                entity.HasOne(d => d.Program)
                  .WithMany(p => p.Incentives)
                  .HasForeignKey(d => d.IdProgram)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ZanoxIncentive_ZanoxProgram");
            });

            modelBuilder.Entity<ZanoxIncentiveClick>(entity => {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Incentive)
                    .WithMany(p => p.ZanoxIncentiveClicks)
                    .HasForeignKey(d => d.IdZanoxIncentive)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZanoxIncentiveClick_ZanoxIncentive");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ZanoxIncentiveClicks)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZanoxIncentiveClick_Customer");
            });

            modelBuilder.Entity<ZanoxProgram>(entity => {
                
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Created).HasColumnType("datetime");
                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                
                entity.Property(e => e.AdRank).HasColumnType("money");
                entity.Property(e => e.MaxCommissionPercent).HasColumnType("money");
                entity.Property(e => e.MinCommissionPercent).HasColumnType("money");

                entity.Property(e => e.Url).HasMaxLength(500);
                entity.Property(e => e.Image).HasMaxLength(500);
                entity.Property(e => e.Currency).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<ZanoxProgramView>(entity => {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.ZanoxProgramViews)
                    .HasForeignKey(d => d.IdZanoxProgram)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZanoxProgramView_ZanoxProgram");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ZanoxProgramViews)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZanoxProgramView_Customer");
            });

            modelBuilder.Entity<ZanoxSale>(entity => {
                entity.Property(e => e.TrackingDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ClickDate).HasColumnType("datetime");
                entity.Property(e => e.Modified).HasColumnType("datetime");
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.ClickId).HasColumnType("bigint");
                entity.Property(e => e.ClickInId).HasColumnType("bigint");

                entity.Property(e => e.Amount).HasColumnType("money");
                entity.Property(e => e.Commission).HasColumnType("money");

                entity.Property(e => e.ZanoxId).HasMaxLength(50);
                entity.Property(e => e.ReviewState).HasMaxLength(50);
                entity.Property(e => e.Currency).HasMaxLength(50);
                entity.Property(e => e.AdspaceValue).HasMaxLength(300);
                entity.Property(e => e.AdmediumValue).HasMaxLength(300);
                entity.Property(e => e.ProgramValue).HasMaxLength(300);
                entity.Property(e => e.Gpps).HasMaxLength(4000);
                entity.Property(e => e.ReviewNote).HasMaxLength(1000);
                entity.Property(e => e.Zpar).IsRequired().HasMaxLength(200);

                entity.HasOne(d => d.Customer)
                  .WithMany(p => p.ZanoxSales)
                  .HasForeignKey(d => d.IdCustomer)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Withdraw_Customer");
            });
        }
    }
}
