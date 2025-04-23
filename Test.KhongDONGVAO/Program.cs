using System;
using System.Threading.Tasks;
using Client.Core;
using Client.enums;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Client đấu giá trực tuyến");
            Console.WriteLine("-------------------------");

            try
            {
                var client = AuctionClient.gI("127.0.0.1", 8000);

                while (true)
                {
                    if (client._isConnected)
                    {
                        Console.Write("Nhập tên đăng nhập: ");
                        string username = Console.ReadLine();

                        Console.Write("Nhập mật khẩu: ");
                        string password = Console.ReadLine();

                        var loginMessage = new Message(CommandType.Login);
                        loginMessage.WriteUTF(username);
                        loginMessage.WriteUTF(password);

                        AuctionClient.SendMessage(loginMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.WriteLine("Nhấn phím bất kỳ để thoát...");
                Console.ReadKey();
            }
        }
    }
}
