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

                Thread clientThread = new Thread(() => ClientSession.HandleClient(client));
                clientThread.Start();
            }
        }
    }
}