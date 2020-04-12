using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenNetworkStatus.Converters;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services;
using OpenNetworkStatus.Services.IncidentServices;
using OpenNetworkStatus.Services.StatusServices;

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
            var builder = services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
            
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
            
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddApiVersioning();
            
            services.AddDbContext<StatusDataContext>(
                options => options.UseNpgsql(
                    Configuration.GetConnectionString("DatabaseContext"), 
                    x => x.MigrationsAssembly(typeof(StatusDataContext).Namespace)
                )
            );

            services.AddTransient<IncidentService>();
            services.AddTransient<SiteStatusCalculationService>();
            services.AddTransient<StatusDayGenerationService>();
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
                app.UseStatusCodePagesWithRedirects("/error/code/{0}");
                app.UseExceptionHandler("/error/code/500");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller}/{action=Index}/{id?}"
                );
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Status}/{action=Index}/{id?}"
                );
            });
        }
    }
}