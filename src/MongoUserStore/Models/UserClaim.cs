namespace MongoUserStore.Models
{
    public class UserClaim
    {
        public UserClaim(string claimType, string claimValue)
        {
            this.ClaimType = claimType;

            this.ClaimValue = claimValue;
        }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}