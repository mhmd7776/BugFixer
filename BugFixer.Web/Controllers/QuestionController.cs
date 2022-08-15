using BugFixer.Application.Extensions;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BugFixer.Web.Controllers
{
    public class QuestionController : BaseController
    {
        #region Ctor

        private IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        #endregion

        #region Create Question

        [Authorize]
        [HttpGet("create-question")]
        public IActionResult CreateQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost("create-question"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(CreateQuestionViewModel createQuestion)
        {
            if (!ModelState.IsValid)
            {
                createQuestion.SelectedTagsJson = JsonConvert.SerializeObject(createQuestion.SelectedTags);
                createQuestion.SelectedTags = null;

                return View(createQuestion);
            }

            var tagResult =
                await _questionService.CheckTagValidation(createQuestion.SelectedTags, HttpContext.User.GetUserId());

            if (tagResult.Status == CreateQuestionResultEnum.NotValidTag)
            {
                createQuestion.SelectedTagsJson = JsonConvert.SerializeObject(createQuestion.SelectedTags);
                createQuestion.SelectedTags = null;

                TempData[WarningMessage] = tagResult.Message;

                return View(createQuestion);
            }

            createQuestion.UserId = HttpContext.User.GetUserId();

            var result = await _questionService.CreateQuestion(createQuestion);

            if (result)
            {
                TempData[SuccessMessage] = "عملیات با موفقیت انجام شد .";

                return Redirect("/");
            }

            createQuestion.SelectedTagsJson = JsonConvert.SerializeObject(createQuestion.SelectedTags);
            createQuestion.SelectedTags = null;

            return View(createQuestion);
        }

        #endregion

        #region Edit Question

        [HttpGet("edit-question/{id}")]
        [Authorize]
        public async Task<IActionResult> EditQuestion(long id)
        {
            var result = await _questionService.FillEditQuestionViewModel(id, User.GetUserId());

            if (result == null) return NotFound();

            return View(result);
        }

        [HttpPost("edit-question/{id}"), ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditQuestion(EditQuestionViewModel edit)
        {
            if (!ModelState.IsValid)
            {
                edit.SelectedTagsJson = JsonConvert.SerializeObject(edit.SelectedTags);
                edit.SelectedTags = null;

                return View(edit);
            }

            var tagResult =
                await _questionService.CheckTagValidation(edit.SelectedTags, HttpContext.User.GetUserId());

            if (tagResult.Status == CreateQuestionResultEnum.NotValidTag)
            {
                edit.SelectedTagsJson = JsonConvert.SerializeObject(edit.SelectedTags);
                edit.SelectedTags = null;

                TempData[WarningMessage] = tagResult.Message;

                return View(edit);
            }

            edit.UserId = HttpContext.User.GetUserId();

            var result = await _questionService.EditQuestion(edit);

            if (result)
            {
                TempData[SuccessMessage] = "عملیات با موفقیت انجام شد .";

                return Redirect("/");
            }

            edit.SelectedTagsJson = JsonConvert.SerializeObject(edit.SelectedTags);
            edit.SelectedTags = null;

            return View(edit);
        }

        #endregion

        #region Get Tags Ajax

        [HttpGet("get-tags")]
        public async Task<IActionResult> GetTagsForSuggest(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(null);
            }

            var tags = await _questionService.GetAllTags();

            var filteredTags = tags.Where(s => s.Title.Contains(name))
                .Select(s => s.Title)
                .ToList();

            return Json(filteredTags);
        }

        #endregion

        #region Get Questions Ajax

        [HttpGet("get-questions")]
        public async Task<IActionResult> GetQuestionsForSuggest(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(null);
            }

            var questions = await _questionService.GetAllQuestions();

            var filteredQuestions = questions.Where(s => s.Title.Contains(name))
                .Select(s => s.Title)
                .ToList();

            return Json(filteredQuestions);
        }

        #endregion

        #region Question List

        [HttpGet("questions")]
        public async Task<IActionResult> QuestionList(FilterQuestionViewModel filter)
        {
            var result = await _questionService.FilterQuestions(filter);

            return View(result);
        }

        #endregion

        #region Filter Question By Tag

        [HttpGet("tags/{tagName}")]
        public async Task<IActionResult> QuestionListBtTag(FilterQuestionViewModel filter, string tagName)
        {
            tagName = tagName.Trim().ToLower().SanitizeText();

            filter.TagTitle = tagName;

            var result = await _questionService.FilterQuestions(filter);

            ViewBag.TagTitle = tagName;

            return View(result);
        }

        #endregion

        #region Filter Tags

        [HttpGet("tags")]
        public async Task<IActionResult> FilterTags(FilterTagViewModel filter)
        {
            filter.TakeEntity = 12;

            var result = await _questionService.FilterTags(filter);

            return View(result);
        }

        #endregion

        #region Question Detail

        [HttpGet("questions/{questionId}")]
        public async Task<IActionResult> QuestionDetail(long questionId)
        {
            var question = await _questionService.GetQuestionById(questionId);

            if (question == null) return NotFound();

            ViewBag.IsBookMark = false;

            if (User.Identity.IsAuthenticated &&
                await _questionService.IsExistsQuestionInUserBookmarks(question.Id, User.GetUserId()))
            {
                ViewBag.IsBookMark = true;
            }

            var userIp = Request.HttpContext.Connection.RemoteIpAddress;
            if (userIp != null)
            {
                await _questionService.AddViewForQuestion(userIp.ToString(), question);
            }

            ViewData["TagsList"] = await _questionService.GetTagListByQuestionId(question.Id);

            return View(question);
        }

        [HttpGet("q/{questionId}")]
        public async Task<IActionResult> QuestionDetailByShortLink(long questionId)
        {
            var question = await _questionService.GetQuestionById(questionId);

            if (question == null) return NotFound();

            return RedirectToAction("QuestionDetail", "Question", new {questionId = questionId});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AnswerQuestion(AnswerQuestionViewModel answerQuestion)
        {
            if (string.IsNullOrEmpty(answerQuestion.Answer))
            {
                return new JsonResult(new { status = "EmptyAnswer" });
            }

            answerQuestion.UserId = User.GetUserId();

            var result = await _questionService.AnswerQuestion(answerQuestion);

            if (result)
            {
                return new JsonResult(new {status = "Success"});
            }

            return new JsonResult(new { status = "Error" });
        }

        #endregion

        #region Edit Answer

        [HttpGet("EditAnswer/{answerId}")]
        [Authorize]
        public async Task<IActionResult> EditAnswer(long answerId)
        {
            var result = await _questionService.FillEditAnswerViewModel(answerId, User.GetUserId());

            if (result == null) return NotFound();

            return View(result);
        }

        [HttpPost("EditAnswer/{answerId}"), ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditAnswer(EditAnswerViewModel editAnswerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editAnswerViewModel);
            }

            editAnswerViewModel.UserId = User.GetUserId();

            var result = await _questionService.EditAnswer(editAnswerViewModel);

            if (result)
            {
                TempData[SuccessMessage] = "عملیات با موفقیت انجام شد.";
                return RedirectToAction("QuestionDetail", "Question", new { questionId = editAnswerViewModel.QuestionId });
            }

            TempData[ErrorMessage] = "خطایی رخ داده است.";

            return View(editAnswerViewModel);
        }

        #endregion

        #region Score Answer

        [Authorize]
        [HttpPost("ScoreUpForAnswer")]
        public async Task<IActionResult> ScoreUpForAnswer(long answerId)
        {
            var result = await _questionService.CreateScoreForAnswer(answerId, AnswerScoreType.Plus, User.GetUserId());

            switch (result)
            {
                case CreateScoreForAnswerResult.Error:
                    return new JsonResult(new {status = "Error"});
                case CreateScoreForAnswerResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForAnswerResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForAnswerResult.Success:
                    return new JsonResult(new { status = "Success" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Authorize]
        [HttpPost("ScoreDownForAnswer")]
        public async Task<IActionResult> ScoreDownForAnswer(long answerId)
        {
            var result = await _questionService.CreateScoreForAnswer(answerId, AnswerScoreType.Minus, User.GetUserId());

            switch (result)
            {
                case CreateScoreForAnswerResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForAnswerResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForAnswerResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForAnswerResult.Success:
                    return new JsonResult(new { status = "Success" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Select True Answer

        [HttpPost("SelectTrueAnswer")]
        public async Task<IActionResult> SelectTrueAnswer(long answerId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new {status = "NotAuthorize"});
            }

            if (!await _questionService.HasUserAccessToSelectTrueAnswer(User.GetUserId(), answerId))
            {
                return new JsonResult(new { status = "NotAccess" });
            }

            await _questionService.SelectTrueAnswer(User.GetUserId(), answerId);

            return new JsonResult(new { status = "Success" });
        }

        #endregion

        #region Score Question

        [HttpPost("ScoreUpForQuestion")]
        [Authorize]
        public async Task<IActionResult> ScoreUpForQuestion(long questionId)
        {
            var result = await _questionService.CreateScoreForQuestion(questionId, QuestionScoreType.Plus, User.GetUserId());

            switch (result)
            {
                case CreateScoreForAnswerResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForAnswerResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForAnswerResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForAnswerResult.Success:
                    return new JsonResult(new { status = "Success" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpPost("ScoreDownForQuestion")]
        [Authorize]
        public async Task<IActionResult> ScoreDownForQuestion(long questionId)
        {
            var result = await _questionService.CreateScoreForQuestion(questionId, QuestionScoreType.Minus, User.GetUserId());

            switch (result)
            {
                case CreateScoreForAnswerResult.Error:
                    return new JsonResult(new { status = "Error" });
                case CreateScoreForAnswerResult.NotEnoughScoreForDown:
                    return new JsonResult(new { status = "NotEnoughScoreForDown" });
                case CreateScoreForAnswerResult.NotEnoughScoreForUp:
                    return new JsonResult(new { status = "NotEnoughScoreForUp" });
                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });
                case CreateScoreForAnswerResult.Success:
                    return new JsonResult(new { status = "Success" });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Add Question To Bookmark

        [HttpPost("AddQuestionToBookmark")]
        public async Task<IActionResult> AddQuestionToBookmark(long questionId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new { status = "NotAuthorize" });
            }

            var result = await _questionService.AddQuestionToBookmark(questionId, User.GetUserId());

            if (!result)
            {
                return new JsonResult(new { status = "Error" });
            }

            return new JsonResult(new { status = "Success" });
        }

        #endregion
    }
}
