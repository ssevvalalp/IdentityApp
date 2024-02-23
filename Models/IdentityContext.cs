using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentiyApp.Models
{
    public class IdentityContext: IdentityDbContext<IdentityUser>

    {
        //dışarıdan(development.json) options verilecek
        //ya da burada OnConfiguring Methodu içinde ConnectionString
        public IdentityContext(DbContextOptions<IdentityContext>options):base(options)
        {
            
        }
    }
}
