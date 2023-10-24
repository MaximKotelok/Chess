using DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;


        public DbInitializer(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            // migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }



            return;

        }
    }
}
