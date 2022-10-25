﻿using BugFixer.Web.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[PermissionChecker(1)]
public class AdminBaseController : Controller
{
    public static string SuccessMessage = "SuccessMessage";
    public static string WarningMessage = "WarningMessage";
    public static string InfoMessage = "InfoMessage";
    public static string ErrorMessage = "ErrorMessage";
}