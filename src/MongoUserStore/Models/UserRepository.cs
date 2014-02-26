using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Omu.ValueInjecter;
using System.Linq;

namespace MongoUserStore.Models
{
    public class UserRepository : IUserRepository
    {
        private MongoHelper _mongoHelper;

        private MongoCollection<IdentityUser> _identityUserCollection;

        public UserRepository()
        {
            _mongoHelper = new MongoHelper();

            _identityUserCollection = _mongoHelper
                .MongoDatabase
                .GetCollection<IdentityUser>("AspNetUsers");
        }

        public IQueryable<TUser> GetUsers<TUser>() where TUser : IdentityUser
        {
            return _identityUserCollection.AsQueryable<TUser>();
        }

        public void AddUsers<TUser>(TUser user) where TUser : IdentityUser
        {
            user.InjectFrom(new { Id = ObjectId.Empty });

            _identityUserCollection.Save(user);
        }

        public void UpdateUsers<TUser>(TUser user) where TUser : IdentityUser
        {
            var users = GetUsers<TUser>();

            TUser target = users.FirstOrDefault(x => x.Id == user.Id);

            target.InjectFrom(
                user,
                new
                {
                    Roles = user.Roles,
                    Logins = user.Logins,
                    Claims = user.Claims
                }
            );

            _identityUserCollection.Save(user);
        }

        public void RemoveUsers<TUser>(string id) where TUser : IdentityUser
        {
            IQueryable query = _identityUserCollection
                .AsQueryable<TUser>()
                .Where(x => x.Id == id);

            IMongoQuery mongoQuery = (query as MongoQueryable<TUser>)
                .GetMongoQuery();

            _identityUserCollection.Remove(mongoQuery);
        }
    }
}