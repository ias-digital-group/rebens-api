using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.IdentityModel.Tokens;
using ias.Rebens.api.helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ServiceLocator<IAddressRepository>.Config(() => new AddressRepository(configuration));
            ServiceLocator<IAdminUserRepository>.Config(() => new AdminUserRepository(configuration));
            ServiceLocator<IBenefitTypeRepository>.Config(() => new BenefitTypeRepository(configuration));
            ServiceLocator<ICategoryRepository>.Config(() => new CategoryRepository(configuration));
            ServiceLocator<IContactRepository>.Config(() => new ContactRepository(configuration));
            ServiceLocator<IFaqRepository>.Config(() => new FaqRepository(configuration));
            ServiceLocator<IIntegrationTypeRepository>.Config(() => new IntegrationTypeRepository(configuration));
            ServiceLocator<ILogErrorRepository>.Config(() => new LogErrorRepository(configuration));
            ServiceLocator<IOperationRepository>.Config(() => new OperationRepository(configuration));
            ServiceLocator<IOperationTypeRepository>.Config(() => new OperationTypeRepository(configuration));
            ServiceLocator<IPermissionRepository>.Config(() => new PermissionRepository(configuration));
            ServiceLocator<IProfileRepository>.Config(() => new ProfileRepository(configuration));
            ServiceLocator<IStaticTextTypeRepository>.Config(() => new StaticTextTypeRepository(configuration));
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
            });

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

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rebens API");
            });
            app.UseAuthentication();
            app.UseMvc();

        }
    }
}
