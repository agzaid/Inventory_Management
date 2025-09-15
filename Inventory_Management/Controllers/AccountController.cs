using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Register(string identifier, string password)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            TempData["error"] = "Email or phone number is required.";
            return View();
        }

        ApplicationUser user;

        if (identifier.Contains("@"))
        {
            // Email registration
            user = new ApplicationUser
            {
                UserName = identifier,
                Email = identifier
            };
        }
        else
        {
            // Phone registration
            user = new ApplicationUser
            {
                UserName = identifier,   // still needs a UserName for identity
                PhoneNumber = identifier,
                Email = null
            };
        }

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            TempData["success"] = "Account Created Successfully";
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        TempData["error"] = result.Errors?.FirstOrDefault()?.Description;
        return View();
    }



    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login() => View();

    // POST: /Account/Login
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Login(string identifier, string password)
    {
        ApplicationUser user = null;

        if (identifier.Contains("@"))
        {
            // Try find by email
            user = await _userManager.FindByEmailAsync(identifier);
        }
        else
        {
            // Try find by phone
            user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == identifier);
        }

        if (user == null)
        {
            TempData["error"] = "Invalid login attempt.";
            return View();
        }

        // Use UserName for sign in
        var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

        if (result.Succeeded)
        {
            TempData["success"] = "Signed in Successfully";
            return RedirectToAction("Index", "Home");
        }

        TempData["error"] = "Invalid login attempt.";
        return View();
    }


    // POST: /Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ModelState.AddModelError("", "Please enter your email.");
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email); // assuming Identity
        if (user == null)
        {
            // Don’t reveal that user doesn’t exist
            TempData["error"] = "email was not found";
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Build reset link
        var resetLink = Url.Action(
            "ResetPassword", "Account",
            new { token, email = user.Email },
            protocol: HttpContext.Request.Scheme);

        // TODO: Send email (use SMTP, SendGrid, etc.)
        // TODO: Send email (use SMTP, SendGrid, etc.)
        await _emailSender.SendEmailAsync(user.Email, "Reset Password",
            $"Click <a href='{resetLink}'>here</a> to reset your password.");

        TempData["success"] = "An email was sent to your email";
        return RedirectToAction("login");
    }
    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
            return RedirectToAction("Index", "Home");

        return View(new ResetPasswordViewModel { Token = token, Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["error"]="email was not found";
            return View();
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            TempData["success"] = "Password has been reset successfully";
            return RedirectToAction("login");
        }

        TempData["error"] = "Something went wrong";

        return View();
    }

}
