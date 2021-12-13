using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scheduling.DbInitializer;
using Scheduling.Models;
using Scheduling.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling
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
            services.AddControllersWithViews();
            // configure DB link
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SchedulingDB")));

            // configure dependency injection to partial view
            // We configure the first parameter in AddIdentity to be the name of our custom identity model
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDBContext>();

            // we build dependency injection and register our service with limited lifetime "transient"
            services.AddTransient<IAppointmentService, AppointmentService> ();
            services.AddHttpContextAccessor();

            // to save session
            services.AddDistributedMemoryCache();

            // sessions options
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // add dbinitalizer usiner services.addscoped
            services.AddScoped<IDbInitializer, DbInitializer.DbInitializer>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // add IDbInitalizer to the class to be able to invoke the idbinitalizer initalize method
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitalizer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // to be able to login
            app.UseAuthorization();
            // invoke the idbinitalizer
            dbInitalizer.initialize();
            // to use sessions
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
