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

        public DbSet<Course> Courses { get; set; }
        public DbSet<Quiz> Quizes { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }

    }
}
