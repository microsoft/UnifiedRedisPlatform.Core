using System;
using Autofac;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AppInsights.EnterpriseTelemetry.Web.Extension;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AppInsights.EnterpriseTelemetry.Web.Extension.Middlewares;
using Microsoft.UnifiedRedisPlatform.Manager.API.Dependency;
using Microsoft.UnifiedRedisPlatform.Manager.API.ExceptionHandlers;

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
            AddAuthentication(services);
            AddTelemetry(services, Configuration);
            AddSwaggerDocumentation(services);
            services.AddControllers();
            RegisterDependencies(services, Configuration);
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/2020-5-1-beta/swagger.json", "Unified Redis Platform Administration API");
                c.RoutePrefix = string.Empty;
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseEnterpriseTelemetry(Configuration);
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void AddAuthentication(IServiceCollection services)
        {       
            var tenant = Configuration.GetValue<string>("Authentication:AAD:Tenant");
            var instance = Configuration.GetValue<string>("Authentication:AAD:Instance");
            var authority = string.Format(instance, tenant);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.Audience = Configuration["Authentication:AAD:Audience"];
                       options.Authority = authority;
                   });
            services.AddAuthorization();
        }

        protected virtual void AddTelemetry(IServiceCollection services, IConfiguration configuration)
        {
            services.AddEnterpriseLogger(configuration);
            services.AddSingleton<IGlobalExceptionHandler, UnauthorizedUserExceptionHandler>();
            services.AddSingleton<IGlobalExceptionHandler, GenericExceptionHandler>();
        }

        protected virtual void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DependencyResolver(configuration));
            containerBuilder.Populate(services);
            ApplicationContainer = containerBuilder.Build();
        }

        protected virtual void AddSwaggerDocumentation(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("2020-5-1-beta", new OpenApiInfo
                {
                    Title = "Unifed Redis Platform Administration API",
                    Version = "2020-5-1-beta",
                    Description = "APIs for managing registered applications by partners' Admins",
                    Contact = new OpenApiContact
                    {
                        Name = "Field Experience Engineering Team",
                        Email = "fxpswe@microsoft.com"
                    }
                });

                try
                {
                    var documentationFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var documentationPath = Path.Combine(AppContext.BaseDirectory, documentationFile);
                    c.IncludeXmlComments(documentationPath);
                }
                catch
                {
                    // DO-Nothing if XML documentation fails
                }
            });
        }
    }
}
