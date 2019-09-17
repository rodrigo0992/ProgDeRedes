using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using System.Net.Sockets;
using Entities;

namespace Server
{
    public class ServerActions
    {
        CourseLogic courseLogic;
        StudentLogic studentLogic;
        public ServerActions(CourseLogic courseLogic, StudentLogic studentLogic)
        {
            this.courseLogic = courseLogic;
            this.studentLogic = studentLogic;
        }

        private static void ReadDataFromStream(int length, NetworkStream networkStream, byte[] dataBytes)
        {
            int totalReceivedData = 0;

            while (totalReceivedData < length)
            {
                var received = networkStream.Read(dataBytes, totalReceivedData, length - totalReceivedData);
                if (received == 0)
                {
                    //bum
                }
                totalReceivedData += received;
            }

        }

        private static string ReceiveMessage(NetworkStream networkStream)
        {
            var dataLengthBytes = new byte[4];
            ReadDataFromStream(4, networkStream, dataLengthBytes);

            var dataLength = BitConverter.ToInt32(dataLengthBytes, 0);
            var dataBytes = new byte[dataLength];

            ReadDataFromStream(dataLength, networkStream, dataBytes);

           return Encoding.UTF8.GetString(dataBytes);

        }

        public void Login(NetworkStream networkStream)
        {
            //Student Number
            var studentNum = ReceiveMessage(networkStream);

            var studentToLogin = this.studentLogic.GetStudentByStudentNum(Convert.ToInt32(studentNum));
            //Password
            var password = ReceiveMessage(networkStream);

            if (studentToLogin.Password == password)
            {
                Console.WriteLine("Pasword correcta");
            }
            else
            {
                Console.WriteLine("Pasword incorrecta");
            }
                
        }

        public void AddStudent()
        {
            Console.WriteLine("CREAR USUARIO");
            Console.WriteLine("Ingrese número de usuario:");
            var studentNum = Console.ReadLine();
            Console.WriteLine("Ingrese nombre:");
            var studentName = Console.ReadLine();
            Console.WriteLine("Ingrese password:");
            var studentPassword = Console.ReadLine();

            Student newStudent = new Student();
            newStudent.StudentNum = Convert.ToInt32(studentNum);
            newStudent.Name = studentName;
            newStudent.Password = studentPassword;

            try
            {
                this.studentLogic.AddStudent(newStudent);
                Console.WriteLine("Usuario creado con éxito");
                Console.WriteLine("Volver a menú");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        public void ListStudents()
        {
            Console.WriteLine("Lista de estudiantes:");
            var students = this.studentLogic.GetStudents();
            foreach (Student item in students)
            {
                Console.WriteLine(item.StudentNum);
            }
        }
    }
}
