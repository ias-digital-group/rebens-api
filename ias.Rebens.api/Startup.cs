using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using ias.Rebens.api.helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.DotNet.PlatformAbstractions;
using Serilog;
using Microsoft.Extensions.Logging;

namespace ias.Rebens.api
{
    /// <summary>
    /// Startup Class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure Services Method
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenOptions()
            {
                Issuer = "Rebens",
                Audience = "Rebens"
            };
            new ConfigureFromConfigurationOptions<TokenOptions>(Configuration.GetSection("TokenOptions")).Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Rebens API", Version = "v1" });
                c.AddSecurityDefinition("bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Autenticação baseada em Json Web Token (JWT)",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });

                string appPath = ApplicationEnvironment.ApplicationBasePath;
                string appName = "ias.Rebens.api"; // ApplicationEnvironment.ApplicationName;
                string xmlDocumentPath = Path.Combine(appPath, $"{appName}.xml");
                //var xmlDocumentPath = "ias.Rebens.api.xml";

                if (File.Exists(xmlDocumentPath))
                {
                    c.IncludeXmlComments(xmlDocumentPath);
                }
            });

            services.AddTransient<IAddressRepository, AddressRepository>();
            services.AddTransient<IAdminUserRepository, AdminUserRepository>();
            services.AddTransient<IBankAccountRepository, BankAccountRepository>();
            services.AddTransient<IBannerRepository, BannerRepository>();
            services.AddTransient<IBenefitRepository, BenefitRepository>();
            services.AddTransient<IBenefitUseRepository, BenefitUseRepository>();
            services.AddTransient<IBenefitViewRepository, BenefitViewRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<ICouponCampaignRepository, CouponCampaignRepository>();
            services.AddTransient<ICouponRepository, CouponRepository>();
            services.AddTransient<ICourseCollegeRepository, CourseCollegeRepository>();
            services.AddTransient<ICourseGraduationTypeRepository, CourseGraduationTypeRepository>();
            services.AddTransient<ICourseModalityRepository, CourseModalityRepository>();
            services.AddTransient<ICoursePeriodRepository, CoursePeriodRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<ICourseUseRepository, CourseUseRepository>();
            services.AddTransient<ICourseViewRepository, CourseViewRepository>();
            services.AddTransient<ICustomerReferalRepository, CustomerReferalRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerPromoterRepository, CustomerPromoterRepository>();
            services.AddTransient<IDrawRepository, DrawRepository>();
            services.AddTransient<IFaqRepository, FaqRepository>();
            services.AddTransient<IFileToProcessRepository, FileToProcessRepository>();
            services.AddTransient<IFormContactRepository, FormContactRepository>();
            services.AddTransient<IFormEstablishmentRepository, FormEstablishmentRepository>();
            services.AddTransient<ILogErrorRepository, LogErrorRepository>();
            services.AddTransient<IFreeCourseRepository, FreeCourseRepository>();
            services.AddTransient<IModuleRepository, ModuleRepository>();
            services.AddTransient<IMoipNotificationRepository, MoipNotificationRepository>();
            services.AddTransient<IMoipRepository, MoipRepository>();
            services.AddTransient<IOperationRepository, OperationRepository>();
            services.AddTransient<IOperationCustomerRepository, OperationCustomerRepository>();
            services.AddTransient<IOperationPartnerRepository, OperationPartnerRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IPartnerRepository, PartnerRepository>();
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddTransient<IScratchcardRepository, ScratchcardRepository>();
            services.AddTransient<IScratchcardDrawRepository, ScratchcardDrawRepository>();
            services.AddTransient<IScratchcardPrizeRepository, ScratchcardPrizeRepository>();
            services.AddTransient<IStaticTextRepository, StaticTextRepository>();
            services.AddTransient<IWirecardPaymentRepository, WirecardPaymentRepository>();
            services.AddTransient<IWithdrawRepository, WithdrawRepository>();
            services.AddTransient<IZanoxSaleRepository, ZanoxSaleRepository>();
            

            services.AddDbContext<RebensContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        /// <summary>
        /// Configure Method
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // https redirection
            app.UseHttpsRedirection();
            
            // initilize swagger to generate the documentation
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rebens API");
            });

            // autentication, to block the anonimous requests
            app.UseAuthentication();

            // to use the static files with the defaults
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // enable CORs 
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            // MVC
            app.UseMvc(routes =>
            {
                routes.MapRoute("CourseVoucher", "Voucher/Course/{code}", defaults: new { controller = "Voucher", action = "Course" } );
                routes.MapRoute("Voucher", "Voucher/{code}", defaults: new { controller = "Voucher", action = "Index" } );
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            IServiceScopeFactory serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            //FluentScheduler.JobManager.Initialize(new SchedulerRegistry(serviceScopeFactory));

            Rotativa.AspNetCore.RotativaConfiguration.Setup(env);
        }
    }
}
