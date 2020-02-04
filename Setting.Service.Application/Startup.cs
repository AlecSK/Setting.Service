using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Setting.Service.Application.Helpers;
using Setting.Service.Application.Interfaces;
using Setting.Service.Application.Utils;
using Setting.Service.Common;
using Setting.Service.Contract;
using Setting.Service.Contract.DtoModels;
using Setting.Service.Contract.Interfaces;
using Setting.Service.DataAccess;
using Setting.Service.Implementation;

[assembly: ApiConventionType(typeof(ResponseConventions))]
namespace Setting.Service.Application
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAppSettings, AppSettings>();

            services.AddSingleton<ICacheService<CacheableList<ConfigurationModule>>, CacheService<CacheableList<ConfigurationModule>>>();

            services.AddSingleton<ICacheService<CacheableList<ConfigurationSetting>>>(provider =>
            {
                var appSettings = provider.GetService<IAppSettings>();
                return new CacheService<CacheableList<ConfigurationSetting>>(new MemoryCacheOptions
                {
                    SizeLimit = appSettings.MaxSettingCountInCache
                });
            });

            string connection = Configuration.GetConnectionString("MasterDataDb");
            services.AddDbContext<MasterDataDbContext>(options =>
            {
                //options.UseSqlServer(connection);
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            services.AddScoped<ICachingRepository, CachingRepository>();

            services.AddCors();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Use the default property (Pascal) casing
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressInferBindingSourcesForParameters = true;
                        options.SuppressModelStateInvalidFilter = true;
                        options.SuppressMapClientErrors = false;
                        ClientErrorsSetup.Setup(options.ClientErrorMapping);
                    })
                ;


            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Setting.Service",
                    Description = "API для работы с сервисом настроек",
                    TermsOfService = new Uri("https://swagger.io"),
                    Contact = new OpenApiContact
                    {
                        Name = "Open API Project",
                        Email = string.Empty,
                        Url = new Uri("https://swagger.io/docs/specification/about/"),
                    },
                });

                c.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    //Description = "Для доступа к API необходимо авторизоваться!",
                    Description = "!!!! Для доступа можно использовать учетные данные - user1 : password1",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basicAuth" }
                        },
                        new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
                c.CustomSchemaIds(SchemaIdSelector);
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        private string SchemaIdSelector(Type type)
        {

            if (type == typeof(DtoConfigurationModule)) 
                return "Module";

            else if (type == typeof(DtoConfigurationSetting)) 
                return "Setting";

            else if (type == typeof(ProblemDetails)) 
                return "ProblemDetails";

            return type.FullName;
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.ConfigureCustomExceptionMiddleware();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = AppConstants.SwaggerDocumentTitle;
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "Setting.Service OpenApi3 v1.0.0");
                c.RoutePrefix = string.Empty;
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
