using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExpenseTracker.Server.MVC.Startup))]
namespace ExpenseTracker.Server.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
