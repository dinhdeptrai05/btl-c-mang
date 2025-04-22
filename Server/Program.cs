using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Message = Server.Core.Message;
using Server.Core;

namespace AuctionServer
{

    class Server
    {
        static void Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8000);
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

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
                    Controller.HandleMessage(message, stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected: " + ex.Message);
                client.Close();
            }
        }
    }
}