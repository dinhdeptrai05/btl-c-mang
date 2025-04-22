using System;
using Server.Core;

namespace AuctionServer
{
    public class MessageController
    {
        private readonly Server _server;

        public MessageController(Server server)
        {
            _server = server;
        }

        public void HandleMessage(Message message, ClientSession session)
        {
            try
            {
                // Đọc lại command ID từ đầu message
                sbyte commandId = message.ReadSByte();

                switch (commandId)
                {
                    case CommandType.Login:
                        HandleLogin(message, session);
                        break;
                    case CommandType.GetAllAuctions:
                        HandleGetAllAuctions(message, session);
                        break;
                    case CommandType.PlaceBid:
                        HandlePlaceBid(message, session);
                        break;
                    case CommandType.CreateAuction:
                        HandleCreateAuction(message, session);
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

        private void HandleLogin(Message message, ClientSession session)
        {
            string username = message.ReadUTF();
            string password = message.ReadUTF();

            // Xác thực đơn giản (trong thực tế sẽ kiểm tra với database)
            bool success = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            int userId = 0;

            if (success)
            {
                // Giả sử mỗi username có một ID (trong thực tế sẽ lấy từ DB)
                userId = username.GetHashCode();
                session.UserId = userId;
                session.Username = username;
                Console.WriteLine($"User {username} đã đăng nhập (ID: {userId})");
            }

            var response = new Message(CommandType.LoginResponse);
            response.WriteBoolean(success);
            
            if (success)
            {
                response.WriteInt(userId);
                response.WriteUTF(username);
            }

            session.SendMessage(response);
        }

        private void HandleGetAllAuctions(Message message, ClientSession session)
        {
            var auctions = _server.GetAllAuctions();
            
            var response = new Message(CommandType.AllAuctionsResponse);
            response.WriteInt(auctions.Count);

            foreach (var auction in auctions.Values)
            {
                response.WriteInt(auction.Id);
                response.WriteUTF(auction.Name);
                response.WriteUTF(auction.Description);
                response.WriteDouble((double)auction.CurrentPrice);
                response.WriteDouble((double)auction.StartingPrice);
                response.WriteLong(auction.EndTime.Ticks);
                response.WriteBoolean(auction.IsActive);
                response.WriteInt(auction.HighestBidderId);
                response.WriteUTF(auction.HighestBidderName ?? "");
            }

            session.SendMessage(response);
        }

        private void HandlePlaceBid(Message message, ClientSession session)
        {
            // if (session.UserId == 0)
            // {
            //     // Người dùng chưa đăng nhập
            //     var response = new Message(CommandType.BidResponse);
            //     response.WriteBoolean(false);
            //     response.WriteUTF("Vui lòng đăng nhập để đặt giá!");
            //     session.SendMessage(response);
            //     return;
            // }

            int auctionId = message.ReadInt();
            decimal bidAmount = (decimal)message.ReadDouble();

            bool success = _server.PlaceBid(auctionId, session.UserId, session.Username, bidAmount);

            var response = new Message(CommandType.BidResponse);
            response.WriteBoolean(success);
            
            if (success)
                response.WriteUTF("Đặt giá thành công!");
            else
                response.WriteUTF("Đặt giá thất bại. Giá đặt phải cao hơn giá hiện tại!");

            session.SendMessage(response);
        }

        private void HandleCreateAuction(Message message, ClientSession session)
        {
            if (session.UserId == 0)
            {
                // // Người dùng chưa đăng nhập
                // var response = new Message(CommandType.CreateAuctionResponse);
                // response.WriteBoolean(false);
                // response.WriteUTF("Vui lòng đăng nhập để tạo đấu giá!");
                // session.SendMessage(response);
                // return;
            }

            string name = message.ReadUTF();
            string description = message.ReadUTF();
            decimal startingPrice = (decimal)message.ReadDouble();
            long endTimeTicks = message.ReadLong();
            DateTime endTime = new DateTime(endTimeTicks);

            // Kiểm tra thông tin đấu giá
            if (string.IsNullOrEmpty(name) || startingPrice <= 0 || endTime <= DateTime.Now)
            {
                // var response = new Message(CommandType.CreateAuctionResponse);
                // response.WriteBoolean(false);
                // response.WriteUTF("Thông tin đấu giá không hợp lệ!");
                // session.SendMessage(response);
                // return;
            }

            int auctionId = _server.CreateAuction(name, description, startingPrice, endTime);

            var response = new Message(CommandType.CreateAuctionResponse);
            response.WriteBoolean(true);
            response.WriteInt(auctionId);
            response.WriteUTF("Tạo đấu giá thành công!");

            session.SendMessage(response);
        }
    }
}