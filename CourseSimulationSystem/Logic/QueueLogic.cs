using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class QueueLogic
    {
        MessageQueue mq;
        List<Log> historyLog;

        public QueueLogic(String queuePath)
        {
            this.mq = new MessageQueue(queuePath);
            this.historyLog = new List<Log>();
        }

        public void AddToQueue(String type,String description)
        {
            Log log = new Log
            {
                Type = type,
                Description = description,
            };

            using (mq)
            {
                mq.Send(log);
            }
        }

        public void GetMessage()
        {
            using (mq)
            {
                mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(Log) });
                var l1 = mq.Peek();
                var log = (Log)l1.Body;
                Console.WriteLine("Tipo" + log.Type + " Descripcion: " + log.Description);
                Console.ReadLine();
            }
        }

        public List<Log> GetLogsByType(int type)
        {
            using (mq)
            {
                var msgEnumerator = mq.GetMessageEnumerator2();
                while (msgEnumerator.MoveNext(new TimeSpan(0, 0, 1)))
                {
                    mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(Log) });
                    var msg = mq.ReceiveById(msgEnumerator.Current.Id, new TimeSpan(0, 0, 1));
                    var log = (Log)msg.Body;
                    historyLog.Add(log);
                }

                var messagesToReturn = new List<Log>();
                foreach (var item in historyLog)
                {
                    if (Convert.ToInt32(item.Type) == type)
                        messagesToReturn.Add(item);
                }
                return messagesToReturn;
            }
        }
    }
}
