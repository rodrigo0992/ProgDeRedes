using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public struct StudentSocket
    {
        public Student student;
        public TcpClient tcpClient;

        public StudentSocket(Student student, TcpClient tcpClient)
        {
            this.student = student;
            this.tcpClient = tcpClient;
        }
    }
}
