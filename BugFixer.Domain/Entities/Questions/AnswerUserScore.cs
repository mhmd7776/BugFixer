using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Enums;

namespace BugFixer.Domain.Entities.Questions
{
    public class AnswerUserScore : BaseEntity
    {
        #region Properties

        public long UserId { get; set; }

        public long AnswerId { get; set; }

        public AnswerScoreType Type { get; set; }

        #endregion

        #region Relations

        public User User { get; set; }

        public Answer Answer { get; set; }

        #endregion
    }
}
