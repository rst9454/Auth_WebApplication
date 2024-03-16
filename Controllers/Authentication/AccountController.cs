﻿using Auth_WebApplication.Data;
using Auth_WebApplication.Models.IdentityModel;
using Auth_WebApplication.Repostory.Interface;
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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;
        private readonly ApplicationContext context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, ApplicationContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
            this.context = context;
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
            Response response = new Response();
            try
            {
                ModelState.Remove("ModifiedOn");
                if (ModelState.IsValid)
                {
                    var chkEmail = await userManager.FindByEmailAsync(model.Email);
                    if (chkEmail != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email already exist");
                        return View(model);
                    }
                    var isUsernameExist = context.Users.Where(e => e.UserName == model.Username).Any();
                    if (isUsernameExist)
                    {
                        ModelState.AddModelError("", "Username not available");
                        return View(model);
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        FirstName=model.FirstName,
                        LastName=model.LastName,
                        Gender=model.Gender,
                        Status=model.Status,
                        BirthDate=model.BirthDate
                    };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(user);
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmMail", "Account", new { userId = userId, Token = code },
                            protocol: Request.Scheme);
                        string emailBody = GetEmailBody(model.Email, "Email Confirmation", confirmationLink, "EmailConfirmation");
                        bool status = await emailSender.EmailSendAsync(model.Email, "Email Confirmation", emailBody);
                        if (status)
                        {
                            response.Message = "Please check your email for the verification action.";
                            return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                        }
                        //await signInManager.SignInAsync(user, isPersistent: false);
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
        [HttpGet]
        public async Task<IActionResult> ConfirmMail(string userId, string Token)
        {
            Response response = new Response();
            if (userId != null && Token != null)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return View("Error");
                }
                else
                {
                    var result = await userManager.ConfirmEmailAsync(user, Token);
                    if (result.Succeeded)
                    {
                        response.Message = "Thank you for confirming your eamil.";
                        return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                    }
                }
            }
            return View("Error");
        }

        public async Task<IActionResult> Login()
        {
            await Logout();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser checkEmail = await userManager.FindByEmailAsync(model.Email);
                    if (checkEmail == null)
                    {
                        ModelState.AddModelError(string.Empty, "Email not found");
                        return View(model);
                    }
                    if (await userManager.CheckPasswordAsync(checkEmail, model.Password) == false)
                    {
                        ModelState.AddModelError(string.Empty, "Incorrect Password");
                        return View(model);
                    }
                    bool confirmStatus = await userManager.IsEmailConfirmedAsync(checkEmail);
                    if (!confirmStatus)
                    {
                        ModelState.AddModelError("", "Email not confirmed");
                        return View(model);
                    }
                    else
                    {
                        var result = await signInManager.PasswordSignInAsync(checkEmail.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    }


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
        public string GetEmailBody(string? username, string? title, string? callbackUrl, string? EmailTemplateName)
        {
            string url = configuration.GetValue<string>("Urls:LoginUrl");
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Template/" + EmailTemplateName + ".cshtml");
            string htmlStrig = System.IO.File.ReadAllText(path);
            htmlStrig = htmlStrig.Replace("{{title}}", title);
            htmlStrig = htmlStrig.Replace("{{Username}}", username);
            htmlStrig = htmlStrig.Replace("{{url}}", url);
            htmlStrig = htmlStrig.Replace("{{callbackUrl}}", callbackUrl);
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
                string emailBody = GetEmailBody("", "Reset Password", callbackUrl, "ResetPassword");

                bool isSendEmail = await emailSender.EmailSendAsync(forget.Email, "Reset Password", emailBody);
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
        public async Task<IActionResult> ResetPassword(ForgetPasswordOrUsernameVM forget)
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
