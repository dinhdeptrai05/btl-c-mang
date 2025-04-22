using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Message = Server.Core.Message;
using Server.Core;

namespace AuctionServer
{
    public class Server
    {
        private TcpListener _listener;
        private bool _isRunning;
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly List<ClientSession> _clients = new List<ClientSession>();
        private readonly Dictionary<int, Auction> _auctions = new Dictionary<int, Auction>();
        private int _nextAuctionId = 1;
        private readonly MessageController _messageController;

        public Server(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _messageController = new MessageController(this);
        }

        public void Start()
        {
            _isRunning = true;
            _listener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            _listener.Start();

            Console.WriteLine($"Server đang lắng nghe tại {_ipAddress}:{_port}");

            Task.Run(AcceptClientsAsync);

            // Tạo một vài đấu giá mẫu để test
            CreateSampleAuctions();
        }

        private void CreateSampleAuctions()
        {
            CreateAuction("iPhone 14 Pro", "Điện thoại iPhone 14 Pro mới 100%", 20000000, DateTime.Now.AddHours(24));
            CreateAuction("Laptop Dell XPS 13", "Laptop Dell XPS 13 9310, i7 gen 11, 16GB RAM", 25000000, DateTime.Now.AddHours(12));
            CreateAuction("Sony PlayStation 5", "Máy chơi game PS5 Digital Edition", 12000000, DateTime.Now.AddHours(6));
        }

        public int CreateAuction(string name, string description, decimal startingPrice, DateTime endTime)
        {
            var auction = new Auction
            {
                Id = _nextAuctionId++,
                Name = name,
                Description = description,
                StartingPrice = startingPrice,
                CurrentPrice = startingPrice,
                EndTime = endTime,
                IsActive = true
            };

            _auctions.Add(auction.Id, auction);

            // Thông báo cho tất cả client về đấu giá mới
            BroadcastNewAuction(auction);

            return auction.Id;
        }

        private async Task AcceptClientsAsync()
        {
            try
            {
                while (_isRunning)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Client mới kết nối: {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                    var clientSession = new ClientSession(client, this, _messageController);
                    lock (_clients)
                    {
                        _clients.Add(clientSession);
                    }

                    // Khởi chạy session trong Task riêng
                    _ = Task.Run(() => clientSession.StartAsync());
                }
            }
            catch (Exception ex) when (_isRunning)
            {
                Console.WriteLine($"Lỗi khi chấp nhận client: {ex.Message}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();

            // Đóng tất cả kết nối client
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    client.Close();
                }
                _clients.Clear();
            }
        }

        public void RemoveClient(ClientSession clientSession)
        {
            lock (_clients)
            {
                _clients.Remove(clientSession);
                Console.WriteLine($"Client đã ngắt kết nối: {((IPEndPoint)clientSession.TcpClient.Client.RemoteEndPoint).Address}");
            }
        }

        public void BroadcastMessage(Message message, ClientSession excludeClient = null)
        {
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    if (client != excludeClient && client.IsConnected)
                    {
                        client.SendMessage(message);
                    }
                }
            }
        }

        public void BroadcastNewAuction(Auction auction)
        {
            var message = new Message(CommandType.NewAuction);
            message.WriteInt(auction.Id);
            message.WriteUTF(auction.Name);
            message.WriteUTF(auction.Description);
            message.WriteDouble((double)auction.CurrentPrice);
            message.WriteLong(auction.EndTime.Ticks);

            BroadcastMessage(message);
        }

        public void BroadcastAuctionUpdate(Auction auction)
        {
            var message = new Message(CommandType.AuctionUpdate);
            message.WriteInt(auction.Id);
            message.WriteDouble((double)auction.CurrentPrice);
            message.WriteInt(auction.HighestBidderId);
            message.WriteUTF(auction.HighestBidderName ?? "");

            BroadcastMessage(message);
        }

        public Dictionary<int, Auction> GetAllAuctions()
        {
            return _auctions;
        }

        public bool PlaceBid(int auctionId, int userId, string username, decimal bidAmount)
        {
            if (!_auctions.TryGetValue(auctionId, out var auction))
                return false;

            if (!auction.IsActive || DateTime.Now > auction.EndTime)
                return false;

            if (bidAmount <= auction.CurrentPrice)
                return false;

            auction.CurrentPrice = bidAmount;
            auction.HighestBidderId = userId;
            auction.HighestBidderName = username;

            // Thông báo cho tất cả client về cập nhật đấu giá
            BroadcastAuctionUpdate(auction);

            return true;
        }
    }
}