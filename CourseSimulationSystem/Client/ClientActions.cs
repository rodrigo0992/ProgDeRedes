using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientActions
    { 
        public bool Login(NetworkStream networkStream)
        {
            Console.WriteLine("Ingrese su número de usuario:");
            var studentNum = Console.ReadLine();
            Console.WriteLine("Ingrese su contraseña:");
            var studentPassword = Console.ReadLine();

            var data = @"{studentNum:'" + studentNum + "', password:'" + studentPassword + "'}";

            Message.SendMessage(networkStream, "REQ", 01, data);

            var protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var loginResponse = protocolPackageResponse.Data;
            Console.WriteLine(protocolPackageResponse.Data);

            if (loginResponse.Equals("Password correcta"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddStudentToCourse(NetworkStream networkStream)
        {
            var data = "abc";
            Message.SendMessage(networkStream, "REQ", 2, data); // 
            var protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var courseList = protocolPackageResponse.Data;
            ShowCourses(courseList);
            Console.WriteLine("Escriba el nombre del curso al que desea anotarse:");
            var course= Console.ReadLine();
            Message.SendMessage(networkStream, "REQ", 3, course);
        }

        public void ShowCourses(string courseList)
        {
            string[] list = courseList.Split('-');
            foreach (string course in list)
            {
                Console.WriteLine(course);
            }
        }
    }
}
