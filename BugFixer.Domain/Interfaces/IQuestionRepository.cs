using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;

namespace BugFixer.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        #region Tags

        Task<List<Tag>> GetAllTags();
        
        Task<IQueryable<Tag>> GetAllTagsAsQueryable();

        Task<bool> IsExistsTagByName(string name);

        Task<Tag?> GetTagByName(string name);
        
        Task<Tag?> GetTagById(long id);

        Task<bool> CheckUserRequestedForTag(long userId, string tag);

        Task AddRequestTag(RequestTag tag);

        Task AddTag(Tag tag);

        Task UpdateTag(Tag tag);

        Task SaveChanges();

        Task<int> RequestCountForTag(string tag);

        Task<List<string>> GetTagListByQuestionId(long questionId);

        #endregion

        #region Question

        Task AddQuestion(Question question);

        Task AddBookmark(UserQuestionBookmark bookmark);

        void RemoveBookmark(UserQuestionBookmark bookmark);

        Task<bool> IsExistsQuestionInUserBookmarks(long questionId, long userId);

        Task<UserQuestionBookmark?> GetBookmarkByQuestionAndUserId(long questionId, long userId);

        Task UpdateQuestion(Question question);

        Task<IQueryable<Question>> GetAllQuestions();
        
        IQueryable<UserQuestionBookmark> GetAllBookmarks();

        Task<Question?> GetQuestionById(long id);

        #endregion

        #region View

        Task<bool> IsExistsViewForQuestion(string userIp, long questionId);

        Task AddQuestionView(QuestionView view);

        #endregion

        #region Selected Tag

        Task AddSelectedQuestionTag(SelectQuestionTag selectQuestionTag);

        Task DeleteSelectedQuestionTag(SelectQuestionTag selectQuestionTag);

        #endregion

        #region Answer

        Task AddAnswer(Answer answer);

        Task AddAnswerUserScore(AnswerUserScore score);

        Task AddQuestionUserScore(QuestionUserScore score);

        Task UpdateAnswer(Answer answer);

        Task<List<Answer>> GetAllQuestionAnswers(long questionId);

        Task<Answer?> GetAnswerById(long id);

        Task<bool> IsExistsUserScoreForAnswer(long answerId, long userId);

        Task<bool> IsExistsUserScoreForQuestion(long questionId, long userId);

        #endregion
    }
}
