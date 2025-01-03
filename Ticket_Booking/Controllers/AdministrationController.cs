using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket_Booking.ViewModel.AdministrationViewModel;

namespace Ticket_Booking.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {

        private RoleManager<IdentityRole> _roleManager;
        private UserManager<IdentityUser> _userManager;
        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region ALL ROLES
        [HttpGet]
        public IActionResult ListRoles()
        {
            try
            {
                var roles = _roleManager.Roles;
                return View(roles);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }
        #endregion


        #region CREATE ROLE
        [HttpGet]
        public IActionResult CreateRole()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = model.RoleName
                    };

                    IdentityResult result = await _roleManager.CreateAsync(identityRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Administration");
                    }

                    foreach (var i in result.Errors)
                    {
                        ModelState.AddModelError("", i.Description);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion


        #region EDIT ROLE

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This role does not exist." });
                }
                else
                {
                    role.Name = model.RoleName;

                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var i in result.Errors)
                    {
                        ModelState.AddModelError("", i.Description);
                    }

                    return View(model);

                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This role does not exist." });
                }
                else
                {
                    EditRoleViewModel model = new EditRoleViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name

                    };

                    foreach (var user in _userManager.Users)
                    {
                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            model.Users.Add(user.UserName);
                        }
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }


        #endregion


        #region  EDIT ROLES USER

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            try
            {
                ViewBag.roleId = roleId;

                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This userrole does not exist." });
                }

                var model = new List<UserRoleViewModel>();

                foreach (var user in _userManager.Users)
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };

                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRoleViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRoleViewModel.IsSelected = false;
                    }
                    model.Add(userRoleViewModel);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        [HttpPost]

        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return RedirectToAction("Error", "Bus");
                }

                for (int i = 0; i < model.Count; i++)
                {
                    var user = await _userManager.FindByIdAsync(model[i].UserId);

                    IdentityResult result = null;

                    if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else if (!(model[i].IsSelected) && (await _userManager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else
                    {
                        continue;
                    }

                    if (result.Succeeded)
                    {
                        if (i < model.Count - 1)
                        {
                            continue;
                        }
                        else
                        {
                            return RedirectToAction("EditRole", new { id = roleId });
                        }
                    }

                }

                return RedirectToAction("EditRole", new { id = roleId });

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }


        #endregion


        #region DELETE ROLE

        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This role does not exist." });
                }
                else
                {
                    try
                    {
                        var result = await _roleManager.DeleteAsync(role);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("ListRoles");
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(ListRoles);
                    }
                    catch (DbUpdateException ex)
                    {
                        ViewBag.ErrorTitle = $"{role.Name} role is in use";
                        ViewBag.ErrorMessage = $"{role.Name} role cannot be detect as there are users in this role" +
                            "If yopu want to delete the role ,please remove the user from the role and then try to delete";

                        return View("Error");
                    }

                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        #endregion

    }
}
