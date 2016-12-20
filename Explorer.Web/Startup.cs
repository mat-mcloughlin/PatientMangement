using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Explorer.Web.Startup))]

namespace Explorer.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(with => with.Bootstrapper = new Bootstrapper());
        }
    }
}
