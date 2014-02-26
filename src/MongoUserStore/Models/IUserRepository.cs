using System.Linq;

namespace MongoUserStore.Models
{
    internal interface IUserRepository
    {
        IQueryable<TUser> GetUsers<TUser>() where TUser : IdentityUser;

        void AddUsers<TUser>(TUser user) where TUser : IdentityUser;

        void UpdateUsers<TUser>(TUser user) where TUser : IdentityUser;

        void RemoveUsers<TUser>(string id) where TUser : IdentityUser;
    }
}