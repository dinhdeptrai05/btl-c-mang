using System;

namespace AuctionServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Auction Server";
            Console.WriteLine("Server đấu giá đang khởi động...");

            Server server = new Server("127.0.0.1", 8000);
            server.Start();

            Console.WriteLine("Server đã khởi động. Nhấn Enter để tắt server.");
            Console.ReadLine();

            server.Stop();
            Console.WriteLine("Server đã dừng.");
        }
    }
}