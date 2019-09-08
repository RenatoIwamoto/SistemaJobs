using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemaJobs.Startup))]
namespace SistemaJobs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
