using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Application.Extensions;
using BugFixer.Application.Security;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Admin.Tag;
using BugFixer.Domain.ViewModels.Common;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BugFixer.Application.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        #region Ctor

        private readonly IQuestionRepository _questionRepository;
        private readonly ScoreManagementViewModel _scoreManagement;
        private readonly IUserService _userService;

        public QuestionService(IQuestionRepository questionRepository, IOptions<ScoreManagementViewModel> scoreManagement, IUserService userService)
        {
            _questionRepository = questionRepository;
            _scoreManagement = scoreManagement.Value;
            _userService = userService;
        }

        #endregion

        #region Tags

        public async Task<List<string>> GetTagListByQuestionId(long questionId)
        {
            return await _questionRepository.GetTagListByQuestionId(questionId);
        }

        public async Task<List<Tag>> GetAllTags()
        {
            return await _questionRepository.GetAllTags();
        }

        public async Task<IQueryable<Question>> GetAllQuestions()
        {
            return await _questionRepository.GetAllQuestions();
        }

        public async Task<CreateQuestionResult> CheckTagValidation(List<string>? tags, long userId)
        {
            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    var isExistsTag = await _questionRepository.IsExistsTagByName(tag.SanitizeText().Trim().ToLower());

                    if (isExistsTag) continue;

                    var isUserRequestedForTag =
                        await _questionRepository.CheckUserRequestedForTag(userId, tag.SanitizeText().Trim().ToLower());

                    if (isUserRequestedForTag)
                    {
                        return new CreateQuestionResult
                        {
                            Status = CreateQuestionResultEnum.NotValidTag,
                            Message = $"تگ {tag} برای اعتبارسنجی نیاز به {_scoreManagement.MinRequestCountForVerifyTag} درخواست دارد ."
                        };
                    }

                    var tagRequest = new RequestTag
                    {
                        Title = tag.SanitizeText().Trim().ToLower(),
                        UserId = userId
                    };

                    await _questionRepository.AddRequestTag(tagRequest);
                    await _questionRepository.SaveChanges();

                    var requestedCount =
                        await _questionRepository.RequestCountForTag(tag.SanitizeText().Trim().ToLower());

                    if (requestedCount < _scoreManagement.MinRequestCountForVerifyTag)
                    {
                        return new CreateQuestionResult
                        {
                            Status = CreateQuestionResultEnum.NotValidTag,
                            Message = $"تگ {tag} برای اعتبارسنجی نیاز به {_scoreManagement.MinRequestCountForVerifyTag} درخواست دارد ."
                        };
                    }

                    var newTag = new Tag
                    {
                        Title = tag.SanitizeText().Trim().ToLower()
                    };

                    await _questionRepository.AddTag(newTag);
                    await _questionRepository.SaveChanges();
                }

                return new CreateQuestionResult
                {
                    Status = CreateQuestionResultEnum.Success,
                    Message = "تگ های ورودی معتبر می باشد ."
                };
            }

            return new CreateQuestionResult
            {
                Status = CreateQuestionResultEnum.NotValidTag,
                Message = "تگ های ورودی نمی تواند خالی باشد ."
            };
        }

        public async Task<FilterTagViewModel> FilterTags(FilterTagViewModel filter)
        {
            var query = await _questionRepository.GetAllTagsAsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => s.Title.Contains(filter.Title));
            }

            switch (filter.Sort)
            {
                case FilterTagEnum.NewToOld:
                    query = query.OrderByDescending(s => s.CreateDate);
                    break;
                case FilterTagEnum.OldToNew:
                    query = query.OrderBy(s => s.CreateDate);
                    break;
                case FilterTagEnum.UseCountHighToLow:
                    query = query.OrderByDescending(s => s.UseCount);
                    break;
                case FilterTagEnum.UseCountLowToHigh:
                    query = query.OrderBy(s => s.UseCount);
                    break;
            }

            await filter.SetPaging(query);

            return filter;
        }

        #endregion

        #region Question

        public async Task<bool> CreateQuestion(CreateQuestionViewModel createQuestion)
        {
            var question = new Question()
            {
                Content = createQuestion.Description.SanitizeText(),
                Title = createQuestion.Title.SanitizeText(),
                UserId = createQuestion.UserId
            };

            await _questionRepository.AddQuestion(question);
            await _questionRepository.SaveChanges();

            if (createQuestion.SelectedTags != null && createQuestion.SelectedTags.Any())
            {
                foreach (var questionSelectedTag in createQuestion.SelectedTags)
                {
                    var tag = await _questionRepository.GetTagByName(questionSelectedTag.SanitizeText().Trim().ToLower());

                    if (tag == null) continue;

                    tag.UseCount += 1;
                    await _questionRepository.UpdateTag(tag);

                    var selectedTag = new SelectQuestionTag()
                    {
                        QuestionId = question.Id,
                        TagId = tag.Id
                    };

                    await _questionRepository.AddSelectedQuestionTag(selectedTag);
                }

                await _questionRepository.SaveChanges();
            }

            await _userService.UpdateUserScoreAndMedal(createQuestion.UserId, _scoreManagement.AddNewQuestionScore);

            return true;
        }

        public async Task<bool> EditQuestion(EditQuestionViewModel edit)
        {
            var question = await GetQuestionById(edit.Id);

            if (question == null) return false;

            var user = await _userService.GetUserById(edit.UserId);

            if (user == null) return false;

            if (question.UserId != user.Id && !user.IsAdmin)
            {
                return false;
            }

            FileExtensions.ManageEditorImages(question.Content, edit.Description, PathTools.EditorImageServerPath);

            question.Title = edit.Title;
            question.Content = edit.Description;

            await _questionRepository.UpdateQuestion(question);
            await _questionRepository.SaveChanges();

            #region Remove Tags

            var currentTags = question.SelectQuestionTags.ToList();

            if (currentTags.Any())
            {
                foreach (var currentTag in currentTags)
                {
                    await _questionRepository.DeleteSelectedQuestionTag(currentTag);
                }
                await _questionRepository.SaveChanges();
            }

            #endregion

            #region Add Tags

            if (edit.SelectedTags != null && edit.SelectedTags.Any())
            {
                foreach (var questionSelectedTag in edit.SelectedTags)
                {
                    var tag = await _questionRepository.GetTagByName(questionSelectedTag.SanitizeText().Trim().ToLower());

                    if (tag == null) continue;

                    tag.UseCount += 1;
                    await _questionRepository.UpdateTag(tag);

                    var selectedTag = new SelectQuestionTag()
                    {
                        QuestionId = question.Id,
                        TagId = tag.Id
                    };

                    await _questionRepository.AddSelectedQuestionTag(selectedTag);
                }

                await _questionRepository.SaveChanges();
            }

            #endregion

            return true;
        }

        public async Task<FilterQuestionBookmarksViewModel> FilterQuestionBookmarks(FilterQuestionBookmarksViewModel filter)
        {
            var query = _questionRepository.GetAllBookmarks();

            query = query.Where(s => s.UserId == filter.UserId);

            await filter.SetPaging(query.Select(s => s.Question).AsQueryable());

            return filter;
        }

        public async Task<EditQuestionViewModel?> FillEditQuestionViewModel(long questionId, long userId)
        {
            var question = await GetQuestionById(questionId);

            if (question == null) return null;

            var user = await _userService.GetUserById(userId);

            if (user == null) return null;

            if (question.UserId != user.Id && !user.IsAdmin)
            {
                return null;
            }

            var tags = await GetTagListByQuestionId(questionId);

            var result = new EditQuestionViewModel
            {
                Id = question.Id,
                Description = question.Content,
                Title = question.Title,
                SelectedTagsJson = JsonConvert.SerializeObject(tags)
            };

            return result;
        }

        public async Task<FilterQuestionViewModel> FilterQuestions(FilterQuestionViewModel filter)
        {
            var query = await _questionRepository.GetAllQuestions();

            #region Filter By Tag

            if (!string.IsNullOrEmpty(filter.TagTitle))
            {
                query = query.Include(s => s.SelectQuestionTags)
                    .ThenInclude(s => s.Tag)
                    .Where(s => s.SelectQuestionTags.Any(a => a.Tag.Title.Equals(filter.TagTitle)));
            }

            #endregion

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => s.Title.Contains(filter.Title.SanitizeText().Trim()));
            }

            switch (filter.Sort)
            {
                case FilterQuestionSortEnum.NewToOld:
                    query = query.OrderByDescending(s => s.CreateDate);
                    break;
                case FilterQuestionSortEnum.OldToNew:
                    query = query.OrderBy(s => s.CreateDate);
                    break;
                case FilterQuestionSortEnum.ScoreHighToLow:
                    query = query.OrderByDescending(s => s.Score);
                    break;
                case FilterQuestionSortEnum.ScoreLowToHigh:
                    query = query.OrderBy(s => s.Score);
                    break;
            }

            var result = query
                .Include(s => s.Answers)
                .Include(s => s.SelectQuestionTags)
                .ThenInclude(a => a.Tag)
                .Include(s => s.User)
                .Select(s => new QuestionListViewModel()
                {
                    AnswersCount = s.Answers.Count(a => !a.IsDelete),
                    HasAnyAnswer = s.Answers.Any(a => !a.IsDelete),
                    HasAnyTrueAnswer = s.Answers.Any(a => !a.IsDelete && a.IsTrue),
                    QuestionId = s.Id,
                    Score = s.Score,
                    Title = s.Title,
                    ViewCount = s.ViewCount,
                    UserDisplayName = s.User.GetUserDisplayName(),
                    Tags = s.SelectQuestionTags.Where(a => !a.Tag.IsDelete).Select(a => a.Tag.Title).ToList(),
                    AnswerByDisplayName = s.Answers.Any(a => !a.IsDelete) ? s.Answers.OrderByDescending(a => a.CreateDate).First().User.GetUserDisplayName() : null,
                    CreateDate = s.CreateDate.TimeAgo(),
                    AnswerByCreateDate = s.Answers.Any(a => !a.IsDelete) ? s.Answers.OrderByDescending(a => a.CreateDate).First().CreateDate.TimeAgo() : null
                }).AsQueryable();

            await filter.SetPaging(result);

            return filter;
        }

        public async Task<Question?> GetQuestionById(long id)
        {
            return await _questionRepository.GetQuestionById(id);
        }

        public async Task<bool> AnswerQuestion(AnswerQuestionViewModel answerQuestion)
        {
            var question = await GetQuestionById(answerQuestion.QuestionId);

            if (question == null) return false;

            var answer = new Answer()
            {
                Content = answerQuestion.Answer.SanitizeText(),
                QuestionId = answerQuestion.QuestionId,
                UserId = answerQuestion.UserId
            };

            await _questionRepository.AddAnswer(answer);
            await _questionRepository.SaveChanges();

            await _userService.UpdateUserScoreAndMedal(answerQuestion.UserId, _scoreManagement.AddNewAnswerScore);

            return true;
        }

        public async Task AddViewForQuestion(string userIp, Question question)
        {
            if (await _questionRepository.IsExistsViewForQuestion(userIp, question.Id))
            {
                return;
            }

            var view = new QuestionView()
            {
                QuestionId = question.Id,
                UserIP = userIp
            };

            await _questionRepository.AddQuestionView(view);

            question.ViewCount += 1;

            await _questionRepository.UpdateQuestion(question);

            await _questionRepository.SaveChanges();
        }

        public async Task<bool> AddQuestionToBookmark(long questionId, long userId)
        {
            var question = await GetQuestionById(questionId);

            if (question == null) return false;

            if (await _questionRepository.IsExistsQuestionInUserBookmarks(questionId, userId))
            {
                var bookmark = await _questionRepository.GetBookmarkByQuestionAndUserId(questionId, userId);

                if (bookmark == null) return false;

                _questionRepository.RemoveBookmark(bookmark);
            }
            else
            {
                var newBookmark = new UserQuestionBookmark
                {
                    QuestionId = questionId,
                    UserId = userId
                };

                await _questionRepository.AddBookmark(newBookmark);
            }

            await _questionRepository.SaveChanges();

            return true;
        }

        public async Task<bool> IsExistsQuestionInUserBookmarks(long questionId, long userId)
        {
            return await _questionRepository.IsExistsQuestionInUserBookmarks(questionId, userId);
        }

        #endregion

        #region answer

        public async Task<List<Answer>> GetAllQuestionAnswers(long questionId)
        {
            return await _questionRepository.GetAllQuestionAnswers(questionId);
        }

        public async Task<bool> HasUserAccessToSelectTrueAnswer(long userId, long answerId)
        {
            var answer = await _questionRepository.GetAnswerById(answerId);

            if (answer == null) return false;

            var user = await _userService.GetUserById(userId);

            if (user == null) return false;

            if (user.IsAdmin) return true;

            if (answer.Question.UserId != userId)
            {
                return false;
            }

            return true;
        }

        public async Task SelectTrueAnswer(long userId, long answerId)
        {
            var answer = await _questionRepository.GetAnswerById(answerId);

            if (answer == null) return;

            answer.IsTrue = !answer.IsTrue;

            await _questionRepository.UpdateAnswer(answer);
            await _questionRepository.SaveChanges();
        }

        public async Task<CreateScoreForAnswerResult> CreateScoreForAnswer(long answerId, AnswerScoreType type, long userId)
        {
            var answer = await _questionRepository.GetAnswerById(answerId);

            if (answer == null) return CreateScoreForAnswerResult.Error;

            var user = await _userService.GetUserById(userId);

            if (user == null) return CreateScoreForAnswerResult.Error;

            if (type == AnswerScoreType.Minus && user.Score < _scoreManagement.MinScoreForDownScoreAnswer)
            {
                return CreateScoreForAnswerResult.NotEnoughScoreForDown;
            }

            if (type == AnswerScoreType.Plus && user.Score < _scoreManagement.MinScoreForUpScoreAnswer)
            {
                return CreateScoreForAnswerResult.NotEnoughScoreForUp;
            }

            if (await _questionRepository.IsExistsUserScoreForAnswer(answerId, userId))
            {
                return CreateScoreForAnswerResult.UserCreateScoreBefore;
            }

            var score = new AnswerUserScore
            {
                AnswerId = answerId,
                UserId = userId,
                Type = type
            };
            await _questionRepository.AddAnswerUserScore(score);

            if (type == AnswerScoreType.Minus)
            {
                answer.Score -= 1;
            }
            else
            {
                answer.Score += 1;
            }
            await _questionRepository.UpdateAnswer(answer);

            await _questionRepository.SaveChanges();

            return CreateScoreForAnswerResult.Success;
        }

        public async Task<CreateScoreForAnswerResult> CreateScoreForQuestion(long questionId, QuestionScoreType type, long userId)
        {
            var question = await _questionRepository.GetQuestionById(questionId);

            if (question == null) return CreateScoreForAnswerResult.Error;

            var user = await _userService.GetUserById(userId);

            if (user == null) return CreateScoreForAnswerResult.Error;

            if (type == QuestionScoreType.Minus && user.Score < _scoreManagement.MinScoreForDownScoreAnswer)
            {
                return CreateScoreForAnswerResult.NotEnoughScoreForDown;
            }

            if (type == QuestionScoreType.Plus && user.Score < _scoreManagement.MinScoreForUpScoreAnswer)
            {
                return CreateScoreForAnswerResult.NotEnoughScoreForUp;
            }

            if (await _questionRepository.IsExistsUserScoreForQuestion(questionId, userId))
            {
                return CreateScoreForAnswerResult.UserCreateScoreBefore;
            }

            var score = new QuestionUserScore
            {
                QuestionId = questionId,
                UserId = userId,
                Type = type
            };
            await _questionRepository.AddQuestionUserScore(score);

            if (type == QuestionScoreType.Minus)
            {
                question.Score -= 1;
            }
            else
            {
                question.Score += 1;
            }
            await _questionRepository.UpdateQuestion(question);

            await _questionRepository.SaveChanges();

            return CreateScoreForAnswerResult.Success;
        }

        public async Task<EditAnswerViewModel?> FillEditAnswerViewModel(long answerId, long userId)
        {
            var answer = await _questionRepository.GetAnswerById(answerId);

            if (answer == null) return null;

            var user = await _userService.GetUserById(userId);

            if (user == null) return null;

            if (answer.UserId != user.Id && !user.IsAdmin)
            {
                return null;
            }

            return new EditAnswerViewModel { 
                Answer = answer.Content,
                AnswerId = answer.Id,
                QuestionId = answer.QuestionId
            };
        }

        public async Task<bool> EditAnswer(EditAnswerViewModel editAnswerViewModel)
        {
            var answer = await _questionRepository.GetAnswerById(editAnswerViewModel.AnswerId);

            if (answer == null) return false;

            if (answer.QuestionId != editAnswerViewModel.QuestionId) return false;

            var user = await _userService.GetUserById(editAnswerViewModel.UserId);

            if (user == null) return false;

            if (answer.UserId != user.Id && !user.IsAdmin)
            {
                return false;
            }

            answer.Content = editAnswerViewModel.Answer;

            await _questionRepository.UpdateAnswer(answer);
            await _questionRepository.SaveChanges();

            return true;
        }

        #endregion
        
        #region Admin

        public async Task<List<TagViewModelJson>> GetTagViewModelJson()
        {
            var tags = await _questionRepository.GetAllTagsAsQueryable();

            return tags.OrderByDescending(s => s.UseCount)
                .Take(10)
                .Select(s => new TagViewModelJson
                {
                    Title = s.Title,
                    UseCount = s.UseCount
                }).ToList();
        }

        public async Task<FilterTagAdminViewModel> FilterTagAdmin(FilterTagAdminViewModel filter)
        {
            var query = await _questionRepository.GetAllTagsAsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => s.Title.Contains(filter.Title));
            }

            switch (filter.Status)
            {
                case FilterTagAdminStatus.All:
                    break;
                case FilterTagAdminStatus.HasDescription:
                    query = query.Where(s => !string.IsNullOrEmpty(s.Description));
                    break;
                case FilterTagAdminStatus.NoDescription:
                    query = query.Where(s => string.IsNullOrEmpty(s.Description));
                    break;
            }

            await filter.SetPaging(query.OrderByDescending(s => s.CreateDate));

            return filter;
        }

        public async Task CreateTagAdmin(CreateTagAdminViewModel createTagAdminViewModel)
        {
            var tag = new Tag
            {
                Description = createTagAdminViewModel.Description,
                Title = createTagAdminViewModel.Title
            };

            await _questionRepository.AddTag(tag);
            await _questionRepository.SaveChanges();
        }

        public async Task<EditTagAdminViewModel?> FillEditTagAdminViewModel(long id)
        {
            var tag = await _questionRepository.GetTagById(id);

            if (tag == null || tag.IsDelete)
            {
                return null;
            }

            var result = new EditTagAdminViewModel
            {
                Description = tag.Description,
                Id = tag.Id,
                Title = tag.Title
            };

            return result;
        }

        public async Task<bool> EditTagAdmin(EditTagAdminViewModel edit)
        {
            var tag = await _questionRepository.GetTagById(edit.Id);

            if (tag == null || tag.IsDelete)
            {
                return false;
            }

            tag.Title = edit.Title;
            tag.Description = edit.Description;

            await _questionRepository.UpdateTag(tag);
            await _questionRepository.SaveChanges();

            return true;
        }

        public async Task<bool> DeleteTagAdmin(long id)
        {
            var tag = await _questionRepository.GetTagById(id);

            if (tag == null || tag.IsDelete)
            {
                return false;
            }

            tag.IsDelete = true;
            
            await _questionRepository.UpdateTag(tag);
            await _questionRepository.SaveChanges();

            return true;
        }

        #endregion
    }
}
