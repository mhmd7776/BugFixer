using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.UserPanel.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers;

public class QuestionController : UserPanelBaseController
{
    #region Ctor

    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    #endregion

    #region Bookmarks

    [HttpGet]
    public async Task<IActionResult> QuestionBookmarks(FilterQuestionBookmarksViewModel filter)
    {
        filter.UserId = User.GetUserId();

        var result = await _questionService.FilterQuestionBookmarks(filter);
        
        return View(result);
    }

    #endregion
}