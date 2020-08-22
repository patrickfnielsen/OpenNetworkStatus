using System;
using System.Text;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenNetworkStatus.Converters;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Models.Options;
using OpenNetworkStatus.Services;
using OpenNetworkStatus.Services.Authentication;
using OpenNetworkStatus.Services.Behaviors;
using OpenNetworkStatus.Services.StatusServices;
using Serilog;

namespace OpenNetworkStatus
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            Env = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            var builder = services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //Allow posting enums as strings and not only integers
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()); //Make sure API only returns UTC Timezone (in/out)
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(); //snake_case all properties
            });

            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
            
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddApiVersioning();
            
            services.Configure<SiteOptions>(Configuration.GetSection("site"));
            
            services.AddDbContext<StatusDataContext>(
                options => options.UseNpgsql(
                    Configuration.GetConnectionString("default"), 
                    x => x.MigrationsAssembly(typeof(StatusDataContext).Namespace)
                )
            );

            ConfigureSwagger(services);
            ConfigureAuthentication(services);
            ConfigureDependencies(services);
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = nameof(OpenNetworkStatus),
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = nameof(OpenNetworkStatus),
                        Email = string.Empty,
                        Url = new Uri("https://github.com/patrickfnielsen/OpenNetworkStatus/issues")
                    }
                });

                //Add JWT to swagger
                c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                });

                //Make sure Swagger UI requires a Bearer token specified
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "JWT",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new string[] { }
                    },
                });
            });
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            var siteOptions = new SiteOptions();
            Configuration.GetSection("site").Bind(siteOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                    {
                        var before = notBefore == null || notBefore <= DateTime.UtcNow;
                        var after = expires == null || expires >= DateTime.UtcNow;
                        return before && after;
                    },
                    ValidIssuer = siteOptions.Jwt.ValidIssuer,
                    ValidAudience = siteOptions.Jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(siteOptions.Jwt.IssuerSigningKey)),
                };
            });
        }

        private void ConfigureDependencies(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddTransient<SiteStatusCalculationService>();
            services.AddTransient<StatusDayGenerationService>();

            services.AddTransient<PasswordHasher>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ITokenManager, JwtTokenManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptionsMonitor<SiteOptions> siteOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), subApp =>
                {
                    subApp.UseStatusCodePagesWithRedirects("/error/code/{0}");
                    subApp.UseExceptionHandler("/error/code/500");
                });
            }

            //Added swagger if enabled in config
            if (siteOptions.CurrentValue.EnableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }

            //Cors
            app.UseCors(
                options => options.WithOrigins(siteOptions.CurrentValue.Cors.Origins).AllowAnyMethod().AllowAnyHeader()
            );

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
        
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Status}/{action=Index}/{id?}"
                );
            });
        }
    }
}
