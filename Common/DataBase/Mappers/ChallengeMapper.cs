using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Common.DataBase.Entities;

namespace Common.DataBase.Mappers
{
    internal class ChallengeMapper
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<Challenge>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(new StringObjectIdGenerator());
                //cm.MapProperty(c => c.ProgramingLanguage).SetSerializer(new EnumSerializer<ProgramingLanguage>(BsonType.String));
            });

            BsonClassMap.RegisterClassMap<ChallengeFile>(cm =>
            {
                cm.AutoMap();
                //cm.MapProperty(c => c.FileType).SetSerializer(new EnumSerializer<FileType>(BsonType.String));
            });

            BsonClassMap.RegisterClassMap<TestCase>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}
