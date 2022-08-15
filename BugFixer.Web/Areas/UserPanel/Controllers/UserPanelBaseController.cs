using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class UserPanelBaseController : Controller
    {
        public static string SuccessMessage = "SuccessMessage";
        public static string WarningMessage = "WarningMessage";
        public static string InfoMessage = "InfoMessage";
        public static string ErrorMessage = "ErrorMessage";
    }
}
