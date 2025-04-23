using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Server.Core;

namespace AuctionServer
{
    public class ClientSession
    {
        private static NetworkStream _stream;

        public static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            _stream = stream;
            try
            {
                while (true)
                {
                    byte[] lengthBuffer = new byte[4];
                    stream.Read(lengthBuffer, 0, 4);
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    byte[] buffer = new byte[messageLength];
                    int bytesRead = 0;
                    while (bytesRead < messageLength)
                    {
                        bytesRead += stream.Read(buffer, bytesRead, messageLength - bytesRead);
                    }

                    // Tạo message từ buffer (đã bao gồm CommandID ở byte đầu tiên)
                    var message = new Message(buffer);

                    // Gọi handler
                    Controller.HandleMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected: " + ex.Message);
                client.Close();
            }
        }

        public static void SendMessage(Message message)
        {
            try
            {
                byte[] data = message.GetBytes();
                byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

                _stream.Write(lengthPrefix, 0, 4);
                _stream.Write(data, 0, data.Length);
                _stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gửi lại message cho client: " + ex.Message);
            }
        }
    }
}