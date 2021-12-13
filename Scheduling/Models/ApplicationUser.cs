using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Models
{
    // Here we make the new model use the (IdentityUser) class via inheritance, so we can add new properties
    // Next we go to AppDbContext and implement this model to the inheritied class >>>
    public class ApplicationUser: IdentityUser 
    {
        // Adding new properties to show in the Db when migration happens
        public string Name { get; set; } // New property "Name" to show column name in the db
    }
}
