
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web2
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

			routes.MapRoute (
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = "" }
			);

		}

		public static void RegisterGlobalFilters (GlobalFilterCollection filters)
		{
			filters.Add (new HandleErrorAttribute ());
		}

		protected void Application_Start ()
		{
            //加载QueryEntity的配置
            string file = Server.MapPath("~/Content/QueryConfig.json");
            
            ColysSharp.DataBase.DBContextConfig config = new ColysSharp.DataBase.DBContextConfig()
            {                
                ConnectionString = web2.Controllers.HomeController.getSetting(Server, "connstr",false),
                DatabaseType = "mysql",
                EntityTypeFormat = "Web2.Models.{0},Web2"
            };            
            ColysSharp.DataBase.DBContext.LoadConfigFromFile(file, config);
			log4net.Config.XmlConfigurator.Configure();//这句代码	
			AreaRegistration.RegisterAllAreas ();
			RegisterGlobalFilters (GlobalFilters.Filters);
			RegisterRoutes (RouteTable.Routes);
		}

		protected void Application_Error(Object sender, EventArgs e) {
			Exception ex = Server.GetLastError ();
			log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
			log.Error("Application_Error", ex);
		}
	}
}
