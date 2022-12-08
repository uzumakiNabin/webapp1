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

        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> CreateUser(Register user)
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                string RoleName = role.RoleName.Trim();
                if(RoleName.Length > 0)
                {
                    if(!await _roleManager.RoleExistsAsync(RoleName))
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole { Name = RoleName });
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError(String.Empty, "Could not create new role. Try again.");
                            return View(role);
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Role already exists. Try another.");
                        return View(role);
                    }
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDepartment(Department dept)
        {
            if (ModelState.IsValid)
            {
                string DeptName = dept.DepartmentName.Trim();
                if (DeptName.Length > 0)
                {
                    Department ExistingDept = _userContext.Departments.Where(d => d.DepartmentName == DeptName).FirstOrDefault();
                    if(ExistingDept != null)
                    {
                        ModelState.AddModelError(String.Empty, "Department already exists. Try another.");
                        return View(dept);
                    }
                    else
                    {
                        await _userContext.Departments.AddAsync(dept);
                        _userContext.SaveChanges();
                        return View();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(AssignRole roleToUser)
        {
            if (ModelState.IsValid)
            {
                string userName = roleToUser.UserName.Trim();
                string roleName = roleToUser.RoleName.Trim();
                if(userName.Length > 0 && roleName.Length > 0)
                {
                    IdentityUser user = await _userManager.FindByNameAsync(userName);
                    if(user != null && await _roleManager.RoleExistsAsync(roleName))
                    {
                        var result = await _userManager.AddToRoleAsync(user, roleName);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError(String.Empty, "Cannot assign now. Try again.");
                            return View(roleToUser);
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Either user or role does not exist. Check and try again.");
                        return View(roleToUser);
                    }
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignDepartment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDepartment(AssignDepartment deptToUser)
        {
            if (ModelState.IsValid)
            {
                string userName = deptToUser.UserName.Trim();
                string deptName = deptToUser.DepartmentName.Trim();
                if (userName.Length > 0 && deptName.Length > 0)
                {
                    IdentityUser user = await _userManager.FindByNameAsync(userName);
                    if (user != null && _userContext.Departments.Where(d => d.DepartmentName == deptName).FirstOrDefault() != null)
                    {
                        Claim claim = new Claim("Department", deptName);
                        var result = await _userManager.AddClaimAsync(user, claim);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError(String.Empty, "Cannot assign now. Try again.");
                            return View(deptToUser);
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Either user or department does not exist. Check and try again.");
                        return View(deptToUser);
                    }
                }
            }
            return View();
        }
    }
}
