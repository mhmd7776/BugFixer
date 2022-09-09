using System.ComponentModel.DataAnnotations;
using BugFixer.Domain.ViewModels.Common;

namespace BugFixer.Domain.ViewModels.Admin.User;

public class FilterUserAdminViewModel : Paging<Entities.Account.User>
{
    public FilterUserAdminViewModel()
    {
        ActivationStatus = AccountActivationStatus.All;
    }
    
    public string? UserSearch { get; set; }

    public AccountActivationStatus ActivationStatus { get; set; }
}

public enum AccountActivationStatus
{
    [Display(Name = "همه")] All,
    [Display(Name = "حساب کاربری فعال")] IsActive,
    [Display(Name = "حساب کاربری غیرفعال")] NotActive
}