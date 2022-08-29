using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.Admin.Controllers;

public class QuestionController : AdminBaseController
{
    #region Ctor

    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    #endregion

    public async Task<IActionResult> QuestionsList(FilterQuestionViewModel filter)
    {
        var result = await _questionService.FilterQuestions(filter);

        return View(result);
    }
}