using Entities;
using Logic;
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

        // GET api/logs
        public IHttpActionResult Get(int type)
        {
            var queuePath = @"FormatName:Direct=TCP:192.168.1.148\Private$\logqueue";
            QueueLogic queueLogic = new QueueLogic(queuePath);
            ICollection<Log> returnLogs = queueLogic.GetLogsByType(type);

            List<String> listLogs = new List<String>();
            //foreach (var log in listLogs)
            //{
            //    var logModel = new LogModel()
            //    {
            //        Type= log.Type,
            //        Description= log.Description
            //    };
            //    listLogs.Add(logModel);
            //}

            foreach (var msj in returnLogs)
            {
                String line = "Tipo: " + msj.Type + " Descripcion: " + msj.Description;
                listLogs.Add(line);
            }
            return Ok(listLogs);
        }
    }
}
