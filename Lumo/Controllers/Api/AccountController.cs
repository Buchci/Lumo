using Lumo.Models;
using Lumo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Manage()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new ManageProfileViewModel
        {
            Nickname = user.Nickname,
            Email = user.Email!
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manage(ManageProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();


        user.Nickname = model.Nickname;
        if (user.Email != model.Email)
        {
            user.Email = model.Email;
            user.UserName = model.Email; 
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors) ModelState.AddModelError("", error.Description);
            return View(model);
        }


        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            if (string.IsNullOrEmpty(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is required to set a new one.");
                return View(model);
            }

            var changePassResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePassResult.Succeeded)
            {
                foreach (var error in changePassResult.Errors) ModelState.AddModelError("", error.Description);
                return View(model);
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = "Profile updated successfully!";
        return RedirectToAction(nameof(Manage));
    }
}