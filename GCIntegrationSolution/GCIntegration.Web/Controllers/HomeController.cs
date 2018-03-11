using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GCIntegration.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Private Variabls

        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        #endregion


        public ActionResult Index()
        {
           var events = GetUpcommingEvents();            

            return View(events);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private Events GetUpcommingEvents()
        {
            UserCredential credential;

            var partialPath = "/Infrastructure/WorkingDocs/client_secret.json";
            var fullPath = $"{AppDomain.CurrentDomain.BaseDirectory}{partialPath}";

            using (var stream =
                new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {                
                string credPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Infrastructure/WorkingDocs";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;                
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            //string upcommingEvents = "<h3>Upcoming events:</h3></br>";

            //if (events.Items != null && events.Items.Count > 0)
            //{
            //    foreach (var eventItem in events.Items)
            //    {
            //        string when = eventItem.Start.DateTime.ToString();
            //        if (String.IsNullOrEmpty(when))
            //        {
            //            when = eventItem.Start.Date;
            //        }
            //        upcommingEvents += $"{eventItem.Summary} {when} </br>";
            //    }
            //}
            //else
            //{
            //    upcommingEvents += "No upcoming events found.";
            //}

            return events;
        }
    }
}