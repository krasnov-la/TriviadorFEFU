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

    public void InitializeData()
    {
        if (!Questions.Any())
        {
            var question1 = new Question
            {
                Text = "Какая столица Франции?",
                Options = new List<AnswerOption>
                {
                    new AnswerOption { Text = "Париж", Correct = true },
                    new AnswerOption { Text = "Лондон", Correct = false },
                    new AnswerOption { Text = "Берлин", Correct = false }
                }
            };

            var question2 = new Question
            {
                Text = "Сколько планет в солнечной системе?",
                Options = new List<AnswerOption>
                {
                    new AnswerOption { Text = "8", Correct = true },
                    new AnswerOption { Text = "7", Correct = false },
                    new AnswerOption { Text = "9", Correct = false }
                }
            };

            Questions.AddRange(question1, question2);
            SaveChanges();
        }
    }

}