using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MongoUserStore.Startup))]
namespace MongoUserStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
