using DataBase;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcpListener = new TcpListener(IPAddress.Parse("172.29.4.253"), 6000);
            tcpListener.Start(100);

            Information information = new Information();
            CourseLogic courseLogic = new CourseLogic(information);
            StudentLogic studentLogic = new StudentLogic(information);
            ServerActions serverActions = new ServerActions(courseLogic, studentLogic);

            var thread = new Thread(() => 
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                var networkStream = tcpClient.GetStream();

                serverActions.Login(networkStream);
            });
            thread.Start();

            Console.WriteLine("Bienvenido al admin de Aulas");
            Menu(serverActions);
            while (true)
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
                default:
                    Console.WriteLine("Debe seleccionar una opción correcta");
                    break;
            }

        }

    }
}
       