using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        #region Ctor

        private BugFixerDbContext _context;

        public QuestionRepository(BugFixerDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Tags

        public async Task<List<Tag>> GetAllTags()
        {
            return await _context.Tags.Where(s => !s.IsDelete).ToListAsync();
        }

        public async Task<IQueryable<Tag>> GetAllTagsAsQueryable()
        {
            return _context.Tags.Where(s => !s.IsDelete).AsQueryable();
        }

        public async Task UpdateTag(Tag tag)
        {
            _context.Update(tag);
        }

        public async Task<bool> IsExistsTagByName(string name)
        {
            return await _context.Tags.AnyAsync(s => s.Title.Equals(name) && !s.IsDelete);
        }

        public async Task<bool> CheckUserRequestedForTag(long userId, string tag)
        {
            return await _context.RequestTags.AnyAsync(s => s.UserId == userId && s.Title.Equals(tag) && !s.IsDelete);
        }

        public async Task AddRequestTag(RequestTag tag)
        {
            await _context.RequestTags.AddAsync(tag);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> RequestCountForTag(string tag)
        {
            return await _context.RequestTags.CountAsync(s => !s.IsDelete && s.Title.Equals(tag));
        }

        public async Task AddTag(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public async Task<Tag?> GetTagByName(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(s => !s.IsDelete && s.Title.Equals(name));
        }

        #endregion

        #region Question

        public async Task AddQuestion(Question question)
        {
            await _context.AddAsync(question);
        }

        public async Task AddBookmark(UserQuestionBookmark bookmark)
        {
            await _context.AddAsync(bookmark);
        }

        public void RemoveBookmark(UserQuestionBookmark bookmark)
        {
            _context.Remove(bookmark);
        }

        public async Task<bool> IsExistsQuestionInUserBookmarks(long questionId, long userId)
        {
            return await _context.Bookmarks.AnyAsync(s => s.QuestionId == questionId && s.UserId == userId);
        }

        public async Task<UserQuestionBookmark?> GetBookmarkByQuestionAndUserId(long questionId, long userId)
        {
            return await _context.Bookmarks.FirstOrDefaultAsync(s => s.QuestionId == questionId && s.UserId == userId);
        }

        public async Task UpdateQuestion(Question question)
        {
            _context.Questions.Update(question);
        }

        public async Task<IQueryable<Question>> GetAllQuestions()
        {
            return _context.Questions.Where(s => !s.IsDelete).AsQueryable();
        }

        public IQueryable<UserQuestionBookmark> GetAllBookmarks()
        {
            return _context.Bookmarks.Include(s => s.Question).AsQueryable();
        }

        public async Task<Question?> GetQuestionById(long id)
        {
            return await _context.Questions
                .Include(s => s.Answers)
                .Include(s => s.User)
                .Include(s => s.SelectQuestionTags)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);
        }

        #endregion

        #region View

        public async Task<bool> IsExistsViewForQuestion(string userIp, long questionId)
        {
            return await _context.QuestionViews.AnyAsync(s => s.UserIP.Equals(userIp) && s.QuestionId == questionId);
        }

        public async Task AddQuestionView(QuestionView view)
        {
            await _context.QuestionViews.AddAsync(view);
        }

        #endregion

        #region Selected Tag

        public async Task AddSelectedQuestionTag(SelectQuestionTag selectQuestionTag)
        {
            await _context.AddAsync(selectQuestionTag);
        }

        public async Task<List<string>> GetTagListByQuestionId(long questionId)
        {
            return await _context.SelectQuestionTags
                .Include(s => s.Tag)
                .Where(s => s.QuestionId == questionId)
                .Select(s => s.Tag.Title).ToListAsync();
        }

        public async Task DeleteSelectedQuestionTag(SelectQuestionTag selectQuestionTag)
        {
            _context.Remove(selectQuestionTag);
        }

        #endregion

        #region Answer

        public async Task AddAnswer(Answer answer)
        {
            await _context.Answers.AddAsync(answer);
        }

        public async Task AddAnswerUserScore(AnswerUserScore score)
        {
            await _context.AnswerUserScores.AddAsync(score);
        }

        public async Task AddQuestionUserScore(QuestionUserScore score)
        {
            await _context.QuestionUserScores.AddAsync(score);
        }

        public async Task UpdateAnswer(Answer answer)
        {
            _context.Answers.Update(answer);
        }

        public async Task<List<Answer>> GetAllQuestionAnswers(long questionId)
        {
            return await _context.Answers
                .Include(s => s.User)
                .Where(s => s.QuestionId == questionId && !s.IsDelete)
                .OrderByDescending(s => s.CreateDate).ToListAsync();
        }

        public async Task<Answer?> GetAnswerById(long id)
        {
            return await _context.Answers.Include(s => s.Question).FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);
        }

        public async Task<bool> IsExistsUserScoreForAnswer(long answerId, long userId)
        {
            return await _context.AnswerUserScores.AnyAsync(s => s.AnswerId == answerId && s.UserId == userId);
        }

        public async Task<bool> IsExistsUserScoreForQuestion(long questionId, long userId)
        {
            return await _context.QuestionUserScores.AnyAsync(s => s.QuestionId == questionId && s.UserId == userId);
        }

        #endregion
    }
}
