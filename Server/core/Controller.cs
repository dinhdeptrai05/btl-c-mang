using System;
using Server.Core;
using System.Net.Sockets;
using System.Data;
using Google.Protobuf.WellKnownTypes;

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
                    case CommandType.getAllRoom:
                        HandleGetAllRoom(message, session);
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

        private static void HandleGetAllRoom(Message message, ClientSession session)
        {
            Console.WriteLine("Nhận được yêu cầu lấy tất cả phòng đấu giá.");

            try
            {
                string query = "SELECT * FROM rooms";
                DataTable result = database.ExecuteQuery(query);

                var response = new Message(CommandType.getAllRoomResponse);
                int count = result.Rows.Count;
                response.WriteInt(count);

                for (int i = 0; i < count; i++)
                {
                    DataRow row = result.Rows[i];
                    response.WriteInt(Convert.ToInt32(row["id"]));
                    response.WriteInt(Convert.ToInt32(row["owner_id"]));
                    response.WriteInt(Convert.ToInt32(row["is_open"]));
                    response.WriteUTF(row["name"].ToString());
                    response.WriteUTF(row["time_created"].ToString());
                }

                session.SendMessage(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách phòng đấu giá: {ex.Message}");
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
            string avatar_url = null;

            string error = null;

            try
            {
                string query = "SELECT * FROM accounts WHERE username = @param0 AND password = @param1";
                DataTable result = database.ExecuteQuery(query, username, password);

                Console.WriteLine(result);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    userId = Convert.ToInt32(row["id"]);
                    name = row["name"].ToString();
                    avatar_url = row["avatar_url"].ToString();
                    if (string.IsNullOrEmpty(avatar_url))
                    {
                        avatar_url = "https://www.w3schools.com/howto/img_avatar.png";
                    }
                    success = true;
                }
                else
                {
                    success = false;
                    error = "Tài khoản hoặc mật khẩu không đúng.";
                }
            }
            catch (Exception ex)
            {
                success = false;
                error = $"Lỗi khi xử lý đăng nhập: {ex.Message}";
                Console.WriteLine($"Lỗi khi xử lý đăng nhập: {ex.Message}");
            }

            var response = new Message(CommandType.LoginResponse);
            response.WriteBoolean(success);

            if (success)
            {
                response.WriteInt(userId);
                response.WriteUTF(username);
                response.WriteUTF(name);
                response.WriteUTF(avatar_url);
            }
            else
            {
                response.WriteUTF(error);
            }

            session.SendMessage(response);
        }
    }
}