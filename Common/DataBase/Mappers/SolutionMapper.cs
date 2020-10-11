using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Common.DataBase.Entities;

namespace Common.DataBase.Mappers
{
    internal class SolutionMapper
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<Solution>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(new StringObjectIdGenerator());
                //cm.MapProperty(c => c.ProgramingLanguage).SetSerializer(new EnumSerializer<ProgramingLanguage>(BsonType.String));
            });

            BsonClassMap.RegisterClassMap<SolutionFile>(cm =>
            {
                cm.AutoMap();
            });
        }        
    }
}
