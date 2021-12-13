using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scheduling.Models;
using Scheduling.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        // inject these methods 
        // make sure to use ApplicationUser, it's important
        private readonly AppDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // this is identityrole not applicationuser IMP!!!!

        public DbInitializer(AppDBContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // inject complete
        public void initialize()
        {
            try
            {
                // if there's any pending migrations, push them
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    // push any pending migrations
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            // if there's any roles created, return, if not, create the roles with admin account
            if (_db.Roles.Any(x => x.Name == Helper.Admin)) return; // if admin role created, do nothing

            // if not, create the roles here
            // if role doesn't exist we create role
            // don't forget .GetAwaiter().GetResult(); with CreateAsync
            _roleManager.CreateAsync(new IdentityRole(Helper.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Doctor)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Patient)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "Main@Admin.com",
                Email = "Main@Admin.com",
                EmailConfirmed = true,
                Name = "Admin Spark"
            }, "Admin1234*").GetAwaiter().GetResult();

            // make the user admin
            ApplicationUser user = _db.Users.FirstOrDefault(u => u.Email == "Main@Admin.com");
            _userManager.AddToRoleAsync(user, Helper.Admin).GetAwaiter().GetResult();
        }
    }
}
