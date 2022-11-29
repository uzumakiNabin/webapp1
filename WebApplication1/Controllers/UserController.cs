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
                    return RedirectToAction("Index", "Home");
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

                    //if (!await _roleManager.RoleExistsAsync("Admin"))
                    //{
                    //    var roleResult = await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                    //}

                    //user = await _userManager.FindByEmailAsync(register.Email);
                    //await _userManager.AddToRoleAsync(user, "Admin");
                    var claim = new Claim("Department", register.Department);
                    await _userManager.AddClaimAsync(user, claim);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                    /*
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    if (result.Succeeded)
                    {
                        //var confirmationLink = Url.ActionLink("Confirm Email", "Identity", new { userId = user.Id, @token = token });
                        //await _emailSender.SendEmailAsync("nabin.kaucha@gmail.com", user.Email, "Confirm your email address", confirmationLink);
                        return RedirectToAction("MFASetup");
                    }
                    */
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
            return RedirectToAction("Login");
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(Role role)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(role.RoleName))
                {
                    ModelState.AddModelError("RoleName", "Already Exists");
                    return View(role);
                }
                var result = await _roleManager.CreateAsync(new IdentityRole() { Name = role.RoleName });
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                ModelState.AddModelError("Register", String.Join(" ", result.Errors.Select(x => x.Description)));
                return View(role);
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDepartment(Department dept)
        {
            if (ModelState.IsValid)
            {
                var test = await _userContext.Department.FirstOrDefaultAsync(u => u.DepartmentName == dept.DepartmentName);
                if (test != null)
                {
                    ModelState.AddModelError("DepartmentName", "Already Exists");
                    return View(dept);
                }
                var result = await _userContext.Department.AddAsync(dept);
                _userContext.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddRoleToUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoleToUser(string email)
        {
            return View();
        }
    }
}
