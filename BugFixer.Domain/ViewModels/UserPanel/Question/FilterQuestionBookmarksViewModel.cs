using BugFixer.Domain.ViewModels.Common;

namespace BugFixer.Domain.ViewModels.UserPanel.Question;

public class FilterQuestionBookmarksViewModel : Paging<Domain.Entities.Questions.Question>
{
    public long UserId { get; set; }
}