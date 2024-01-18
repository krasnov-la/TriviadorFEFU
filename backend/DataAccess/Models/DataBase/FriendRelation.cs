using Microsoft.EntityFrameworkCore;
namespace DataAccess.Models;

public class FriendRelation
{
    public Guid PersonId {get; set;}
    public Guid FriendId {get; set;}
}