using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReWare.Startup))]
namespace ReWare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
