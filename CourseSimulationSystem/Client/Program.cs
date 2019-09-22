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
            ClientActions clientActions= new ClientActions();
            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("192.168.1.61"), 0));
            tcpClient.Connect(IPAddress.Parse("192.168.1.61"), 6000);
            var networkStream = tcpClient.GetStream();

            Console.WriteLine("Bienvenido a Aulas");
            Console.WriteLine("Continue para iniciar sesión");
            Console.ReadLine();
            /*var loggedIn = false;
            while (!loggedIn)
            {
                loggedIn = clientActions.Login(networkStream);
            }*/
            Menu(networkStream, clientActions);

            networkStream.Close();
            tcpClient.Dispose();
            Console.ReadLine();
        }

        private static void Menu(NetworkStream networkStream, ClientActions clientActions)
        {
            Console.WriteLine("Seleccione una opcion:");
            Console.WriteLine("1- Alta de curso");
            var opcion = Console.ReadLine();

            switch (Convert.ToInt32(opcion))
            {
                case 1:
                    clientActions.AddStudentToCourse(networkStream);
                break;
                
            }
        }



    }
}
