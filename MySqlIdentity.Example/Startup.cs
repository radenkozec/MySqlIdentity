using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MySqlIdentity.Example.Startup))]
namespace MySqlIdentity.Example
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
