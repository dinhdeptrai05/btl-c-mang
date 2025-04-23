using System;
using Server.Core;
using System.Net.Sockets;

namespace AuctionServer
{
    public class Controller
    {
        public static void HandleMessage(Message message)
        {
            try
            {
                sbyte commandId = message.ReadSByte();

                switch (commandId)
                {
                    case CommandType.Login:
                        HandleLogin(message);
                        break;
                    default:
                        Console.WriteLine($"Nhận được command không xác định: {commandId}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý message: {ex.Message}");
            }
        }

        private static void HandleLogin(Message message)
        {
            string username = message.ReadUTF();
            string password = message.ReadUTF();

            Console.WriteLine($"Nhận được yêu cầu đăng nhập từ {username} với mật khẩu {password}");

            // Xác thực đơn giản (trong thực tế sẽ kiểm tra với database)
            bool success = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            int userId = -1;

            if (success)
            {
                if (username == "admin" && password == "admin")
                {
                    success = true;
                    userId = 1;
                }
                else
                {
                    success = false;
                }
            }

            var response = new Message(CommandType.LoginResponse);
            response.WriteBoolean(success);

            if (success)
            {
                response.WriteInt(userId);
                response.WriteUTF(username);
            }

            ClientSession.SendMessage(response);
        }
    }
}