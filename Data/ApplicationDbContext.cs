using Microsoft.EntityFrameworkCore;
using StudyMATEUpload.Models;

namespace StudyMATEUpload.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        //public DbSet<LearnCourse> LearnCourses { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Quiz> Quizes { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<UserAward> UserAwards { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        //public DbSet<UserFeedback> UserFeedbacks { get; set; }
        //public DbSet<UserLearnCourse> UserLearnCourses { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserQuiz> UserQuizzes { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<UserTest> UserTests { get; set; }
        public DbSet<UserVideo> UserVideos { get; set; }
        public DbSet<Video> Videos { get; set; }

    }
}
