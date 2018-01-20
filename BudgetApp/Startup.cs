using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BudgetApp.Startup))]
namespace BudgetApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
