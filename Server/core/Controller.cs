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
                    case CommandType.Register:
                        HandleRegister(message, session);
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

        private static void HandleRegister(Message message, ClientSession session)
        {
            string name = message.ReadUTF();
            string username = message.ReadUTF();
            string password = message.ReadUTF();

            string content = "";

            Console.WriteLine($"Nhận được yêu cầu đăng ký từ {username} với mật khẩu {password}");

            bool success = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(name);

            try
            {
                string query = "SELECT COUNT(*) FROM accounts WHERE username = @param0";
                DataTable result = database.ExecuteQuery(query, username);

                if (result.Rows.Count > 0)
                {
                    int count = Convert.ToInt32(result.Rows[0][0]);
                    if (count > 0)
                    {
                        success = false;
                        content = "Tên tài khoản đã tồn tại.";
                        Console.WriteLine("Tên tài khoản đã tồn tại.");
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                content = $"Lỗi khi kiểm tra tên tài khoản: {ex.Message}";
                Console.WriteLine($"Lỗi khi kiểm tra tên tài khoản: {ex.Message}");
            }

            if (success)
            {
                try
                {
                    string query = "INSERT INTO accounts (username, password, name) VALUES (@param0, @param1, @param2)";
                    int rowsAffected = database.ExecuteNonQuery(query, username, password, name);

                    if (rowsAffected > 0)
                    {
                        success = true;
                        Console.WriteLine($"Đăng ký thành công cho tài khoản {username}");
                    }
                    else
                    {
                        success = false;
                        Console.WriteLine("Không thể thêm tài khoản vào cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    Console.WriteLine($"Lỗi khi xử lý đăng ký: {ex.Message}");
                }
            }

            var response = new Message(CommandType.RegisterResponse);
            response.WriteBoolean(success);

            if (success)
            {
                response.WriteUTF(username);
            }
            else
            {
                response.WriteUTF(content);
            }

            session.SendMessage(response);
        }

        private static void HandleLogin(Message message, ClientSession session)
        {
            string username = message.ReadUTF();
            string password = message.ReadUTF();

            Console.WriteLine($"Nhận được yêu cầu đăng nhập từ {username} với mật khẩu {password}");

            bool success = false;
            int userId = -1;
            string name = null;

            try
            {
                string query = "SELECT id, name FROM accounts WHERE username = @param0 AND password = @param1";
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
                    Console.WriteLine("Không tìm thấy người dùng trong cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"Lỗi khi xử lý đăng nhập: {ex.Message}");
            }

            var response = new Message(CommandType.LoginResponse);
            response.WriteBoolean(success);

            if (success)
            {
                response.WriteInt(userId);
                response.WriteUTF(username);
                response.WriteUTF(name);
            }
            else
            {
                response.WriteUTF("Tài khoản hoặc mật khẩu không đúng.");
            }

            session.SendMessage(response);
        }
    }
}