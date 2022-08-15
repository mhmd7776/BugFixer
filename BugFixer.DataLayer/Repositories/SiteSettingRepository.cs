using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.SiteSetting;
using BugFixer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Repositories
{
    public class SiteSettingRepository : ISiteSettingRepository
    {
        #region Ctor

        private BugFixerDbContext _context;

        public SiteSettingRepository(BugFixerDbContext context)
        {
            _context = context;
        }

        #endregion

        public async Task<EmailSetting> GetDefaultEmail()
        {
            return await _context.EmailSettings.FirstOrDefaultAsync(s => !s.IsDelete && s.IsDefault);
        }
    }
}
