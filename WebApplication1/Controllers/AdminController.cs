using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.Win32;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DbaseContext _userContext;
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DbaseContext dbaseContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userContext = dbaseContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(CreateUser user)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByNameAsync(user.UserName) == null)
                {
                    if (await _userManager.FindByEmailAsync(user.Email) == null)
                    {
                        IdentityUser identityUser = new IdentityUser()
                        {
                            Email = user.Email,
                            UserName = user.UserName,
                        };
                        var result = await _userManager.CreateAsync(identityUser, user.Password);
                        if (!result.Succeeded)
                        {
                            foreach (var err in result.Errors)
                            {
                                ModelState.AddModelError(String.Empty, err.Description);
                            }
                            return View(user);
                        }
                        else
                        {
                            IdentityUser newUser = await _userManager.FindByEmailAsync(user.Email);
                            if (user.RoleName != null)
                            {
                                if (await _roleManager.RoleExistsAsync(user.RoleName))
                                {
                                    var roleResult = await _userManager.AddToRoleAsync(newUser, user.RoleName);
                                    if (!roleResult.Succeeded)
                                    {
                                        ModelState.AddModelError(String.Empty, "Failed to create User. Try again.");
                                        return View(user);
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(String.Empty, "Role does not exist. Please create first and try again.");
                                    return View(user);
                                }
                            }

                            if (user.DepartmentName != null)
                            {
                                var claim = new Claim("Department", user.DepartmentName);
                                var claimResult = await _userManager.AddClaimAsync(newUser, claim);
                                if (!claimResult.Succeeded)
                                {
                                    ModelState.AddModelError(String.Empty, "Failed to create User. Try again.");
                                    return View(user);
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Email already used. Try another.");
                        return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Username already used. Try another.");
                    return View(user);
                }
            }
            return View();
        }
    }
}
