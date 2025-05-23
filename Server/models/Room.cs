using System.Collections.Generic;
using Newtonsoft.Json;
using Server.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Message = Server.Core.Message;

namespace AuctionServer
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int MinParticipant { get; set; }
        public string TimeCreated { get; set; }
        public List<Item> Items { get; set; }
        public bool isOpen { get; set; }
        public string Chat { get; set; }
        public List<User> Users { get; set; }
        public List<Chat> Chats { get; set; }
        private CancellationTokenSource _countdownTokenSource;
        private Task _countdownTask;

        public static List<Room> Rooms = new List<Room>();

        public Room(int id, string name, int ownerId, string ownerName, int minParticipant, string timeCreated, string itemsJson, bool isOpen, string chat)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            OwnerName = ownerName;
            MinParticipant = minParticipant;
            TimeCreated = timeCreated;
            Items = JsonConvert.DeserializeObject<List<Item>>(itemsJson);
            this.isOpen = isOpen;
            Chat = chat;
            Users = new List<User>();
            Chats = new List<Chat>();
            StartCountdown();
        }

        public static Room GetRoom(int roomId)
        {
            return Rooms.FirstOrDefault(r => r.Id == roomId);
        }

        private void StartCountdown()
        {
            _countdownTokenSource = new CancellationTokenSource();
            _countdownTask = Task.Run(async () =>
            {
                while (!_countdownTokenSource.Token.IsCancellationRequested)
                {
                    var currentItem = Items.FirstOrDefault(i => !i.IsSold);
                    if (currentItem == null)
                    {
                        // Không còn item nào để đấu giá
                        // CloseRoom();
                        break;
                    }

                    if (currentItem.TimeLeft > 0)
                    {
                        currentItem.TimeLeft -= 1000; // Giảm 1 giây (1000ms)
                    }

                    await Task.Delay(1000, _countdownTokenSource.Token); // Đợi 1 giây
                }
            }, _countdownTokenSource.Token);
        }

        // private void HandleAuctionEnd(Item item)
        // {
        //     if (item.LatestBidderId > 0)
        //     {
        //         // Có người đấu giá thành công
        //         item.IsSold = true;
        //         var successMsg = new Message(CommandType.AuctionEnd);
        //         successMsg.WriteInt(Id);
        //         successMsg.WriteInt(item.Id);
        //         successMsg.WriteBoolean(true); // Đấu giá thành công
        //         successMsg.WriteInt(item.LatestBidderId);
        //         successMsg.WriteUTF(item.LatestBidderName);
        //         successMsg.WriteDouble(item.LatestBidPrice);
        //         SendToAllUsers(successMsg);
        //     }
        //     else
        //     {
        //         // Không có người đấu giá
        //         item.IsSold = true;
        //         var failMsg = new Message(CommandType.AuctionEnd);
        //         failMsg.WriteInt(Id);
        //         failMsg.WriteInt(item.Id);
        //         failMsg.WriteBoolean(false); // Đấu giá thất bại
        //         SendToAllUsers(failMsg);
        //     }

        //     // Cập nhật trạng thái phòng trong database
        //     try
        //     {
        //         string stmt = "UPDATE rooms SET items = @param0 WHERE id = @param1";
        //         string itemsJson = JsonConvert.SerializeObject(Items);
        //         Database.gI().ExecuteNonQuery(stmt, itemsJson, Id);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}");
        //     }
        // }

        // private void CloseRoom()
        // {
        //     isOpen = false;
        //     _countdownTokenSource?.Cancel();

        //     // Gửi thông báo đóng phòng cho tất cả user
        //     var closeMsg = new Message(CommandType.RoomClosed);
        //     closeMsg.WriteInt(Id);
        //     SendToAllUsers(closeMsg);

        //     // Kick tất cả user
        //     foreach (var user in Users.ToList())
        //     {
        //         var kickMsg = new Message(CommandType.KickedFromRoom);
        //         kickMsg.WriteInt(Id);
        //         user.Session?.SendMessage(kickMsg);
        //         Users.Remove(user);
        //     }

        //     // Cập nhật trạng thái phòng trong database
        //     try
        //     {
        //         string stmt = "UPDATE rooms SET is_open = 0 WHERE id = @param0";
        //         Database.gI().ExecuteNonQuery(stmt, Id);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}");
        //     }

        //     // Xóa phòng khỏi danh sách
        //     Rooms.Remove(this);
        // }

        private void SendToAllUsers(Message message)
        {
            foreach (var user in Users)
            {
                user.Session?.SendMessage(message);
            }
        }
    }
}
