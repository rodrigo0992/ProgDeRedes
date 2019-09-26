using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static bool clientRunning = true;
        public static List<String> notifications = new List<String>();

        static void Main(string[] args)
        {
            
            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("172.29.0.63"), 0));
            tcpClient.Connect(IPAddress.Parse("172.29.0.63"), 6000);
            var networkStream = tcpClient.GetStream();

            var tcpClientBackground = new TcpClient(new IPEndPoint(IPAddress.Parse("172.29.0.63"), 0));
            tcpClientBackground.Connect(IPAddress.Parse("172.29.0.63"), 7000);
            var networkStreamBackground = tcpClientBackground.GetStream();
            var threadClient = new Thread(() => {
                while (clientRunning)
                {
                    var protocolPackage = Message.ReceiveMessage(networkStreamBackground);

                    notifications.Add(protocolPackage.Data);
                }
            });
            threadClient.Start();


            ClientActions clientActions = new ClientActions(networkStream);

            Console.WriteLine("Bienvenido a Aulas");
            Console.WriteLine("Continue para iniciar sesión");
            Console.ReadLine();

            bool correctLogin = false;
            while(!correctLogin)
                correctLogin = clientActions.Login();
            
            Menu(clientActions);
            while (clientRunning)
            {
                Menu(clientActions);
            }

            //networkStream.Close();
            //tcpClient.Dispose();
        }

        private static void Menu(ClientActions clientActions)
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("NOTIFICACIONES:");
            if (notifications.Count > 0)
            {
                foreach (var item in notifications)
                {
                    try
                    {
                        var dataArray = Message.Deserialize(item);
                        var courseName = dataArray[0];
                        var materialName = dataArray[1];
                        var grade = dataArray[2];

                        Console.WriteLine("Curso: " + courseName + " - Material: " + materialName + " - Resultado: " + grade);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error al procesar notificación");
                    }
                }
                notifications.Clear();
            }
            else
            {
                Console.WriteLine("No tienen notificaciones");
            }
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("");


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
