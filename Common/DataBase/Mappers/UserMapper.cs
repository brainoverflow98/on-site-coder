using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Common.DataBase.Entities;

namespace Common.DataBase.Mappers
{
    internal class UserMapper
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(new StringObjectIdGenerator());
                //cm.MapProperty(c => c.Role).SetSerializer(new EnumSerializer<Role>(BsonType.String));
            });           
        }
    }
}
