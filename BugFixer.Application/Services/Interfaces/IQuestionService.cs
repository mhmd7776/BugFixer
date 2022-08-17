using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Admin.Tag;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IQuestionService
    {
        #region Tags

        Task<List<Tag>> GetAllTags();

        Task<CreateQuestionResult> CheckTagValidation(List<string>? tags, long userId);

        Task<bool> CreateQuestion(CreateQuestionViewModel createQuestion);

        Task<FilterTagViewModel> FilterTags(FilterTagViewModel filter);

        Task<List<string>> GetTagListByQuestionId(long questionId);

        #endregion

        #region Questions

        Task<FilterQuestionViewModel> FilterQuestions(FilterQuestionViewModel filter);

        Task<Question?> GetQuestionById(long id);

        Task<IQueryable<Question>> GetAllQuestions();

        Task<bool> AnswerQuestion(AnswerQuestionViewModel answerQuestion);

        Task AddViewForQuestion(string userIp, Question question);

        Task<bool> AddQuestionToBookmark(long questionId, long userId);

        Task<bool> IsExistsQuestionInUserBookmarks(long questionId, long userId);

        Task<EditQuestionViewModel?> FillEditQuestionViewModel(long questionId, long userId);

        Task<bool> EditQuestion(EditQuestionViewModel edit);

        Task<FilterQuestionBookmarksViewModel> FilterQuestionBookmarks(FilterQuestionBookmarksViewModel filter);

        #endregion

        #region answer

        Task<List<Answer>> GetAllQuestionAnswers(long questionId);

        Task<bool> HasUserAccessToSelectTrueAnswer(long userId, long answerId);

        Task SelectTrueAnswer(long userId, long answerId);

        Task<CreateScoreForAnswerResult> CreateScoreForAnswer(long answerId, AnswerScoreType type, long userId);

        Task<CreateScoreForAnswerResult> CreateScoreForQuestion(long questionId, QuestionScoreType type, long userId);

        Task<EditAnswerViewModel?> FillEditAnswerViewModel(long answerId, long userId);

        Task<bool> EditAnswer(EditAnswerViewModel editAnswerViewModel);

        #endregion

        #region Admin

        Task<List<TagViewModelJson>> GetTagViewModelJson();

        Task<FilterTagAdminViewModel> FilterTagAdmin(FilterTagAdminViewModel filter);

        Task CreateTagAdmin(CreateTagAdminViewModel createTagAdminViewModel);

        Task<EditTagAdminViewModel?> FillEditTagAdminViewModel(long id);

        Task<bool> EditTagAdmin(EditTagAdminViewModel edit);

        #endregion
    }
}
