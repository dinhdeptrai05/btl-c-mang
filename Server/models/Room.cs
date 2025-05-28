using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Core;

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
        public DateTime AuctionStartTime { get; set; }
        public bool isStarted { get; set; }
        public List<Chat> Chats { get; set; }
        private CancellationTokenSource _countdownTokenSource;
        private Task _countdownTask;

        public static List<Room> Rooms = new List<Room>();

        public Room(int id, string name, int ownerId, string ownerName, int minParticipant, string timeCreated, string itemsJson, bool isOpen, string chat, bool is_started, DateTime auctionStartTime)
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
            AuctionStartTime = auctionStartTime;
            isStarted = is_started;
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
                    CheckAuctionTimeStart();

                    if (isStarted)
                    {
                        var currentItem = Items.FirstOrDefault(i => !i.IsSold);
                        if (currentItem == null)
                        {
                            // Không còn item nào để đấu giá
                            CloseRoom();
                            break;
                        }

                        if (currentItem.TimeLeft > 0)
                        {
                            currentItem.TimeLeft -= 1000;
                        }
                        else
                        {
                            HandleAuctionEnd(currentItem);
                            break;
                        }
                    }
                    await Task.Delay(1000, _countdownTokenSource.Token);
                }
            }, _countdownTokenSource.Token);
        }

        private void StartAuction()
        {
            isStarted = true;
            var startMsg = new Message(CommandType.AuctionStarted);
            startMsg.WriteInt(Id);
            ClientSession.SendToAll(startMsg);
        }

        private void CheckAuctionTimeStart()
        {
            if (DateTime.Now >= AuctionStartTime && !isStarted)
            {
                StartAuction();
            }
        }

        private void HandleAuctionEnd(Item item)
        {
            if (!String.IsNullOrEmpty(item.LatestBidderName))
            {
                // Có người đấu giá thành công
                item.IsSold = true;
                var successMsg = new Message(CommandType.AuctionEnd);
                successMsg.WriteInt(Id);
                successMsg.WriteBoolean(true); // Đấu giá thành công
                successMsg.WriteInt(item.Id);
                successMsg.WriteUTF(item.LatestBidderName);
                successMsg.WriteDouble(item.LatestBidPrice);
                ClientSession.SendToAll(successMsg);
            }
            else
            {
                // Không có người đấu giá
                item.IsSold = true;
                var failMsg = new Message(CommandType.AuctionEnd);
                failMsg.WriteInt(Id);
                failMsg.WriteBoolean(false); // Đấu giá thất bại
                failMsg.WriteInt(item.Id);
                ClientSession.SendToAll(failMsg);
            }

            // Cập nhật trạng thái phòng trong database
            try
            {
                string stmt = "UPDATE rooms SET items = @param0 WHERE id = @param1";
                string itemsJson = JsonConvert.SerializeObject(Items);
                Database.gI().ExecuteNonQuery(stmt, itemsJson, Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}");
            }
        }

        private void CloseRoom()
        {
            isOpen = false;
            _countdownTokenSource?.Cancel();

            // Gửi thông báo đóng phòng cho tất cả user
            var closeMsg = new Message(CommandType.RoomClosed);
            closeMsg.WriteInt(Id);
            ClientSession.SendToAll(closeMsg);

            // Kick tất cả user
            foreach (var user in Users.ToList())
            {
                var kickMsg = new Message(CommandType.KickedFromRoom);
                kickMsg.WriteInt(Id);
                user.Session?.SendMessage(kickMsg);
                Users.Remove(user);
            }

            // Cập nhật trạng thái phòng trong database
            try
            {
                string stmt = "UPDATE rooms SET is_open = 0 WHERE id = @param0";
                Database.gI().ExecuteNonQuery(stmt, Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}");
            }

            // Xóa phòng khỏi danh sách
            Rooms.Remove(this);
        }
    }
}