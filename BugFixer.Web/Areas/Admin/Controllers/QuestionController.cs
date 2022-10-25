using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Web.ActionFilters;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.Admin.Controllers;

[PermissionChecker(3)]
public class QuestionController : AdminBaseController
{
    #region Ctor

    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    #endregion

    #region Questions List

    public async Task<IActionResult> QuestionsList(FilterQuestionViewModel filter)
    {
        var result = await _questionService.FilterQuestions(filter);

        return View(result);
    }

    #endregion

    #region Delete Question

    [HttpPost]
    public async Task<IActionResult> DeleteQuestion(long id)
    {
        var result = await _questionService.DeleteQuestion(id);
        
        if (!result)
        {
            return new JsonResult(new {status = "error", message = "مقادیر ورودی معتبر نمی باشد."});
        }
        
        return new JsonResult(new {status = "success", message = "عملیات با موفقیت انجام شد."});
    }

    #endregion
    
    #region Is Checked Question

    [HttpPost]
    public async Task<IActionResult> ChangeIsCheckedQuestion(long id)
    {
        var result = await _questionService.ChangeQuestionIsCheck(id);
        
        if (!result)
        {
            return new JsonResult(new {status = "error", message = "مقادیر ورودی معتبر نمی باشد."});
        }
        
        return new JsonResult(new {status = "success", message = "عملیات با موفقیت انجام شد."});
    }

    #endregion
}