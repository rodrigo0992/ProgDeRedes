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

namespace Server
{
    class Program
    {
        private static bool serverRunning = true;

        static void Main(string[] args)
        {
            var tcpListener = new TcpListener(IPAddress.Parse("192.168.1.43"), 6000);
            tcpListener.Start(100);


            Information information = new Information();
            CourseLogic courseLogic = new CourseLogic(information);
            StudentLogic studentLogic = new StudentLogic(information);
            ServerActions serverActions = new ServerActions(courseLogic, studentLogic);
            
            
                var thread = new Thread(() => 
                {
                    while (serverRunning)
                    {
                        var tcpClient = tcpListener.AcceptTcpClient();

                        var threadClient = new Thread(() => {

                            Student studentConected = new Student();
                            bool clientRunning = true;
                            var networkStream = tcpClient.GetStream();

                            while (clientRunning)
                            {
                                var protocolPackage = Message.ReceiveMessage(networkStream);

                                switch (protocolPackage.Cmd)
                                {
                                    case 1:
                                        studentConected = serverActions.Login(protocolPackage.Data, networkStream, tcpClient);
                                        break;
                                    case 2:
                                        serverActions.ListCoursesRequest(studentConected, networkStream);
                                        break;
                                    case 3:
                                        serverActions.AddStudentToCourse(studentConected,protocolPackage.Data, networkStream);
                                        break;
                                    case 4:
                                        serverActions.GetEnrolledCourses(studentConected,networkStream);
                                        break;
                                    case 5:
                                        serverActions.AddFileToStudentCourse(studentConected, protocolPackage.Data,networkStream);
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

                                }
                            };

                            networkStream.Close();
                            tcpClient.Dispose();

                        });
                        threadClient.Start();


                    }
                });
                thread.Start();


            Console.WriteLine("Bienvenido al admin de Aulas");
            Menu(serverActions);
            while (serverRunning)
            {
                Menu(serverActions);
            }

        }

        private static void Menu(ServerActions serverActions)
        {
            
            Console.WriteLine("Seleccione una opción");
            Console.WriteLine("1 - Crear Estudiante");
            Console.WriteLine("2 - Listar Estudiantes");
            Console.WriteLine("3 - Crear Curso");
            Console.WriteLine("4 - Listar Cursos");
            Console.WriteLine("5 - Eliminar Curso");
            Console.WriteLine("6 - Dar de alta a alumno en curso");
            Console.WriteLine("7 - Asignar nota a alumno");
            Console.WriteLine("8 - Salir");
            var opcion = Console.ReadLine();

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
                    serverActions.ListCourses();
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

    }
}
       