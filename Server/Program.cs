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
            try
            {
                var database = Database.gI();
                database.TestConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return;
            }
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            Console.WriteLine($"Server started on {listener.LocalEndpoint}...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine($"Client connected from {client.Client.RemoteEndPoint}");

                var session = new ClientSession(client);
                session.Start();
            }

        }
    }
}