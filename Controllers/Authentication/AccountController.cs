using Auth_WebApplication.Repostory.Interface;
using Auth_WebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text;

namespace Auth_WebApplication.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender,IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chkEmail = await userManager.FindByEmailAsync(model.Email);
                    if (chkEmail != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email already exist");
                        return View(model);
                    }
                    var user = new IdentityUser
                    {
                        UserName = model.Email,
                        Email = model.Email
                    };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        
                        bool status = await emailSender.EmailSendAsync(model.Email, "User Registration", await GetEmailBody(model.Email));
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser checkEmail = await userManager.FindByEmailAsync(model.Email);
                    if (checkEmail == null)
                    {
                        ModelState.AddModelError(string.Empty, "Email not found");
                        return View(model);
                    }
                    if (await userManager.CheckPasswordAsync(checkEmail, model.Password) == false)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials");
                        return View(model);
                    }
                    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public async Task<string> GetEmailBody(string username)
        {
            string url = configuration.GetValue<string>("Urls:LoginUrl");
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Template/Welcome.cshtml");
            string htmlStrig = System.IO.File.ReadAllText(path);
            htmlStrig = htmlStrig.Replace("{{title}}", "User Registration");
            htmlStrig = htmlStrig.Replace("{{Username}}", username);
            htmlStrig = htmlStrig.Replace("{{url}}", url);
            return htmlStrig;
        }

    }
}
