using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Models;
using Scheduling.Models.ViewModels;
using Scheduling.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _db; // To connect the Db to store/recall accounts
        // We use dependency injection to get UserManager & SignInManager methods implemented in IdentityDbContext
        UserManager<ApplicationUser> _userManager; // Create _userManager
        SignInManager<ApplicationUser> _signInManager;  // Create _signinManager
        RoleManager<IdentityRole> _roleManager;  // Create _roleManager

        // We use dependency injection to add them here
        public AccountController(AppDBContext db, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // server side validation
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // create session name variable
                    // if you forget await, user won't be accessible
                    var user =await _userManager.FindByNameAsync(model.Email);
                    // use httpcontext to store anything in the session
                    // setting session  
                    HttpContext.Session.SetString("ssuserName", user.Name);
                    // getting session data in controller
                    // var userName = HttpContext.Session.GetString("ssuserName");

                    return RedirectToAction("Index", "Appointment"); // if successful login
                }
                // This for the error part
                ModelState.AddModelError("", "Invalid Login Attempt"); // if unsuccessful login, show error and pass them
            }
            return View(model);
        }

        public async Task<IActionResult> Register()
        {
            // add "async Task<>" since you're using async functionality
            // to check if the role exists, and if not, we create one role for each
            // we use GetAwaiter().GetResult() to get the result before proceeding
            if (!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
            {
                // if role doesn't exist we create role
                await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Doctor));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Patient));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) // Refers to "RegisterViewModel" in Register.cshtml
        {
            // implement server side validation
            if(ModelState.IsValid)
            {
                //create the user 
                var user = new ApplicationUser
                {
                    //properties population
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                }; // the required properties

                var result = await _userManager.CreateAsync(user, model.Password); // helper methods to create a user, model.Password to check and hash passwords
                // await to queue the async methods
                // to use await we need to use "async Task<>" next to public
                if (result.Succeeded) // if the user is created, we sign in the user and redirect to homepage
                {
                    await _userManager.AddToRoleAsync(user, model.RoleName); // assign role to the user

                    // login the new user if ONLY is not an admin
                    if (!User.IsInRole(Helper.Admin))
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false); // to make user sign in
                    }
                    else
                    {
                        TempData["newAdminSignup"] = user.Name; // save admin username in tempdata
                    }
                    return RedirectToAction("Index", "Appointment");   // to redirect to Appointment
                }
                // Here we iterate on each and every condition and for every error show the detailed errors P.S SCREW THIS PW
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        // logout action
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
