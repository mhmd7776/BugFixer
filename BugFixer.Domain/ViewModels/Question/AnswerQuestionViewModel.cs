using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.ViewModels.Question
{
    public class AnswerQuestionViewModel
    {
        public string Answer { get; set; }

        public long QuestionId { get; set; }

        public long UserId { get; set; }
    }

    public class EditAnswerViewModel
    {
        [Display(Name = "پاسخ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Answer { get; set; }

        public long AnswerId { get; set; }

        public long UserId { get; set; }

        public long QuestionId { get; set; }
    }
}
