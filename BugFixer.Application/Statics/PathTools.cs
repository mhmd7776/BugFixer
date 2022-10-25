using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Statics
{
    public static class PathTools
    {
        #region User

        public static readonly string DefaultUserAvatar = "DefaultAvatar.png";

        public static readonly string UserAvatarServerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/user/");
        public static readonly string UserAvatarPath = "/content/user/";

        #endregion

        #region Site

        public static readonly string SiteAddress = "https://bugfixer.mohammadmahdavi.com";

        #endregion

        #region Ckeditor

        public static readonly string EditorImageServerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/ckeditor/");
        public static readonly string EditorImagePath = "/content/ckeditor/";

        #endregion
    }
}
