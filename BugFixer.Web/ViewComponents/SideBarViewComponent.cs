using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.ViewComponents
{
    public class ScoreDesQuestionsViewComponent : ViewComponent
    {
        #region Ctor

        private IQuestionService _questionService;

        public ScoreDesQuestionsViewComponent(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var options = new FilterQuestionViewModel
            {
                TakeEntity = 5,
                Sort = FilterQuestionSortEnum.ScoreHighToLow
            };

            var result = await _questionService.FilterQuestions(options);

            return View("ScoreDesQuestions", result);
        }
    }

    public class CreateDateDesQuestionsViewComponent : ViewComponent
    {
        #region Ctor

        private IQuestionService _questionService;

        public CreateDateDesQuestionsViewComponent(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var options = new FilterQuestionViewModel
            {
                TakeEntity = 5,
                Sort = FilterQuestionSortEnum.NewToOld
            };

            var result = await _questionService.FilterQuestions(options);

            return View("CreateDateDesQuestions", result);
        }
    }

    public class UseCountDesTagsViewComponent : ViewComponent
    {
        #region Ctor

        private IQuestionService _questionService;

        public UseCountDesTagsViewComponent(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var options = new FilterTagViewModel
            {
                TakeEntity = 10,
                Sort = FilterTagEnum.UseCountHighToLow
            };

            var result = await _questionService.FilterTags(options);

            return View("UseCountDesTags", result);
        }
    }
}
