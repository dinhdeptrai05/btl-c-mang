using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Client.enums;
using System.Threading.Tasks;

namespace Client.Core
{
    public class AuctionClient
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private bool _isConnected = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Dictionary<sbyte, Action<Message>> _messageHandlers = new Dictionary<sbyte, Action<Message>>();

        // Events
        public event EventHandler<string> WelcomeMessageReceived;
        public event EventHandler<LoginResponseEventArgs> LoginResponseReceived;
        public event EventHandler<AuctionsListEventArgs> AuctionsListReceived;
        public event EventHandler<BidResponseEventArgs> BidResponseReceived;
        public event EventHandler<AuctionEventArgs> NewAuctionReceived;
        public event EventHandler<AuctionUpdateEventArgs> AuctionUpdateReceived;
        // public event EventHandler<CreateAuctionResponseEventArgs> CreateAuctionResponseReceived;
        public event EventHandler Disconnected;

        // User data
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public bool IsLoggedIn => UserId > 0;
        public bool IsConnected => _isConnected;

        public AuctionClient(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;

            // Register message handlers
            _messageHandlers[CommandType.Welcome] = HandleWelcomeMessage;
            _messageHandlers[CommandType.LoginResponse] = HandleLoginResponse;
            _messageHandlers[CommandType.AllAuctionsResponse] = HandleAllAuctionsResponse;
            _messageHandlers[CommandType.BidResponse] = HandleBidResponse;
            _messageHandlers[CommandType.NewAuction] = HandleNewAuction;
            _messageHandlers[CommandType.AuctionUpdate] = HandleAuctionUpdate;
            // _messageHandlers[CommandType.CreateAuctionResponse] = HandleCreateAuctionResponse;
        }

        public async Task ConnectAsync()
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_serverIp, _serverPort);
            _stream = _tcpClient.GetStream();
            _isConnected = true;

            // Start listening for messages
            _ = Task.Run(ReceiveMessagesAsync);
        }

        public void Disconnect()
        {
            _isConnected = false;
            _cancellationTokenSource.Cancel();

            try
            {
                _stream?.Close();
                _tcpClient?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi ngắt kết nối: {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            try
            {
                byte[] buffer = new byte[4096];

                while (_isConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Read message length
                    byte[] lengthBuffer = new byte[4];
                    int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4, _cancellationTokenSource.Token);

                    if (bytesRead < 4)
                    {
                        // Connection closed
                        break;
                    }

                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    if (messageLength <= 0 || messageLength > 1024 * 1024) // Max 1MB
                    {
                        // Invalid message
                        continue;
                    }

                    // Read message content
                    byte[] messageBuffer = new byte[messageLength];
                    int bytesReadTotal = 0;

                    while (bytesReadTotal < messageLength)
                    {
                        int bytesRemaining = messageLength - bytesReadTotal;
                        int bytesReceived = await _stream.ReadAsync(messageBuffer, bytesReadTotal, bytesRemaining, _cancellationTokenSource.Token);

                        if (bytesReceived == 0)
                        {
                            // Connection closed
                            _isConnected = false;
                            break;
                        }

                        bytesReadTotal += bytesReceived;
                    }

                    if (!_isConnected)
                        break;

                    // Process message
                    using (var ms = new System.IO.MemoryStream(messageBuffer))
                    {
                        // Read command ID
                        sbyte commandId = (sbyte)ms.ReadByte();

                        // Create message from received data
                        var message = new Message(commandId);
                        message.WriteBytes(messageBuffer, 0, messageBuffer.Length);

                        // Handle message based on command ID
                        if (_messageHandlers.TryGetValue(commandId, out var handler))
                        {
                            handler(message);
                        }
                        else
                        {
                            Console.WriteLine($"Received unknown command: {commandId}");
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is SocketException || ex is System.IO.IOException)
            {
                // Connection closed
                _isConnected = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving messages: {ex.Message}");
                _isConnected = false;
            }
            finally
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task SendMessageAsync(Message message)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                // Get message data
                byte[] messageData = ((System.IO.MemoryStream)message.GetStream()).ToArray();
                int messageLength = messageData.Length;

                // Send length first
                byte[] lengthBytes = BitConverter.GetBytes(messageLength);
                await _stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                // Send message data
                await _stream.WriteAsync(messageData, 0, messageData.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                _isConnected = false;
                Disconnected?.Invoke(this, EventArgs.Empty);
                throw;
            }
        }

        #region Message Handlers
        private void HandleWelcomeMessage(Message message)
        {
            string welcomeMessage = message.ReadUTF();
            WelcomeMessageReceived?.Invoke(this, welcomeMessage);
        }

        private void HandleLoginResponse(Message message)
        {
            bool success = message.ReadBoolean();

            if (success)
            {
                UserId = message.ReadInt();
                Username = message.ReadUTF();
            }

            LoginResponseReceived?.Invoke(this, new LoginResponseEventArgs(success, UserId, Username));
        }

        private void HandleAllAuctionsResponse(Message message)
        {
            int count = message.ReadInt();
            var auctions = new List<AuctionItem>();

            for (int i = 0; i < count; i++)
            {
                var auction = new AuctionItem
                {
                    Id = message.ReadInt(),
                    Name = message.ReadUTF(),
                    Description = message.ReadUTF(),
                    CurrentPrice = (decimal)message.ReadDouble(),
                    StartingPrice = (decimal)message.ReadDouble(),
                    EndTime = new DateTime(message.ReadLong()),
                    IsActive = message.ReadBoolean(),
                    HighestBidderId = message.ReadInt(),
                    HighestBidderName = message.ReadUTF()
                };

                auctions.Add(auction);
            }

            AuctionsListReceived?.Invoke(this, new AuctionsListEventArgs(auctions));
        }

        private void HandleBidResponse(Message message)
        {
            bool success = message.ReadBoolean();
            string message_text = message.ReadUTF();

            BidResponseReceived?.Invoke(this, new BidResponseEventArgs(success, message_text));
        }

        private void HandleNewAuction(Message message)
        {
            var auction = new AuctionItem
            {
                Id = message.ReadInt(),
                Name = message.ReadUTF(),
                Description = message.ReadUTF(),
                CurrentPrice = (decimal)message.ReadDouble(),
                EndTime = new DateTime(message.ReadLong()),
                IsActive = true
            };

            NewAuctionReceived?.Invoke(this, new AuctionEventArgs(auction));
        }

        private void HandleAuctionUpdate(Message message)
        {
            int auctionId = message.ReadInt();
            decimal currentPrice = (decimal)message.ReadDouble();
            int highestBidderId = message.ReadInt();
            string highestBidderName = message.ReadUTF();

            AuctionUpdateReceived?.Invoke(this, new AuctionUpdateEventArgs(
                auctionId, currentPrice, highestBidderId, highestBidderName));
        }

        // private void HandleCreateAuctionResponse(Message message)
        // {
        //     bool success = message.ReadBoolean();

        //     int auctionId = 0;
        //     string responseMessage = "";

        //     if (success)
        //     {
        //         auctionId = message.ReadInt();
        //     }

        //     responseMessage = message.ReadUTF();

        //     CreateAuctionResponseReceived?.Invoke(this, new CreateAuctionResponseEventArgs(
        //         success, auctionId, responseMessage));
        // }
        #endregion

        #region API Methods
        public async Task LoginAsync(string username, string password)
        {
            var message = new Message(CommandType.Login);
            message.WriteUTF(username);
            message.WriteUTF(password);

            await SendMessageAsync(message);
        }

        public async Task GetAllAuctionsAsync()
        {
            var message = new Message(CommandType.GetAllAuctions);
            await SendMessageAsync(message);
        }

        public async Task PlaceBidAsync(int auctionId, decimal bidAmount)
        {
            var message = new Message(CommandType.PlaceBid);
            message.WriteInt(auctionId);
            message.WriteDouble((double)bidAmount);

            await SendMessageAsync(message);
        }

        public async Task CreateAuctionAsync(string name, string description, decimal startingPrice, DateTime endTime)
        {
            var message = new Message(CommandType.CreateAuction);
            message.WriteUTF(name);
            message.WriteUTF(description);
            message.WriteDouble((double)startingPrice);
            message.WriteLong(endTime.Ticks);

            await SendMessageAsync(message);
        }
        #endregion
    }
}