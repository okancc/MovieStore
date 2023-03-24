using Microsoft.AspNetCore.Mvc;
using MovieStore.Models.DTO;
using MovieStore.Repositories.Abstract;

namespace MovieStore.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private IUserAuthenticationService authService;
        public UserAuthenticationController(IUserAuthenticationService authService)
        {
            this.authService= authService;

        }
        public async  Task<IActionResult> Register()
        {
            var model = new RegistrationModel
            {
                Email = "admin@gmail.com",
                UserName = "admin",
                Name = "Okan",
                Password = "Admin@123",
                PasswordConfirm = "Admin@123",
                Role = "Admin",

            };
           var result = await authService.RegisterAsync(model);
            return Ok(result.Message);
        }
    }
}
