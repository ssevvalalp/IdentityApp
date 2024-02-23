using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentiyApp.Models
{
    public class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Admin_123";

        //program.cs'deki app bilgisini buraya parametre olarak göndercez
        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            // IdentityContext service konteyner'ından alınacak. app üzerinden bu konteynera erişilebiliyor

            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();
            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();//database update
            }
            //hazır servisler

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync(adminUser);

            if (user == null) {

                user = new IdentityUser
                {
                    UserName = adminUser,
                    Email = "admin@admin.com",
                    PhoneNumber = "444444"
                };

                await userManager.CreateAsync(user, adminPassword); //password hashleniyor
            }

        }
    }
}
