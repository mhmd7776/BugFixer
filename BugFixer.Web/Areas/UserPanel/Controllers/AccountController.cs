using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.UserPanel.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class AccountController : UserPanelBaseController
    {
        #region Ctor

        private readonly IStateService _stateService;
        private readonly IUserService _userService;

        public AccountController(IStateService stateService, IUserService userService)
        {
            _stateService = stateService;
            _userService = userService;
        }

        #endregion

        #region Edit User Info

        [HttpGet]
        public async Task<IActionResult> EditInfo()
        {
            ViewData["States"] = await _stateService.GetAllStates();

            var result = await _userService.FillEditUserViewModel(HttpContext.User.GetUserId());

            if (result.CountryId.HasValue)
            {
                ViewData["Cities"] = await _stateService.GetAllStates(result.CountryId.Value);
            }

            return View(result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(EditUserViewModel editUserView)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditUserInfo(editUserView, HttpContext.User.GetUserId());

                switch (result)
                {
                    case EditUserInfoResult.Success:
                        TempData[SuccessMessage] = "عملیات با موفقیت انجام شد .";
                        return RedirectToAction("EditInfo", "Account", new {area = "UserPanel"});
                    case EditUserInfoResult.NotValidDate:
                        TempData[ErrorMessage] = "تاریخ وارد شده معتبر نمی باشد .";
                        break;
                }
            }

            ViewData["States"] = await _stateService.GetAllStates();

            if (editUserView.CountryId.HasValue)
            {
                ViewData["Cities"] = await _stateService.GetAllStates(editUserView.CountryId.Value);
            }

            return View(editUserView);
        }

        #endregion

        #region Load Cities

        public async Task<IActionResult> LoadCities(long countryId)
        {
            var result = await _stateService.GetAllStates(countryId);

            return new JsonResult(result);
        }

        #endregion

        #region Change User Password

        [HttpGet]
        public async Task<IActionResult> changeUserPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> changeUserPassword(ChangeUserPasswordViewModel changeUserPassword)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangeUserPassword(HttpContext.User.GetUserId(), changeUserPassword);

                switch (result)
                {
                    case ChangeUserPasswordResult.Success:
                        TempData[SuccessMessage] = "عملیات با موفقیت انجام شد .";
                        await HttpContext.SignOutAsync();
                        return RedirectToAction("Login", "Account", new {area = ""});
                    case ChangeUserPasswordResult.OldPasswordIsNotValid:
                        ModelState.AddModelError("OldPassword", "کلمه عبور وارد شده اشتباه است .");
                        break;
                }
            }

            return View(changeUserPassword);
        }

        #endregion
    }
}
