using BugFixer.Domain.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.ViewModels.Admin.User;
using BugFixer.Domain.ViewModels.UserPanel.Account;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IUserService
    {
        #region Regitser

        Task<RegisterResult> RegisterUser(RegisterViewModel register);

        #endregion

        #region Login

        Task<LoginResult> CheckUserForLogin(LoginViewModel login);

        Task<User> GetUserByEmail(string email);

        #endregion

        #region Email Activation

        Task<bool> ActivateUserEmail(string activationCode);

        #endregion

        #region Forgot Password

        Task<ForgotPasswordResult> ForgotPassword(ForgotPasswordViewModel forgotPassword);

        #endregion

        #region Reset Password

        Task<ResetPasswordResult> ResetPassword(ResetPasswordViewModel reset);

        Task<User> GetUserByActivationCode(string activationCode);

        #endregion

        #region User Panel

        Task<User?> GetUserById(long userId);

        Task ChangeUserAvatar(long userId, string fileName);

        Task<EditUserViewModel> FillEditUserViewModel(long userId);

        Task<EditUserInfoResult> EditUserInfo(EditUserViewModel editUserViewModel, long userId);

        Task<ChangeUserPasswordResult> ChangeUserPassword(long userId, ChangeUserPasswordViewModel changeUserPassword);

        #endregion

        #region User Question

        Task UpdateUserScoreAndMedal(long userId, int score);

        #endregion

        #region Admin

        #region User

        Task<FilterUserAdminViewModel> FilterUserAdmin(FilterUserAdminViewModel filter);

        Task<EditUserAdminViewModel?> FillEditUserAdminViewModel(long userId);

        Task<EditUserAdminResult> EditUserAdmin(EditUserAdminViewModel editUserAdminViewModel);

        Task<bool> CheckUserPermission(long permissionId, long userId);

        #endregion

        #endregion
    }
}
