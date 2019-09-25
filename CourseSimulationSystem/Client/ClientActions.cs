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
        NetworkStream networkStream;

        public ClientActions(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        public bool Login()
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

        public void AddStudentToCourse()
        {
            var data = "";
            Message.SendMessage(networkStream, "REQ", 2, data); 
            var protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var courseList = protocolPackageResponse.Data;
            if (courseList != "")
            {
                Console.WriteLine("");
                Console.WriteLine("Cursos disponibles para inscribirse:");
                ShowCourses(courseList);
                Console.WriteLine("Escriba el nombre del curso al que desea inscribirse:");
                var course = Console.ReadLine();
                Message.SendMessage(networkStream, "REQ", 3, course);

                protocolPackageResponse = Message.ReceiveMessage(networkStream);
                var loginResponse = protocolPackageResponse.Data;
                Console.WriteLine(protocolPackageResponse.Data);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("No tiene cursos disponibles para inscribirse");
            }

        }

        public void getEnrolledCourses()
        {
            var data = "";
            Message.SendMessage(networkStream, "REQ", 4, data);
            var protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var courseList = protocolPackageResponse.Data;
            Console.WriteLine("");
            Console.WriteLine("Se encuentra inscripto a los siguientes cursos:");
            ShowCourses(courseList);
        }

        public void AddFileToCourse()
        {
            Message.SendMessage(networkStream, "REQ", 4, "");
            var protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var courseList = protocolPackageResponse.Data;
            if (courseList != "")
            {
                Console.WriteLine("");
                Console.WriteLine("Cursos disponibles para subir un material:");
                ShowCourses(courseList);
                Console.WriteLine("Escriba el nombre del curso al que desea subir un material:");
                var course = Console.ReadLine();
                Console.WriteLine("Ingrese el nombre del material:");
                var name = Console.ReadLine();
                Console.WriteLine("Ingrese src a material:");
                var src = Console.ReadLine();


                var data = @"{course:'" + course + "', filesource:'" + src + "', name:'" + name + "'}";

                Message.SendMessage(networkStream, "REQ", 5, data);

                protocolPackageResponse = Message.ReceiveMessage(networkStream);
                Console.WriteLine(protocolPackageResponse.Data);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("No tiene cursos disponibles para subir un material");
            }

        }

        public void GetFiles()
        {
            Message.SendMessage(networkStream, "REQ", 4, "");
            var protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var courseList = protocolPackageResponse.Data;
            Console.WriteLine("");
            Console.WriteLine("Se encuentra inscripto a los siguientes cursos:");
            ShowCourses(courseList);

            Console.WriteLine("");
            Console.WriteLine("Escriba el nombre del curso al que desea consultar los materiales:");
            var course = Console.ReadLine();

            Message.SendMessage(networkStream, "REQ", 6, course);
            protocolPackageResponse = Message.ReceiveMessage(networkStream);
            var fileList = protocolPackageResponse.Data;
            Console.WriteLine("");
            Console.WriteLine("Materiales subidos al curso seleccionado:");
            ShowCourses(fileList);
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
