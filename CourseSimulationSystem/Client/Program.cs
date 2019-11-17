using Logic;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace Client
{
    class Program
    {
        private static bool clientRunning = true;
        public static List<String> notifications = new List<String>();
        public static Thread backgroundThread;
        public static TcpClient tcpClient;
        public static TcpClient tcpClientBackground;
        private static StudentLogic studentLogic;

        public static void Main(string[] args)
        {
            try
            {

                string ipServer = ConfigurationManager.AppSettings["ipServer"];
                string ipClient = ConfigurationManager.AppSettings["ipClient"];
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                int portBack = Convert.ToInt32(ConfigurationManager.AppSettings["portBack"]);

                tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse(ipClient), 0));
                tcpClient.Connect(IPAddress.Parse(ipServer), port);
                var networkStream = tcpClient.GetStream();
                studentLogic = new StudentLogic();
                var courseLogic = new CourseLogic();

                tcpClientBackground = new TcpClient(new IPEndPoint(IPAddress.Parse(ipClient), 0));
                tcpClientBackground.Connect(IPAddress.Parse(ipServer), portBack);

                new Thread(() => ListenNotifications()).Start();

                ClientActions clientActions = new ClientActions(networkStream, courseLogic, studentLogic);

                Console.WriteLine("Bienvenido a Aulas");
                Console.WriteLine("Continue para iniciar sesión");
                Console.ReadLine();

                bool correctLogin = false;
                while (!correctLogin)
                    correctLogin = clientActions.Login();

                Menu(clientActions);
                while (clientRunning)
                {
                    Menu(clientActions);
                }
                CloseClient();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error de conexión con el servidor, reinicie la aplicación");
                Console.ReadKey();
                Environment.Exit(1);
            }


        }

        private static void ListenNotifications()
        {
            var networkStreamBackground = tcpClientBackground.GetStream();
            while (clientRunning)
            {
                try
                {
                    var protocolPackage = Message.ReceiveMessage(networkStreamBackground);
                    notifications.Add(protocolPackage.Data);
                }
                catch (Exception e)
                {
                    clientRunning = false;
                }
            }
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
                Console.WriteLine("No tiene notificaciones");
            }
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("");


            Console.WriteLine("Seleccione una opcion:");
            Console.WriteLine("1- Alta de curso");
            Console.WriteLine("2- Cursos disponibles");
            Console.WriteLine("3- Subir material a curso");
            Console.WriteLine("4- Materiales subidos a curso");
            Console.WriteLine("5- Refrescar bandeja notificaciones");
            Console.WriteLine("6- Salir");
            var opcion = studentLogic.setNumber("Ingrese una opcion:");

            switch (Convert.ToInt32(opcion))
            {
                case 1:
                    clientActions.AddStudentToCourse();
                    break;
                case 2:
                    clientActions.GetCoursesWithDetails();
                    break;
                case 3:
                    clientActions.AddFileToCourse();
                    break;
                case 4:
                    clientActions.GetFiles();
                    break;
                case 5:
                    break;
                case 6:
                    clientRunning = !clientActions.DiscontectFromServer();
                    break;
                default:
                    Console.WriteLine("Debe seleccionar una opción correcta");
                    break;

            }
            Console.WriteLine("");
        }

        private static void CloseClient()
        {
            Console.WriteLine("Se encuentra desconectado");
            Console.ReadKey();
            Environment.Exit(1);

        }


    }
}
