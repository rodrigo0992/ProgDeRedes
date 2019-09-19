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

            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("172.29.0.238"), 0));
            tcpClient.Connect(IPAddress.Parse("172.29.0.238"), 6000);
            var networkStream = tcpClient.GetStream();

            tcpListener = new TcpListener(IPAddress.Parse("172.29.0.238"), 0);
            tcpListener.Start(100);

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
            var studentNum = Console.ReadLine();
            Console.WriteLine("Ingrese su contraseña:");
            var studentPassword = Console.ReadLine();

            var data = @"{studentNum:'" + studentNum + "', password:'" + studentPassword + "'}";

            Message.SendMessage(networkStream, "REQ", 01, data);

            var tcpClient = tcpListener.AcceptTcpClient();
            var networkStreamResponse = tcpClient.GetStream();
            var protocolPackageResponse = Message.ReceiveMessage(networkStreamResponse);


            Console.WriteLine(protocolPackageResponse.Data);

        }



    }
}
