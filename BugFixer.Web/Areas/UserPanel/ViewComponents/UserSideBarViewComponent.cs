using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.ViewComponents
{
    public class UserSideBarViewComponent : ViewComponent
    {
        #region Ctor

        private IUserService _userService;

        public UserSideBarViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userService.GetUserById(HttpContext.User.GetUserId());

            return View("UserSideBar", user);
        }
    }
}
