using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class Message
    {

        private static void ReadDataFromStream(int length, NetworkStream networkStream, byte[] dataBytes)
        {
            int totalReceivedData = 0;

            while (totalReceivedData < length)
            {
                var received = networkStream.Read(dataBytes, totalReceivedData, length - totalReceivedData);
                if (received == 0)
                {
                    throw new Exception("Error al recibir datos");
                }
                totalReceivedData += received;
            }

        }

        public static ProtocolPackage ReceiveMessage(NetworkStream networkStream)
        {
            try
            {
                //header
                var headerBytes = new byte[3];
                ReadDataFromStream(3, networkStream, headerBytes);
                var header = Encoding.UTF8.GetString(headerBytes);

                //cmd
                var cmdBytes = new byte[4];
                ReadDataFromStream(4, networkStream, cmdBytes);
                var cmd = BitConverter.ToInt32(cmdBytes, 0);

                //lenght
                var lenghtBytes = new byte[4];
                ReadDataFromStream(4, networkStream, lenghtBytes);
                var lenght = BitConverter.ToInt32(lenghtBytes, 0);

                //data
                var dataBytes = new byte[lenght];
                ReadDataFromStream(lenght, networkStream, dataBytes);
                var data = Encoding.UTF8.GetString(dataBytes);

                return new ProtocolPackage(header, cmd, lenght, data);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void SendMessage(NetworkStream networkStream, string header, int cmd, string data)
        {
            //header
            var headerBytes = Encoding.UTF8.GetBytes(header);
            networkStream.Write(headerBytes, 0, headerBytes.Length);

            //cmd
            var cmdBytes = BitConverter.GetBytes(cmd);
            networkStream.Write(cmdBytes, 0, cmdBytes.Length);

            //lenght - data
            var messageBytes = Encoding.UTF8.GetBytes(data);
            var messageLength = messageBytes.Length;
            var messageLengthBytes = BitConverter.GetBytes(messageLength);

            //lenght
            networkStream.Write(messageLengthBytes, 0, messageLengthBytes.Length);

            //data
            networkStream.Write(messageBytes, 0, messageBytes.Length);
        }

        public static string Serialize(List<String> StringsToSerialize)
        {
            String stringSerialized = "";
            foreach (string item in StringsToSerialize)
            {
                stringSerialized += item + ";";
            }
            return stringSerialized;
        }
        public static string[] Deserialize(String StringsToDeserialize)
        {
            return StringsToDeserialize.Split(';');
        }
    }
}
