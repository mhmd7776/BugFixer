using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Admin.User;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.Admin.Controllers;

public class UserController : AdminBaseController
{
    #region Ctor

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #endregion

    #region Users List

    public async Task<IActionResult> FilterUsers(FilterUserAdminViewModel filter)
    {
        var result = await _userService.FilterUserAdmin(filter);

        return View(result);
    }

    #endregion
}