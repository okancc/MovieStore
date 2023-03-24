using System.ComponentModel.DataAnnotations;


namespace MovieStore.Models.DTO

{
    public class RegistrationModel
    {

        [Required]
        public string Name { get; set; }
     
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }
      
        [Required]

        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers of Password")]

        public string Password { get; set; }
        [Required]
        [Compare("Password")]

        public string PasswordConfirm { get; set; }

        public string  Role { get;  set; }




    }
}
