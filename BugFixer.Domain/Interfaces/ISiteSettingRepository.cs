using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.SiteSetting;

namespace BugFixer.Domain.Interfaces
{
    public interface ISiteSettingRepository
    {
        Task<EmailSetting> GetDefaultEmail();
    }
}
