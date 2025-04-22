using System;
using Server.Core;
using System.Net.Sockets;

namespace AuctionServer
{
    public class Controller
    {
        public static void HandleMessage(Message message, NetworkStream stream)
        {
            try
            {
                sbyte commandId = message.ReadSByte();

                switch (commandId)
                {
                    case CommandType.Login:
                        HandleLogin(message, stream);
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

        private static void HandleLogin(Message message, NetworkStream stream)
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

            SendMessage(stream, response);
        }

        private static void SendMessage(NetworkStream stream, Message message)
        {
            try
            {
                byte[] data = message.GetBytes();
                byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

                stream.Write(lengthPrefix, 0, 4);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gửi lại message cho client: " + ex.Message);
            }
        }
    }
}