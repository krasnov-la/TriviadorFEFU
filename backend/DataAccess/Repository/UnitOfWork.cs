using DataAccess.Models;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public IRepository<AnswerOption> AnswerRepo {get; private set;}
        public IRepository<Question> QuestionRepo {get; private set;}
        public IRepository<User> UserRepo { get; private set; }
        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            AnswerRepo = new Repository<AnswerOption>(db);
            QuestionRepo = new Repository<Question>(db);
            UserRepo = new Repository<User>(db);
        }

        public void Save()
        {
            _db.SaveChangesAsync();
        }
    }
}
