using DataBase;
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
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Entities;
using System.Configuration;

namespace Server
{
    class Program
    {
        private static bool serverRunning = true;
        private static Thread serverThread;
        private static List<Thread> clientThreads = new List<Thread>();
        private static StudentLogic studentLogic;

        static void Main(string[] args)
        {
            string ip = ConfigurationManager.AppSettings["ip"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            int portBack = Convert.ToInt32(ConfigurationManager.AppSettings["portBack"]);

            var tcpListener = new TcpListener(IPAddress.Parse(ip), port);
            tcpListener.Start(100);

            var tcpListenerBackground = new TcpListener(IPAddress.Parse(ip), portBack);
            tcpListenerBackground.Start(100);


            Information information = new Information();
            CourseLogic courseLogic = new CourseLogic(information);
            studentLogic = new StudentLogic(information);
            ServerActions serverActions = new ServerActions(courseLogic, studentLogic);
            
            
                serverThread = new Thread(() => 
                {
                    while (serverRunning)
                    {
                        try
                        {
                            var tcpClient = tcpListener.AcceptTcpClient();
                            var tcpClientBackground = tcpListenerBackground.AcceptTcpClient();

                            var threadClient = new Thread(() => {

                                Student studentConected = new Student();
                                bool clientRunning = true;
                                var networkStream = tcpClient.GetStream();
                                var networkStreamBackground = tcpClientBackground.GetStream();

                                while (clientRunning)
                                {
                                    try
                                    {
                                        var protocolPackage = Message.ReceiveMessage(networkStream);

                                        switch (protocolPackage.Cmd)
                                        {
                                            case 1:
                                                studentConected = serverActions.Login(protocolPackage.Data, networkStream, tcpClient, tcpClientBackground);
                                                break;
                                            case 2:
                                                serverActions.ListCoursesRequest(studentConected, networkStream);
                                                break;
                                            case 3:
                                                serverActions.AddStudentToCourse(studentConected, protocolPackage.Data, networkStream);
                                                break;
                                            case 4:
                                                serverActions.GetCoursesWithDetails(studentConected, networkStream);
                                                break;
                                            case 5:
                                                serverActions.AddFileToStudentCourse(studentConected, protocolPackage.Data, networkStream);
                                                break;
                                            case 6:
                                                serverActions.GetStudentCourseFiles(studentConected, protocolPackage.Data, networkStream);
                                                break;
                                            case 7:
                                                serverActions.GetFileInitialData(protocolPackage.Data, networkStream);
                                                break;
                                            case 8:
                                                serverActions.GetFilePartData(protocolPackage.Data, networkStream);
                                                break;
                                            case 9:
                                                serverActions.GetFileFinalData(protocolPackage.Data, networkStream);
                                                break;
                                            case 10:
                                                serverActions.DisconectClient(studentConected, networkStream);
                                                clientRunning = false;
                                                break;
                                            case 11:
                                                serverActions.GetEnrolledCourses(studentConected, networkStream);
                                                break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        clientRunning = false;
                                        serverActions.DisconectClient(studentConected);
                                    }

                                };

                            });
                            clientThreads.Add(threadClient);
                            threadClient.Start();

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("El servidor dejó de aceptar nuevos clientes");
                        }

                    }
                });
                serverThread.Start();


            Console.WriteLine("Bienvenido al admin de Aulas");
            Menu(serverActions);
            while (serverRunning)
            {
                Menu(serverActions);
            }
            CloseServer(serverActions);
        }

        private static void Menu(ServerActions serverActions)
        {

            Console.WriteLine("Menu:");
            Console.WriteLine("1 - Crear Estudiante");
            Console.WriteLine("2 - Listar Estudiantes");
            Console.WriteLine("3 - Crear Curso");
            Console.WriteLine("4 - Listar Cursos");
            Console.WriteLine("5 - Eliminar Curso");
            Console.WriteLine("6 - Dar de alta a alumno en curso");
            Console.WriteLine("7 - Asignar nota a alumno");
            Console.WriteLine("8 - Salir");
            var opcion = studentLogic.setNumber("Ingrese una opcion:");
            switch (Convert.ToInt32(opcion))
            {
                case 1:
                    serverActions.AddStudent();
                    break;
                case 2:
                    serverActions.ListStudents();
                    break;
                case 3:
                    serverActions.AddCourse();
                    break;
                case 4:
                    serverActions.ListAllCourses();
                    break;
                case 5:
                    serverActions.DeleteCourse();
                    break;
                case 6:
                    serverActions.AssignStudentToCourse();
                    break;
                case 7:
                    serverActions.AssignGrade();
                    break;
                case 8:
                    serverRunning = false;
                    break;
                default:
                    Console.WriteLine("Debe seleccionar una opción correcta");
                    break;
            }
            Console.WriteLine("");
        }

        private static void CloseServer(ServerActions serverActions)
        {
            try
            {

                serverActions.ClearStudentConections();

                Console.WriteLine("Servidor desconectado");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al desconectar servidor");
            }

        }

    }
}
       