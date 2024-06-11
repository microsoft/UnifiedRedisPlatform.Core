using System;
using Autofac;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AppInsights.EnterpriseTelemetry.Web.Extension;
using Microsoft.UnifiedPlatform.Service.Common.Caching;
using AppInsights.EnterpriseTelemetry.Web.Extension.Filters;
using Microsoft.UnifiedRedisPlatform.Service.API.Middlewares;
using AppInsights.EnterpriseTelemetry.Web.Extension.Middlewares;
using Microsoft.UnifiedRedisPlatform.Service.API.ExceptionHandler;
using Microsoft.UnifiedRedisPlatform.Service.API.DependencyResolution;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; protected set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //To do : MISE claims-based authorization module is not available as per the documentation.Once available,we need to make the changes
            //https://identitydivision.visualstudio.com/DevEx/_git/MISE?path=/docs/MigrationGuides/JwtBearerHandler/Readme.md&_a=preview
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretProvider = new KeyVaultProvider(
                        Configuration["KeyVault:Name"],
                        new InMemoryCache(new MemoryCache(new MemoryCacheOptions())));
                    var signingKey = secretProvider.GetSecret("Authentication-RedisCluster-Secret").Result;
                   
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Authentication:RedisCluster:Issuer"],
                        ValidAudience = Configuration["Authentication:RedisCluster:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                    };
                })
                .AddJwtBearer("AzureAD", options =>
                {
                    options.Audience = Configuration["Authentication:AAD:Audience"];
                    options.Authority = Configuration["Authentication:AAD:Authority"];

                });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, "AzureAD");
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddEnterpriseTelemetry(Configuration);

            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
            services.AddControllers(options =>
            {
                options.Filters.Add<TrackingPropertiesFilterAttribute>();
            }).AddNewtonsoftJson();

            RegisterDependencies(services, Configuration);
            return new AutofacServiceProvider(ApplicationContainer);
        }

        protected virtual void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DependencyResolver(configuration));
            containerBuilder.Populate(services);
            ApplicationContainer = containerBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEnterpriseTelemetry(Configuration);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
