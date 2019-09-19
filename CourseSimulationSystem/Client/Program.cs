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
        static void Main(string[] args)
        {

            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("192.168.1.90"), 0));
            tcpClient.Connect(IPAddress.Parse("192.168.1.90"), 6000);
            var networkStream = tcpClient.GetStream();

            Console.WriteLine("Bienvenido a Aulas");
            Console.WriteLine("Continue para iniciar sesión");
            Console.ReadLine();

            Login(networkStream);

            networkStream.Close();
            tcpClient.Dispose();
            Console.ReadLine();
        }

        private static void Login(NetworkStream networkStream)
        {
            Console.WriteLine("Ingrese su número de usuario:");
            var userNum = Console.ReadLine();
            Console.WriteLine("Ingrese su contraseña:");
            var userPassword = Console.ReadLine();

            SendMessage(networkStream, userNum);

            SendMessage(networkStream, userPassword);

        }

        private static void SendMessage(NetworkStream networkStream, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var messageLength = messageBytes.Length;
            var messageLengthBytes = BitConverter.GetBytes(messageLength);

            networkStream.Write(messageLengthBytes, 0, messageLengthBytes.Length);
            networkStream.Write(messageBytes, 0, messageBytes.Length);
        }

    }
}
