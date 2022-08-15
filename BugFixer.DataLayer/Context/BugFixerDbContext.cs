using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Location;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.SiteSetting;
using BugFixer.Domain.Entities.Tags;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Context
{
    public class BugFixerDbContext : DbContext
    {
        #region Ctor

        public BugFixerDbContext(DbContextOptions<BugFixerDbContext> options) : base(options)
        {
            
        }

        #endregion

        #region DbSet

        public DbSet<User> Users { get; set; }

        public DbSet<EmailSetting> EmailSettings { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<UserQuestionBookmark> Bookmarks { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<QuestionView> QuestionViews { get; set; }

        public DbSet<SelectQuestionTag> SelectQuestionTags { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<RequestTag> RequestTags { get; set; }

        public DbSet<QuestionUserScore> QuestionUserScores { get; set; }

        public DbSet<AnswerUserScore> AnswerUserScores { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relation in modelBuilder.Model.GetEntityTypes().SelectMany(s => s.GetForeignKeys()))
            {
                relation.DeleteBehavior = DeleteBehavior.Restrict;
            }

            #region Seed Data

            var date = DateTime.MinValue;

            modelBuilder.Entity<EmailSetting>().HasData(new EmailSetting()
            {
                CreateDate = date,
                DisplayName = "BugFixer",
                EnableSSL = true,
                From = "bugfixer.toplearn@gmail.com",
                Id = 1,
                IsDefault = true,
                IsDelete = false,
                Password = "strong@password",
                Port = 587,
                SMTP = "smtp.gmail.com"
            });

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
