using MongoDB.Driver;
using Common.DataBase.Entities;

namespace Common.DataBase
{
    public interface IDbContext
    {
        public IMongoCollection<User> Users { get; }
        public IMongoCollection<Challenge> Challenges { get; }
        public IMongoCollection<Solution> Solutions { get; }
    }
}
