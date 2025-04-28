using System;
using Server.Core;
using System.Net.Sockets;
using System.Data;

namespace AuctionServer
{
    public class Controller
    {
        private static Database database = Database.gI();
        public static void HandleMessage(Message message, ClientSession session)
        {
            try
            {
                sbyte commandId = message.ReadSByte();

                switch (commandId)
                {
                    case CommandType.Login:
                        HandleLogin(message, session);
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

        private static void HandleLogin(Message message, ClientSession session)
        {
            string username = message.ReadUTF();
            string password = message.ReadUTF();

            Console.WriteLine($"Nhận được yêu cầu đăng nhập từ {username} với mật khẩu {password}");

            // Xác thực đơn giản (trong thực tế sẽ kiểm tra với database)
            bool success = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            int userId = -1;
            string name = null;

            if (success)
            {
                try
                {
                    string query = "SELECT * FROM accounts WHERE username = @param0 AND password = @param1";
                    DataTable result = database.ExecuteQuery(query, username, password);
                    if (result.Rows.Count > 0)
                    {
                        DataRow row = result.Rows[0];
                        userId = Convert.ToInt32(row["id"]);
                        name = row["name"].ToString();
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    Console.WriteLine($"Lỗi khi xử lý đăng nhập: {ex.Message}");
                }
            }

            var response = new Message(CommandType.LoginResponse);
            response.WriteBoolean(success);

            if (success)
            {
                response.WriteInt(userId);
                response.WriteUTF(username);
                response.WriteUTF(name);

            }

            response.WriteUTF("Tài khoản hoặc mật khẩu không đúng.");
            session.SendMessage(response);
        }
    }
}