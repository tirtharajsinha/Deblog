using Microsoft.AspNetCore.Identity;
using Deblog.Areas.Identity.Data;

namespace BulkyBookWeb
{
    public class AdminCreator
    {
        public async Task AdminUserCreator(IServiceProvider serviceProvider) { 
            await CreateRoles(serviceProvider);
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                // ensure that the role does not exist
                if (!roleExist)
                {
                    //create the roles and seed them to the database: 
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // find the user with the admin email 
            Console.WriteLine("searching User..");
            var _user = await UserManager.FindByEmailAsync("admin@email.com");

            Console.WriteLine("Search Done : " + Convert.ToString(_user));

            // check if the user exists
            if (_user == null)
            {
                //Here you could create the super admin who will maintain the web app
                var poweruser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "admin@email.com",
                    EmailConfirmed = true
                };
                string adminPassword = "admin";

           
                var createPowerUser = await UserManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Admin");
                    Console.WriteLine("Admin Created");

                }
                else
                {
                    Console.WriteLine("Error on admin creation"+ createPowerUser.Errors);
                }
            }
            else
            {
                Console.WriteLine("user Found : "+Convert.ToString(_user));
            }
        }
    }
}
