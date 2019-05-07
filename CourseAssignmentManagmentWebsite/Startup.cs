using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourseAssignmentManagmentWebsite.Startup))]
namespace CourseAssignmentManagmentWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
