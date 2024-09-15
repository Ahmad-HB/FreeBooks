using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace FreeBooks.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountsController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountsController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public IActionResult Roles()
    {
        return View(new RolesViewModel
        {
            NewRole = new NewRole(),
            Roles = _roleManager.Roles.OrderBy(x => x.Name).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Roles(RolesViewModel model)
    {
        if (true)
        {
            if (model.NewRole.RoleId == Guid.Empty) // Create new role
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.NewRole.RoleName));
                if (result.Succeeded)
                {
                    // TempData["SuccessMessage"] = "Role created successfully.";
                    HttpContext.Session.SetString("msgType", "success");
                    HttpContext.Session.SetString("title", Resource.Resources.lbSave);
                    HttpContext.Session.SetString("msg", Resource.Resources.lbSaveMsgRole);
                    return RedirectToAction("Roles");
                }
                else
                {
                    // TempData["ErrorMessage"] = "Error creating role.";
                    HttpContext.Session.SetString("msgType", "error");
                    HttpContext.Session.SetString("title", Resource.Resources.lbNotSaved);
                    HttpContext.Session.SetString("msg", Resource.Resources.lbNotSavedMsgRole);
                    return RedirectToAction("Roles");
                }
            }
            else // Update existing role
            {
                var role = await _roleManager.FindByIdAsync(model.NewRole.RoleId.ToString());
                if (role != null)
                {
                    role.Name = model.NewRole.RoleName;
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        // TempData["SuccessMessage"] = "Role updated successfully.";
                        HttpContext.Session.SetString("msgType", "success");
                        HttpContext.Session.SetString("title", Resource.Resources.lbUpdate);
                        HttpContext.Session.SetString("msg", Resource.Resources.lbUpdateMsgRole);
                    }
                    else
                    {
                        // TempData["ErrorMessage"] = "Error updating role.";
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", Resource.Resources.lbNotUpdate);
                        HttpContext.Session.SetString("msg", Resource.Resources.lbNotUpdateMsgRole);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Role not found.";
                }
            }

            return RedirectToAction(nameof(Roles));
        }

        // If we got this far, something failed, redisplay form with the current roles
        model.Roles = _roleManager.Roles.OrderBy(x => x.Name).ToList();
        return View(model);
    }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Roles(RolesViewModel model)
    // {
    //     if (true)
    //     {
    //        
    //         if (model.NewRole.RoleId == Guid.Empty)
    //         {
    //             var Result = await _roleManager.CreateAsync(new IdentityRole(model.NewRole.RoleName));
    //             if (Result.Succeeded)
    //             {
    //                 HttpContext.Session.SetString("msgType", "success");
    //                 HttpContext.Session.SetString("title", Resource.Resources.lbSave);
    //                 HttpContext.Session.SetString("msg", "user saved successfully");
    //                 return RedirectToAction("Roles");
    //             }
    //             else
    //             {
    //                 HttpContext.Session.SetString("msgType", "error");
    //                 HttpContext.Session.SetString("title", "save not done");
    //                 HttpContext.Session.SetString("msg", "user did not save successfully");
    //                 HttpContext.Session.SetString("msg", "user saved successfully");
    //                 return RedirectToAction("Roles");
    //             }
    //         }
    //         else
    //         {
    //             var RoleUpdate = await _roleManager.FindByIdAsync(model.NewRole.RoleId.ToString());
    //             RoleUpdate.Id = model.NewRole.RoleId.ToString();
    //             RoleUpdate.Id = model.NewRole.RoleName;
    //             var result = await _roleManager.UpdateAsync(RoleUpdate);
    //             if (result.Succeeded)
    //             {
    //                 HttpContext.Session.SetString("msgType", "success");
    //                 HttpContext.Session.SetString("title", Resource.Resources.lbUpdate);
    //                 HttpContext.Session.SetString("msg", "user Eidt successfully");
    //             }
    //             else
    //             {
    //                 HttpContext.Session.SetString("msgType", "error");
    //                 HttpContext.Session.SetString("title", "Eidt not done");
    //                 HttpContext.Session.SetString("msg", "user did not Eidt successfully");
    //             }
    //             
    //         }
    //     }
    //     return View();
    // }
    
    public async Task<IActionResult> DeleteRole(string Id)
    {
        var role = _roleManager.Roles.FirstOrDefault(x => x.Id == Id);
        if ((await _roleManager.DeleteAsync(role)).Succeeded)
        {
            return RedirectToAction(nameof(Roles));
        }

        return RedirectToAction("Roles");
    }


    public IActionResult Register()
    {
        return View(new RegisterViewModel
        {
            NewRegister = new NewRegister(),
            Roles = _roleManager.Roles.OrderBy(x => x.Name).ToList(),
            Users = _userManager.Users.OrderBy(x => x.Name).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                Id = model.NewRegister.Id.ToString(),
                Name = model.NewRegister.Name,
                UserName = model.NewRegister.Email,
                Email = model.NewRegister.Email,
                ActiveUser = model.NewRegister.ActiveUser,
                ImageUser = model.NewRegister.ImageUser
            };
            if (Guid.Parse(user.Id) == Guid.Empty)
            {
                user.Id = Guid.NewGuid().ToString();
                var result = await _userManager.CreateAsync(user, model.NewRegister.Password);
                if (result.Succeeded)
                {
                    //.GetAwaiter().GetResult()
                    var Role = await _userManager.AddToRoleAsync(user, model.NewRegister.RoleName);
                    if (Role.Succeeded)
                    {
                        HttpContext.Session.SetString("msgType", "success");
                        HttpContext.Session.SetString("title", Resource.Resources.lbSave);
                        HttpContext.Session.SetString("msg", Resource.Resources.lbNotSavedMsgUserRole);
                        return RedirectToAction("Register", "Accounts");
                    }
                    else
                    {
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", Resource.Resources.lbNotSaved);
                        HttpContext.Session.SetString("msg", Resource.Resources.lbNotSavedMsgUserRole);
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    HttpContext.Session.SetString("msgType", "error");
                    HttpContext.Session.SetString("title", Resource.Resources.lbNotSaved);
                    HttpContext.Session.SetString("msg", Resource.Resources.lbNotSavedMsgUser);
                    return RedirectToAction("Register", "Accounts");
                }
            }
            else
            {
                var userupdate = await _userManager.FindByIdAsync(model.NewRegister.Id.ToString());
                userupdate.Id = model.NewRegister.Id.ToString();
                userupdate.Name = model.NewRegister.Name;
                userupdate.UserName = model.NewRegister.Name;
                userupdate.Email = model.NewRegister.Email;
                userupdate.ActiveUser = model.NewRegister.ActiveUser;
                userupdate.ImageUser = model.NewRegister.ImageUser;

                var result = await _userManager.UpdateAsync(userupdate);
                if (result.Succeeded)
                {
                    var oldRoles = await _userManager.GetRolesAsync(userupdate);
                    await _userManager.RemoveFromRolesAsync(userupdate, oldRoles);
                    var addroles = await _userManager.AddToRoleAsync(userupdate, model.NewRegister.RoleName);

                    if (addroles.Succeeded)
                    {
                        HttpContext.Session.SetString("msgType", "success");
                        HttpContext.Session.SetString("title", Resource.Resources.lbUpdate);
                        HttpContext.Session.SetString("msg", Resource.Resources.lbNotUpdateMsgUserRole);
                        return RedirectToAction("Register", "Accounts");
                    }
                    else
                    {
                        HttpContext.Session.SetString("msgType", "error");
                        HttpContext.Session.SetString("title", Resource.Resources.lbNotUpdate);
                        HttpContext.Session.SetString("msg", Resource.Resources.lbNotUpdateMsgUserRole);
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    HttpContext.Session.SetString("msgType", "error");
                    HttpContext.Session.SetString("title", Resource.Resources.lbNotUpdate);
                    HttpContext.Session.SetString("msg", Resource.Resources.lbNotUpdateMsgUser);
                    return RedirectToAction("Register", "Accounts");
                }
            }

            return RedirectToAction("Register", "Accounts");
        }

        return RedirectToAction("Register", "Accounts");
    }

    public async Task<IActionResult> DeleteUser(string Id)
    {
        var user = _userManager.Users.FirstOrDefault(x => x.Id == Id);
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Register", "Accounts");
        }

        return RedirectToAction("Register", "Accounts");
    }


    public IActionResult Login()
    {
        return View();
    }
}