using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Models
{
    // We implement the custom IdentityUser class called ApplicationUser using the notation <> to the IdentityDbContext inheritance
    // Next we have to reidentity the services at startup.cs and add "ApplicationUser" instead of "IdentityUser"
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // create db set for appointment

        public DbSet<Appointment> Appointments { get; set; }
    }
}
