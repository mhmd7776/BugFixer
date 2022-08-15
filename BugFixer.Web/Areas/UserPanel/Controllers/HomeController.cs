using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class HomeController : UserPanelBaseController
    {
        #region Ctor

        private IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        #region Change User Avatar

        public async Task<IActionResult> ChangeUserAvatar(IFormFile userAvatar)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(userAvatar.FileName);

            var validFormats = new List<string>()
            {
                ".png",
                ".jpg",
                ".jpeg"
            };

            var result = userAvatar.UploadFile(fileName, PathTools.UserAvatarServerPath, validFormats);

            if (!result)
            {
                return new JsonResult(new { status = "Error" });
            }

            await _userService.ChangeUserAvatar(HttpContext.User.GetUserId(), fileName);

            TempData[SuccessMessage] = "عملیات با موفقیت انجام شد .";

            return new JsonResult(new {status = "Success"});
        }

        #endregion
    }
}
