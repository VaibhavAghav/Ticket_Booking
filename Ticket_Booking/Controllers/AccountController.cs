using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticket_Booking.ViewModel.AccountViewModel;

namespace Ticket_Booking.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager; 
        }


        #region LOGOUT
        public async Task<IActionResult> Logout()
        {
            try
            {

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            await signInManager.SignOutAsync();
            return RedirectToAction("GetAllBus", "Bus");
        }

        #endregion


        #region REGISTER
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
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
        [AllowAnonymous]
        public async  Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("BookTicket", "Book");
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


        #region LOGIN

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl); // Use Redirect instead of RedirectToAction for URLs
                        }

                        return RedirectToAction("BookTicket", "Book");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }

                ViewData["ReturnUrl"] = returnUrl;
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }



        #endregion

    }
}
