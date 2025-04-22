using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Forms;
using Client.Core;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Auction Client";
            Console.WriteLine("Client đấu giá trực tuyến");
            Console.WriteLine("-------------------------");

            try
            {
                var client = new AuctionClient("127.0.0.1", 8000);
                await client.ConnectAsync();

                Console.WriteLine("Kết nối thành công với server.");

                // Khởi chạy client UI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormLogin());
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