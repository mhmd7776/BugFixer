﻿using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Admin.Tag;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BugFixer.Web.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
    #region Ctor

    private readonly IQuestionService _questionService;

    public HomeController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    #endregion

    #region Filter Tags

    public async Task<IActionResult> LoadFilterTagsPartial(FilterTagAdminViewModel filter)
    {
        var result = await _questionService.FilterTagAdmin(filter);
        
        return PartialView("_FilterTagsPartial", result);
    }

    #endregion
    
    #region Create Tags

    public IActionResult LoadCreateTagPartial()
    {
        return PartialView("_CreateTagPartial");
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTag(CreateTagAdminViewModel create)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new {status = "error", message = "مقادیر ورودی معتبر نمی باشد."});
        }

        await _questionService.CreateTagAdmin(create);
        
        return new JsonResult(new {status = "success", message = "عملیات با موفقیت انجام شد."});
    }

    #endregion

    #region Edit Tag

    public async Task<IActionResult> LoadEditTagPartial(long id)
    {
        var result = await _questionService.FillEditTagAdminViewModel(id);

        if (result == null)
        {
            return PartialView("_NotFoundDataPartial");
        }
        
        return PartialView("_EditTagPartial", result);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditTag(EditTagAdminViewModel edit)
    {
        if (!ModelState.IsValid)
        {
            return new JsonResult(new {status = "error", message = "مقادیر ورودی معتبر نمی باشد."});
        }

        var result = await _questionService.EditTagAdmin(edit);

        if (!result)
        {
            return new JsonResult(new {status = "error", message = "مقادیر ورودی معتبر نمی باشد."});
        }
        
        return new JsonResult(new {status = "success", message = "عملیات با موفقیت انجام شد."});
    }

    #endregion

    #region Delete tag

    [HttpPost]
    public async Task<IActionResult> DeleteTag(long id)
    {
        var result = await _questionService.DeleteTagAdmin(id);
        
        if (!result)
        {
            return new JsonResult(new {status = "error", message = "مقادیر ورودی معتبر نمی باشد."});
        }
        
        return new JsonResult(new {status = "success", message = "عملیات با موفقیت انجام شد."});
    }

    #endregion

    #region Online Users

    public IActionResult ShowOnlineUsers()
    {
        return View();
    }

    #endregion

    #region Dashboard

    public async Task<IActionResult> Dashboard()
    {
        ViewData["ChartDataJson"] = JsonConvert.SerializeObject(await _questionService.GetTagViewModelJson());
        
        return View();
    }

    #endregion
}