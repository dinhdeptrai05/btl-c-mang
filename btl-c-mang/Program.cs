using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Core;
using Client.Forms.Login;

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
                var client = AuctionClient.gI("18.167.221.122", 8000);

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