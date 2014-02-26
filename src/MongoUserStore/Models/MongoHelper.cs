using MongoDB.Driver;
using System;
using System.Configuration;

namespace MongoUserStore.Models
{
    public class MongoHelper
    {
        private MongoDatabase _mongoDatabase;

        public MongoDatabase MongoDatabase
        {
            get
            {
                return _mongoDatabase;
            }
        }

        private MongoUrl _mongoUrl;

        public MongoUrl MongoUrl
        {
            get
            {
                return _mongoUrl;
            }
        }

        private MongoClient _mongoClient;

        public MongoClient MongoClient
        {
            get
            {
                return _mongoClient;
            }
        }

        private MongoServer _mongoServer;

        public MongoServer MongoServer
        {
            get
            {
                return _mongoServer;
            }
        }

        public MongoHelper()
            : this("DefaultConnection")
        {
        }

        public MongoHelper(string connectionString)
        {
            string _connectionString = ConfigurationManager
                .ConnectionStrings[connectionString]
                .ConnectionString;

            _mongoUrl = new MongoUrl(_connectionString);

            _mongoClient = new MongoClient(_mongoUrl);

            _mongoServer = _mongoClient.GetServer();

            if (_mongoUrl.DatabaseName == null)
            {
                throw new Exception("連線字串中未指定 database name");
            }

            _mongoDatabase = _mongoServer.GetDatabase(_mongoUrl.DatabaseName);
        }
    }
}