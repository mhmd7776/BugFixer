using BugFixer.Application.Services.Implementations;
using BugFixer.Application.Services.Interfaces;
using BugFixer.DataLayer.Repositories;
using BugFixer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BugFixer.IoC
{
    public class DependencyContainer
    {
        public static void RegisterDependencies(IServiceCollection services)
        {
            #region Services

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<IQuestionService, QuestionService>();

            #endregion

            #region Repositories

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();

            #endregion
        }
    }
}
