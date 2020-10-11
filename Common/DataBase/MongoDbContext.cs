using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Linq;
using Common.DataBase.Entities;
using Common.DataBase.Mappers;
using Common.Environment;

namespace Common.DataBase
{
    public class MongoDbContext : IDbContext
    {
        private static bool isInitialized;        
        public MongoDbContext(DatabaseSettings settings, IDataSeeder seeder)
        {
            var client = BuildClient(settings.ConnectionString, settings.LogCommands);          
            if (!isInitialized)
            {                
                ConfigureMapping();
                var dbExsists = client.ListDatabaseNames().ToList().Any(n => n == settings.DatabaseName);
                if (!dbExsists)
                {
                    _database = client.GetDatabase(settings.DatabaseName);
                    seeder.Seed(this);
                }
                isInitialized = true;

            }
            if(_database == null)
                _database = client.GetDatabase(settings.DatabaseName);
        }

        private readonly IMongoDatabase _database;
        private IMongoCollection<User> _users;
        private IMongoCollection<Challenge> _challenges;
        private IMongoCollection<Solution> _solutions;

        public IMongoCollection<User> Users => _users ?? (_users = _database.GetCollection<User>("Users"));
        public IMongoCollection<Challenge> Challenges => _challenges ?? (_challenges = _database.GetCollection<Challenge>("Challenges"));
        public IMongoCollection<Solution> Solutions => _solutions ?? (_solutions = _database.GetCollection<Solution>("Solutions"));


        private MongoClient BuildClient(string connectionString, bool logCommands)
        {
            MongoClientSettings settings;
            if(logCommands)
            {
                settings = new MongoClientSettings()
                {
                    Server = new MongoServerAddress(connectionString),
                    ClusterConfigurator = cb =>
                    {
                        cb.Subscribe<CommandStartedEvent>(e =>
                        {
                            Console.WriteLine($"{e.Command.ToJson()}");
                        });
                    }
                };
            }
            else
            {
                settings = new MongoClientSettings()
                {
                    Server = new MongoServerAddress(connectionString)
                };
            }

            return new MongoClient(settings);
        }

        private void ConfigureMapping()
        {
            UserMapper.Map();
            ChallengeMapper.Map();
            SolutionMapper.Map();
        }
    }
}
