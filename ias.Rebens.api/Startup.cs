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

namespace ias.Rebens.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenOptions();
            tokenConfigurations.Issuer = "Rebens";
            tokenConfigurations.Audience = "Rebens";
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
                c.SwaggerDoc("v1", new Info { Title = "Rebens API", Version = "v1" });
                c.AddSecurityDefinition("bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Autenticação baseada em Json Web Token (JWT)",
                        Name = "Authorization",
                        Type = "apiKey"
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
            services.AddTransient<IBenefitRepository, BenefitRepository>();
            services.AddTransient<IBenefitTypeRepository, BenefitTypeRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<IFaqRepository, FaqRepository>();
            services.AddTransient<IIntegrationTypeRepository, IntegrationTypeRepository>();
            services.AddTransient<ILogErrorRepository, LogErrorRepository>();
            services.AddTransient<IOperationRepository, OperationRepository>();
            services.AddTransient<IOperationTypeRepository, OperationTypeRepository>();
            services.AddTransient<IPartnerRepository, PartnerRepository>();
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddTransient<IStaticTextRepository, StaticTextRepository>();
            services.AddTransient<IStaticTextTypeRepository, StaticTextTypeRepository>();

            services.AddDbContext<RebensContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            app.UseMvc();
        }
    }
}
