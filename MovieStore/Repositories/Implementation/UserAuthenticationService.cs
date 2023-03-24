using Microsoft.AspNetCore.Identity;
using MovieStore.Models.Domain;
using MovieStore.Models.DTO;
using MovieStore.Repositories.Abstract;
using System.Security.Claims;


namespace MovieStore.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;

        }

        public async Task<Status> RegisterAsync(RegistrationModel model)

        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exist";
                return status;

            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Name = model.Name,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,

            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation Failed";
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);

            }
            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;






        }


        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user== null)
            {

                status.StatusCode = 0;
                status.Message = "Invalid Username";
                return status;

            }
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Invalid Username";
                return status;


            }
            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                }
                status.StatusCode = 1;
                status.Message = "Logged in successfully";


            }

            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User is lockedout";

            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on  logging  in";



            }
            return status;


        }
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }











}
}
