using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MongoUserStore.Models
{
    public class IdentityUser : IUser
    {
        public IdentityUser()
        {
            this.Claims = new List<UserClaim>();

            this.Roles = new List<string>();

            this.Logins = new List<UserLoginInfo>();
        }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }

        /*IUser*/

        #region IUser

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }

        public virtual string UserName { get; set; }

        #endregion IUser

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual List<string> Roles { get; set; }

        public virtual List<UserLoginInfo> Logins { get; set; }

        public virtual List<UserClaim> Claims { get; set; }
    }
}