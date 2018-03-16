using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Music.Web.App_Start;

namespace Music.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
	        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

	        AuthDbConfig.RegisterAdmin();
		}
    }
}
