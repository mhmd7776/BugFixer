﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.ViewModels.Question
{
    public class EditQuestionViewModel
    {
        [Display(Name = "عنوان")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        public List<string>? SelectedTags { get; set; }

        public string? SelectedTagsJson { get; set; }

        public long UserId { get; set; }

        public long Id { get; set; }
    }
}
