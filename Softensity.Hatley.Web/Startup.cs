using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;

[assembly: OwinStartupAttribute(typeof(Softensity.Hatley.Web.Startup))]
namespace Softensity.Hatley.Web
{
	
    public partial class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            DataProtectionProvider = app.GetDataProtectionProvider();
			app.MapSignalR();
        }
    }
}
