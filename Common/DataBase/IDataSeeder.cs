using MongoDB.Driver;

namespace Common.DataBase
{
    public interface IDataSeeder
    {
        public void Seed(IDbContext db);
    }
}
