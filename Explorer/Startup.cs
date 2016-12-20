using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Explorer.Startup))]

namespace Explorer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(with => with.Bootstrapper = new Bootstrapper());
        }
    }
}
