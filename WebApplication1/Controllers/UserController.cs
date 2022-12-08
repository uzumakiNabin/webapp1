using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using System.Security.Claims;
using Microsoft.Win32;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DbaseContext _userContext;
        public UserController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DbaseContext dbaseContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userContext = dbaseContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, false);
                if (result.Succeeded)
                {
                    /*
                    var user = await _userManager.FindByEmailAsync(login.UserName);
                    if (await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        return RedirectToAction("Index", "Movies");
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Create", "Movies");
                    }

                    var userClaims = await _userManager.GetClaimsAsync(user);
                    if (!userClaims.Any(x => x.Type == "Department"))
                    {
                        ModelState.AddModelError("Claims", "No claims");
                        return View(signin);
                    }
                    */
                    IdentityUser user = await _userManager.FindByNameAsync(login.UserName);
                    List<string> Roles = await _userManager.GetRolesAsync(user) as List<string>;
                    if (Roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("UploadDocument", "Upload");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Cannot login.");
                }
            }
            return View(login);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register register)
        {
            if (ModelState.IsValid)
            {
                if ((await _userManager.FindByEmailAsync(register.Email)) == null)
                {
                    var user = new IdentityUser
                    {
                        Email = register.Email,
                        UserName = register.UserName,
                    };
                    var result = await _userManager.CreateAsync(user, register.Password);

                    if (result.Succeeded == false)
                    {
                        foreach(var err in result.Errors)
                        {
                            ModelState.AddModelError(String.Empty, err.Description);
                        }
                        return View(register);
                    }
                    if (register.DepartmentName != null)
                    {
                        var claim = new Claim("Department", register.DepartmentName);
                        await _userManager.AddClaimAsync(user, claim);
                    }
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "User");
                    }
                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError(String.Empty, err.Description);
                    }
                    
                    return View(register);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "This email is already associated with another account. Please try with another email.");
                    return View(register);
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }
    }
}
