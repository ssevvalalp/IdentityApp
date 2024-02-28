using System.ComponentModel.DataAnnotations;

namespace IdentiyApp.ViewModels
{
    public class EditViewModel
    {

        public string? Id { get; set; }


        public string? FullName { get; set; } 


    
        public string? Email { get; set; } 

      
        [DataType(DataType.Password)]
        public string? Password { get; set; } 


      
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Parola eşleşmiyor.")]
        public string? ConfirmPassword { get; set; } 
    
}
}
