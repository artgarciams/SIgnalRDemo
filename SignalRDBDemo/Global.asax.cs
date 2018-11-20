using Microsoft.AspNet.SignalR;
using Owin;
using SignalRDBDemo.Hubs;
using SignalRDBDemo.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SignalRDBDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            RegisterNotification();
        }

        private void RegisterNotification()
        {
            //Get the connection string from the Web.Config file. Make sure that the key exists and it is the connection string for the Notification Database and the NotificationList Table that we created


            string connectionString = ConfigurationManager.ConnectionStrings["NotificationConnection"].ConnectionString;

            //connectionString = "Server=tcp:aagsqldb.database.windows.net,1433;Initial Catalog=signalRdemoDB;Persist Security Info=False;User ID=artg;Password=Razz17012345;";
            
            //We have selected the entire table as the command, so SQL Server executes this script and sees if there is a change in the result, raise the event
            string commandText = @"
                                    Select
                                        dbo.RepoTable.ID,
                                        dbo.RepoTable.Text,
                                        dbo.RepoTable.UserID,
                                        dbo.RepoTable.DateStamp                                      
                                    From
                                        dbo.RepoTable                                     
                                    ";

            //Start the SQL Dependency
            SqlDependency.Start(connectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    var sqlDependency = new SqlDependency(command);


                    sqlDependency.OnChange += new OnChangeEventHandler(sqlDependency_OnChange);

                    // NOTE: You have to execute the command, or the notification will never fire.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }

        DateTime LastRun;
        private void sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {

            if (e.Info == SqlNotificationInfo.Insert || e.Info == SqlNotificationInfo.Update || e.Info == SqlNotificationInfo.Delete)
            {
                //This is how signalrHub can be accessed outside the SignalR Hub Notification.cs file
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

                NotificationRespository objRepos = new NotificationRespository();
                List<NotificationList> objList = objRepos.GetLatestNotifications(LastRun);

                LastRun = DateTime.Now.ToUniversalTime();
                
                foreach (var item in objList)
                {
                    //replace domain name with your own domain name
                    context.Clients.User("SignalRDBDemo" + item.UserID).addLatestNotification(item);
                }

            }


            LastRun = DateTime.Now.ToUniversalTime();

            //Call the RegisterNotification method again
            RegisterNotification();
        }
    }
}
