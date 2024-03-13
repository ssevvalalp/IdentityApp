using IdentiyApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentiyApp.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users")]
    public class RoleUsersTagHelper:TagHelper
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HtmlAttributeName("asp-role-users")]
        public string RoleId { get; set; } = null!; // dışarıdan gönderilen role id bilgisi

       
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
           
            var userNames = new List<string>();

            //veri tabanından role ıd'ye göre bilgilerini almak
            var role = await _roleManager.FindByIdAsync(RoleId); 

            if(role != null && role.Name != null)
            {
                //role döngüsü role index'te dönmekteyken
                foreach(var user in _userManager.Users) // o an ulaşılan userin ilgili role içerisinde olup olmama durumu
                {
                    if(await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userNames.Add(user.UserName ?? ""); //varsa userin name'ini userNames Listesine ekle
                    }
                }
                //listeye alınan userNames Listesini geriye göndermek
                output.Content.SetContent(userNames.Count == 0 ? "kullanıcı yok" : string.Join(", ", userNames));


            }

        }
    }
}
