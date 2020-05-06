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

            ConfigureAuthentication(services);
            ConfigureDependencies(services);
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

            app.UseCors(
                options => options.WithOrigins(siteOptions.CurrentValue.Cors.Origins).AllowAnyMethod()
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