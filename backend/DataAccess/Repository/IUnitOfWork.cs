using DataAccess.Models;

namespace DataAccess.Repository;
public interface IUnitOfWork
{
    IRepository<AnswerOption> AnswerRepo { get; }
    IRepository<FriendRelation> FriendRepo {get;}
    IRepository<Question> QuestionRepo { get; }
    IRepository<User> UserRepo { get; }
    void Save();
}