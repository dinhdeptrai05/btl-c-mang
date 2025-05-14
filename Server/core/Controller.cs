using System;
using Server.Core;
using System.Net.Sockets;
using System.Data;
using Google.Protobuf.WellKnownTypes;
using System.Net.Http;
using System.Threading.Tasks;
using Util;
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
                    case CommandType.JoinRoom:
                        HandleJoinRoom(message, session);
                        break;
                    case CommandType.SendChatMessage:
                        HandleSendChatMessage(message);
                        break;
                    case CommandType.LeaveRoom:
                        HandleLeaveRoom(message, session);
                        break;
                    case CommandType.CreateRoom:
                        HandleCreateRoom(message, session);
                        break;
                    case CommandType.Logout:
                        HandleLogout(message, session);
                        break;
                    case CommandType.UpdateProfile:
                        HandleUpdateProfile(message, session);
                        break;
                    // case CommandType.AddItemToRoom:
                    //     HandleAddItemToRoom(message, session);
                    //     break;
                    // case CommandType.StartAuction:
                    //     HandleStartAuction(message, session);
                    //     break;
                    // case CommandType.BuyNow:
                    //     HandleBuyNow(message, session);
                    //     break;
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

        private static async void HandleUpdateProfile(Message message, ClientSession session)
        {
            string name = message.ReadUTF();
            string username = message.ReadUTF();
            string password = message.ReadUTF();
            byte[] imageBytes = message.ReadFile();
            string base64Image = Convert.ToBase64String(imageBytes);

            string avatar_url = null;
            if (!string.IsNullOrEmpty(base64Image))
            {
                avatar_url = await Utils.UploadImageToImgBB(base64Image);
            }

            try
            {
                string query = "UPDATE accounts SET name = @param0, avatar_url = @param1 WHERE id = @param2";
                database.ExecuteNonQuery(query, name, avatar_url ?? session.GetCurrentUser().Avatar, session.GetCurrentUser().Id);

                // Cập nhật thông tin user trong bộ nhớ
                session.GetCurrentUser().Name = name;
                if (avatar_url != null)
                {
                    session.GetCurrentUser().Avatar = avatar_url;
                }

                var response = new Message(CommandType.UpdateProfileResponse);
                response.WriteBoolean(true);
                response.WriteUTF(name);
                response.WriteUTF(avatar_url);
                session.SendMessage(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật profile: {ex.Message}");
                var response = new Message(CommandType.UpdateProfileResponse);
                response.WriteBoolean(false);
                response.WriteUTF("Lỗi khi cập nhật profile");
                session.SendMessage(response);
            }
        }
        private static void HandleLogout(Message message, ClientSession session)
        {
            Console.WriteLine($"Người dùng {session.GetCurrentUser().Username} đã đăng xuất");
            ClientSession.users.Remove(session.GetCurrentUser());
            // session.Disconnect();
        }
        private static void HandleCreateRoom(Message message, ClientSession session)
        {
            string roomName = message.ReadUTF();
            int minParticipant = message.ReadInt();
            int ownerId = message.ReadInt();
            int itemCount = message.ReadInt();
            List<Item> items = new List<Item>();
            for (int i = 0; i < itemCount; i++)
            {
                int itemId = message.ReadInt();
                string itemName = message.ReadUTF();
                string itemDescription = message.ReadUTF();
                string itemImageUrl = message.ReadUTF();
                double itemStartingPrice = message.ReadDouble();
                double itemBuyNowPrice = message.ReadDouble();
                string itemEndTime = message.ReadUTF();
                items.Add(new Item(itemId, itemName, itemDescription, itemImageUrl, itemStartingPrice, itemBuyNowPrice, 0, 0, false, DateTime.Parse(itemEndTime)));
            }
            try
            {
                string query = "INSERT INTO rooms (name, min_participant, owner_id, items) VALUES (@param0, @param1, @param2, @param3); SELECT LAST_INSERT_ID() as id;";
                DataTable result = database.ExecuteQuery(query, roomName, minParticipant, ownerId, Newtonsoft.Json.JsonConvert.SerializeObject(items));
                int roomId = Convert.ToInt32(result.Rows[0]["id"]);

                Room room = new Room(roomId, roomName, ownerId, minParticipant, DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), Newtonsoft.Json.JsonConvert.SerializeObject(items), true, "");
                Room.Rooms.Add(room);
                var response = new Message(CommandType.CreateRoomResponse);
                response.WriteBoolean(true);
                response.WriteInt(roomId);
                session.SendMessage(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo phòng đấu giá: {ex.Message}");
            }
        }

        private static void HandleSendChatMessage(Message message)
        {
            int roomId = message.ReadInt();
            string time = message.ReadUTF();
            string name = message.ReadUTF();
            string msg = message.ReadUTF();

            try
            {
                string selectQUERY = "SELECT chat from rooms where id = @param0";
                DataTable result = database.ExecuteQuery(selectQUERY, roomId);
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    string chatJson = row["chat"].ToString();
                    var chats = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Chat>>(chatJson);
                    chats.Add(new Chat(time, name, msg));
                    string newChat = Newtonsoft.Json.JsonConvert.SerializeObject(chats);
                    string updateQUERY = "UPDATE rooms SET chat = @param0 where id = @param1";
                    database.ExecuteNonQuery(updateQUERY, newChat, roomId);
                    Room room = Room.GetRoom(roomId);
                    room.Chats.Add(new Chat(time, name, msg));
                    var response = new Message(CommandType.ChatMessageReceived);
                    response.WriteSByte(0); // 0 là message thường
                    response.WriteInt(roomId);
                    response.WriteUTF(time);
                    response.WriteUTF(name);
                    response.WriteUTF(msg);
                    ClientSession.SendToAll(response);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private static void HandleJoinRoom(Message message, ClientSession session)
        {
            int roomId = message.ReadInt();
            try
            {
                Room room = Room.GetRoom(roomId);
                if (room != null)
                {
                    room.Users.Add(session.GetCurrentUser());
                    var response = new Message(CommandType.JoinRoomResponse);
                    response.WriteBoolean(true); // Tham gia phòng thành công

                    response.WriteInt(roomId);
                    response.WriteUTF(room.Name);
                    response.WriteInt(room.OwnerId);
                    response.WriteBoolean(room.isOpen);
                    response.WriteInt(room.Items.Count);

                    foreach (Item item in room.Items)
                    {
                        response.WriteInt(item.Id);
                        response.WriteInt(item.LatestBidderId);
                        response.WriteUTF(item.Name);
                        response.WriteUTF(item.Description);
                        response.WriteUTF(item.ImageUrl);
                        response.WriteDouble(item.StartingPrice);
                        response.WriteDouble(item.BuyNowPrice);
                        response.WriteDouble(item.LatestBidPrice);
                        response.WriteBoolean(item.IsSold);
                        response.WriteUTF(item.EndTime.ToString("HH:mm:ss dd/MM/yyyy"));
                    }
                    response.WriteInt(room.Chats.Count);
                    foreach (var chat in room.Chats)
                    {
                        response.WriteUTF(chat.time);
                        response.WriteUTF(chat.name);
                        response.WriteUTF(chat.message);
                    }
                    // Gửi thông báo người dùng tham gia phòng
                    Message joinNotification = new Message(CommandType.UserJoinRoom);
                    joinNotification.WriteSByte(1); // 1 là subcommand join room
                    joinNotification.WriteInt(roomId);
                    joinNotification.WriteUTF((string)DateTime.Now.ToString("HH:mm:ss"));
                    joinNotification.WriteUTF(session.GetCurrentUser().Name);
                    ClientSession.SendToAll(joinNotification);

                    // Gửi response về client
                    session.SendMessage(response);

                }
                else
                {
                    // Không tìm thấy phòng
                    Console.WriteLine("Không tìm thấy phòng đấu giá với ID đã cho.");
                    var response = new Message(CommandType.JoinRoomResponse);
                    response.WriteBoolean(false); // Tham gia phòng thất bại
                    response.WriteUTF("Không tìm thấy phòng đấu giá.");
                    session.SendMessage(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tham gia phòng đấu giá: {ex.Message}");
                var response = new Message(CommandType.JoinRoomResponse);
                response.WriteBoolean(false); // Tham gia phòng thất bại
                response.WriteUTF($"Lỗi khi tham gia phòng đấu giá: {ex.Message}");
                session.SendMessage(response);
            }
        }

        private static void HandleLeaveRoom(Message message, ClientSession session)
        {
            int roomId = message.ReadInt();
            Room room = Room.Rooms.FirstOrDefault(r => r.Id == roomId);
            room.Users.Remove(session.GetCurrentUser());
            // Gửi thông báo người dùng thoát phòng
            Message leaveNotification = new Message(CommandType.UserLeaveRoom);
            leaveNotification.WriteSByte(-1); // -1 là sub command thoát phòng
            leaveNotification.WriteInt(roomId);
            leaveNotification.WriteUTF((string)DateTime.Now.ToString("HH:mm:ss"));
            leaveNotification.WriteUTF(session.GetCurrentUser().Name);
            ClientSession.SendToAll(leaveNotification);
        }

        private static void HandleGetAllRoom(Message message, ClientSession session)
        {
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
                    response.WriteBoolean(Convert.ToBoolean(row["is_open"]));
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
                    User user = new User(userId, username, password, name, avatar_url);
                    user.Session = session;
                    ClientSession.users.Add(user);
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