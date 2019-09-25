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

                if (SendFile(src))
                {
                    var data = @"{course:'" + course + "', filesource:'" + src + "', name:'" + name + "'}";
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

                var initialData = @"{fileLength:'" + fileLength + "', name:'" + name + "'}";
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
    }
}
