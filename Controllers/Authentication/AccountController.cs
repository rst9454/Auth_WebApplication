﻿using Auth_WebApplication.Repostory.Interface;
using Auth_WebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis;
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

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
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

        public IActionResult ForgetPassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordOrUsernameVM forget)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Token");
            if (!ModelState.IsValid)
            {
                return View(forget);
            }
            var user = await userManager.FindByEmailAsync(forget.Email);
            if (user != null)
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, Token = code },
                    protocol: Request.Scheme);
                bool isSendEmail = await emailSender.EmailSendAsync(forget.Email, "Reset Password", "Please reset your password by clicking <a style='background-color:#04aa6d; border:none; color:white; padding:10px; text-align:center; text-decoration:none;dilplay:inline-block; font-size:16px; margin:4px 2px; cursor:pointer; border-radius:10px;' href=\""+callbackUrl+"\">Click Here</a>");
                if (isSendEmail)
                {
                    Response response = new Response();
                    response.Message = "Reset Password Link";
                    return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                }
            }
            return View();
        }

        public IActionResult ForgetPasswordConfirmation(Response response)
        {
            return View(response);
        }

        public IActionResult ResetPassword(string userId, string Token)
        {
            var model = new ForgetPasswordOrUsernameVM
            {
                Token = Token,
                UserId = userId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult>ResetPassword(ForgetPasswordOrUsernameVM forget)
        {
            Response response = new Response();
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View(forget);
            }
            var user = await userManager.FindByIdAsync(forget.UserId);
            if (user == null)
            {
                return View(forget);
            }
            var result = await userManager.ResetPasswordAsync(user, forget.Token, forget.Password);
            if (result.Succeeded)
            {
                response.Message = "Your password has been successfully Reset";
                return RedirectToAction("ForgetPasswordConfirmation", response);
            }
            return View(forget);
        }
    }
}
