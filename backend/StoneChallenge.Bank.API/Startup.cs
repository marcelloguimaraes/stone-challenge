using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StoneChallenge.Bank.API.Auth;
using StoneChallenge.Bank.API.Configurations;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.Services;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Infra.Data.Context;
using StoneChallenge.Bank.Infra.Data.Repository;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;

namespace StoneChallenge.Bank.API
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
            var connection = Configuration["ConnectionStrings:DefaultConnection"];

            // Services
            services.AddScoped<IAccountAppService, AccountAppService>();
            services.AddScoped<ITransactionAppService, TransactionAppService>();
            services.AddScoped<ICustomerAppService, CustomerAppService>();

            // Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<BankContext>();

            services.AddAutoMapperSetup();

            services.AddDbContext<BankContext>(options =>
                options.UseSqlite(connection).
                        UseLazyLoadingProxies());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Stone Challenge - Bank Api", Version = "v1" });
            });

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BankContext>()
                .AddDefaultTokenProviders();

            #region JWT
            var appSettingsSection = Configuration.GetSection("AuthSettings");
            services.Configure<AuthSettings>(appSettingsSection);

            var authSettings = appSettingsSection.Get<AuthSettings>();
            var key = Encoding.ASCII.GetBytes(authSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Audience,
                    ValidIssuer = authSettings.Issuer
                };
            }); 
            #endregion

            services.AddMvc(options =>
            {
                options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<BankContext>();

                context.Database.Migrate();
            }

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stone Challenge - Bank Api v1");
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
