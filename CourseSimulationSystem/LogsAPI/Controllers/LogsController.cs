using Entities;
using Logic;
using RemoteService;
using RemoteServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LogsAPI.Controllers
{
    public class LogsController : ApiController
    {
        private QueueLogic ql { get; set; }
        private QueueLogic queueLogic { get; set; }

        public LogsController()
        {
            String routeLog = System.Configuration.ConfigurationManager.AppSettings["routeLog"];
            var queuePath = @"FormatName:Direct=TCP:" + routeLog;
            //var queuePath = @"FormatName:Direct=TCP:192.168.1.6\Private$\logqueue";

            queueLogic = new QueueLogic(queuePath);
        }

        // GET api/logs
        public IHttpActionResult Get(int type)
        {

            String remoteRoute = System.Configuration.ConfigurationManager.AppSettings["remoteRoute"];
            IRemote Remote = (IRemote)Activator.GetObject(
                        typeof(IRemote),
                        "tcp://" + remoteRoute);

            ICollection<Log> returnLogs = queueLogic.GetLogsByType(type, Remote);

            List<String> listLogs = new List<String>();

            foreach (var msj in returnLogs)
            {
                String line = "Tipo: " + msj.Type + " Descripcion: " + msj.Description;
                listLogs.Add(line);
            }
            return Ok(listLogs);
        }
    }
}
