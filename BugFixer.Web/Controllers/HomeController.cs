using BugFixer.Application.Extensions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Ctor

        private readonly IQuestionService _questionService;

        public HomeController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var options = new FilterQuestionViewModel
            {
                TakeEntity = 10,
                Sort = FilterQuestionSortEnum.NewToOld
            };

            ViewData["Questions"] = await _questionService.FilterQuestions(options);

            return View();
        }

        #region Editor Upload

        public async Task<IActionResult> UploadEditorImage(IFormFile upload)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName);

            upload.UploadFile(fileName, PathTools.EditorImageServerPath);

            return Json(new { url = $"{PathTools.EditorImagePath}{fileName}" });
        }

        #endregion

        #region 404

        [HttpGet("/404")]
        public IActionResult NotFoundPage()
        {
            return View();
        }

        #endregion
    }
}