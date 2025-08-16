using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
}
