using AutoMapper;
using Manmudra.Contract.Settings;
using Manmudra.GroupsAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using System.IO.Compression;

namespace Manmudra.GroupsAPI.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void InjectService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();
            services.AddOptions();
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.InjectDependency();
            services.InjectRepository();
            services.InjectAuthentication();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CustomMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllers(
                    options =>
                    {
                        if (options.Filters.Any(
                            item => item is Microsoft.AspNetCore.Mvc.Authorization.IAllowAnonymousFilter))
                        {
                            return;
                        }
                        options.Filters.Add(new ApiExceptionFilter());
                    })
                    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

            services.AddHealthChecks();

            services.AddHttpClient();
            // Configure the services to service route requests.
            services.AddRouting();

            // Configure IIS.
            services.Configure<IISOptions>(
                options =>
                {
                    options.AutomaticAuthentication = true;
                    options.ForwardClientCertificate = false;
                });

            services.AddResponseCompression(
                options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
                });

            // Add compression and anti-forgery.
            services.AddAntiforgery();
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Manmudra Groups API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });

            services.AddMemoryCache();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
