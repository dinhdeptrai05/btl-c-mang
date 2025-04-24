using System;
using System.Threading.Tasks;
using Client.Core;
using Client.enums;

namespace Client
{
    class Program
    {
        private static string arr = string.Empty;

        private static async Task Menu()
        {
            Console.WriteLine("1. Nhập dãy số (phân cách bằng dấu ' ')");
            Console.WriteLine("2. Chọn phép tính (+ - * /)");
            Console.Write("Chọn: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Nhập dãy số: ");
                    arr = Console.ReadLine();
                    if (string.IsNullOrEmpty(arr))
                    {
                        Console.WriteLine("Dãy không được để trống. Vui lòng nhập lại.");
                    }
                    else if (!isValidNumberArray(arr))
                    {
                        Console.WriteLine("Dãy số không hợp lệ. Vui lòng nhập lại.");
                    }
                    break;
                case "2":
                    if (string.IsNullOrEmpty(arr))
                    {
                        Console.WriteLine("Bạn chưa nhập dãy số. Vui lòng chọn 1 để nhập dãy số trước.");
                        break;
                    }
                    Console.Write("Chọn phép tính (+, - ,* ,/): ");
                    string operation = Console.ReadLine();
                    if (string.IsNullOrEmpty(operation))
                    {
                        Console.WriteLine("Phép tính không được để trống. Vui lòng nhập lại.");
                        break;
                    }
                    var msg = new Message(1);
                    msg.WriteUTF(arr);
                    msg.WriteUTF(operation);
                    AuctionClient.SendMessage(msg);

                    await WaitForServerResponse();
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng thử lại.");
                    break;
            }
        }

        private static bool isValidNumberArray(string input)
        {
            string[] numbers = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string number in numbers)
            {
                if (!int.TryParse(number, out _))
                {
                    return false;
                }
            }
            return true;
        }

        private static async Task WaitForServerResponse()
        {
            await Task.Delay(1000);
        }

        static async Task Main(string[] args)
        {
            try
            {
                var client = AuctionClient.gI("127.0.0.1", 8000);

                while (true)
                {
                    if (client._isConnected)
                    {
                        await Menu();
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
