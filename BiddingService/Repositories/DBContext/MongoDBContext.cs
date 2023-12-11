using MongoDB.Driver;
using VaultSharp.V1.Commons;

namespace BiddingService.Repositories.DBContext
{
    public class MongoDBContext
    {
        private IMongoDatabase _database;
        private IMongoClient _client;
        IConfiguration _configuration;

        public MongoDBContext(IConfiguration configuration, Secret<SecretData> secret)
        {
            _configuration = configuration;
            _client = new MongoClient(secret.Data.Data["ConnectionString"].ToString());
            _database = _client.GetDatabase(_configuration["DatabaseName"]);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}