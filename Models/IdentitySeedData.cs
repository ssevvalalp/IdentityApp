using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentiyApp.Models
{
    public class IdentitySeedData
    {
        private const string adminUser = "admin";
        private const string adminPassword = "Admin_123";

        //program.cs'deki app bilgisini(builder dosyasını) buraya parametre olarak göndercez
        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            // IdentityContext servisi, service konteyner'ından alınacak. app üzerinden bu konteynera erişilebiliyor

            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

           // if (context.Database.GetAppliedMigrations().Any()) => migration oluşturulmuş ve uygulanmış olan varsa
                if (context.Database.GetPendingMigrations().Any()) //=> migration oluşturulmuş ancak uygulanmamış varsa
            {
                context.Database.Migrate();//migrate = database update
            }
            //hazır servisler

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(adminUser); //usermanager üzerinden bir user(aslında admin user) olusturma

            if (user == null) {

                user = new AppUser // new IdentityUser
                {
                    FullName = "Şevval Alp",
                    UserName = adminUser,
                    Email = "admin@admin.com",
                    PhoneNumber = "444444"
                };

                await userManager.CreateAsync(user, adminPassword); //password hashleniyor
            }

        }
    }
}
