using Logic;
using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientActions
    {
        NetworkStream networkStream;
        StudentLogic studentLogic;
        CourseLogic courseLogic;
        public ClientActions(NetworkStream networkStream, CourseLogic courseLogic, StudentLogic studentLogic)
        {
            this.networkStream = networkStream;
            this.courseLogic = courseLogic;
            this.studentLogic = studentLogic;
        }

        public bool Login()
        {
            var studentNum = studentLogic.setName("Ingrese su numero de usuario o mail");
            var studentPassword = studentLogic.setName("Ingrese su contrasena");

            var data = studentNum + ";" + studentPassword;

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
                var course = courseLogic.setName("Escriba el nombre del curso al que desea inscribirse:");
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
                var course = courseLogic.setName("Escriba el nombre del curso al que desea subir un material:");
                var name = courseLogic.setName("Ingrese el nombre del material:");
                var src = courseLogic.setName("Ingrese src al material");

                if (SendFile(src))
                {
                    var data = course + ";" + src + ";" + name;
                    Message.SendMessage(networkStream, "REQ", 5, data);

                    protocolPackageResponse = Message.ReceiveMessage(networkStream);
                    Console.WriteLine(protocolPackageResponse.Data);
                }

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

        public bool SendFile(string src)
        {
            bool exito = true;
            src = Path.Combine(src, "");
            const int PART_SIZE = 9999;
            try
            {
                FileInfo fileInfo = new FileInfo(src);
                var extension = fileInfo.Extension;
                var name = fileInfo.Name;
                var fileLength = (int)fileInfo.Length;
                double fileSplits = (double)fileLength / PART_SIZE;

                var initialData = fileLength + ";" + name;
                Message.SendMessage(networkStream, "REQ", 7, initialData);
                var protocolPackageResponse = Message.ReceiveMessage(networkStream);
                if (protocolPackageResponse.Data == "OK")
                {
                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                        var lengthRead = 0;
                        while (lengthRead < fileLength)
                        {
                            byte[] partToSend = new byte[PART_SIZE];
                            lengthRead += fileStream.Read(partToSend, 0,PART_SIZE);

                            var partToSendString = Convert.ToBase64String(partToSend);

                            if (fileSplits > 1)
                            {
                                Message.SendMessage(networkStream, "REQ", 8, partToSendString);
                            }
                            else
                            {
                                Message.SendMessage(networkStream, "REQ", 9, partToSendString);
                            }
                            protocolPackageResponse = Message.ReceiveMessage(networkStream);

                            fileSplits = fileSplits - 1;

                            if (protocolPackageResponse.Data != "OK")
                            {
                                Console.WriteLine("No se pudo enviar el archivo");
                                exito = false;
                                break;
                            }

                        }
                    }
                    Console.WriteLine("Material enviado exitosamente");

                }
                else
                {
                    exito = false;
                    Console.WriteLine("No se puede enviar el material: " + protocolPackageResponse.Data);
                }
                        


            }
            catch (Exception e)
            {
                exito = false;
                Console.WriteLine("El path es incorrecto");
            }

            return exito;
        }

        public void ShowCourses(string courseList)
        {
            string[] list = courseList.Split('-');
            foreach (string course in list)
            {
                Console.WriteLine(course);
            }
        }

        public bool ValidateNumber(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
        public static bool isEmpty(string s)
        {
            return (s == null || s == String.Empty) ? true : false;
        }

        public String setNumber()
        {
            var studentNum = "";
            bool isCorrect = false;
            while (!isCorrect)
            {
                Console.WriteLine("Ingrese numero:");
                studentNum = Console.ReadLine();
                isCorrect = ValidateNumber(studentNum) && !isEmpty(studentNum);
            }
            return studentNum;
        }
    }
}
