namespace DataAccess;

using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<AnswerOption> Answers {get; set;}
    public DbSet<Question> Questions {get; set;}
    public DbSet<User> Users {get; set;}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex("Login").IsUnique(true);
    }

    public void IncludeQuestionRelations()
    {
        Questions.Include(q => q.Options);
    }
}