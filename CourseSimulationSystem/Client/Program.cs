using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static TcpListener tcpListener;

        static void Main(string[] args)
        {
            
            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("192.168.1.42"), 0));
            tcpClient.Connect(IPAddress.Parse("192.168.1.42"), 6000);
            var networkStream = tcpClient.GetStream();

            ClientActions clientActions = new ClientActions(networkStream);

            Console.WriteLine("Bienvenido a Aulas");
            Console.WriteLine("Continue para iniciar sesión");
            Console.ReadLine();

            bool correctLogin = false;
            while(!correctLogin)
                correctLogin = clientActions.Login();
            
            Menu(clientActions);
            while (true)
            {
                Menu(clientActions);
            }

            // networkStream.Close();
            //tcpClient.Dispose();
            Console.ReadLine();
        }

        private static void Menu(ClientActions clientActions)
        {
            Console.WriteLine("Seleccione una opcion:");
            Console.WriteLine("1- Alta de curso");
            Console.WriteLine("2- Cursos inscripto");
            Console.WriteLine("3- Subir material a curso");
            Console.WriteLine("4- Materiales subidos a curso");
            var opcion = Console.ReadLine();

            switch (Convert.ToInt32(opcion))
            {
                case 1:
                    clientActions.AddStudentToCourse();
                    break;
                case 2:
                    clientActions.getEnrolledCourses();
                    break;
                case 3:
                    clientActions.AddFileToCourse();
                    break;
                case 4:
                    clientActions.GetFiles();
                    break;
                default:
                    Console.WriteLine("Debe seleccionar una opción correcta");
                    break;

            }
            Console.WriteLine("");
        }



    }
}
