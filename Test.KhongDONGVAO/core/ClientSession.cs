using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Core
{
    public class AuctionClient
    {
        private TcpClient _tcpClient;

        private static NetworkStream stream;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Dictionary<sbyte, Action<Message>> _messageHandlers = new Dictionary<sbyte, Action<Message>>();
        private static AuctionClient _instance;
        private bool _isConnecting = false;

        public bool _isConnected = false;

        public event EventHandler Disconnected;

        public static AuctionClient gI(string ip = null, int port = 0)
        {
            if (_instance == null)
            {
                if (ip == null || port == 0)
                    throw new InvalidOperationException("AuctionClient chưa được khởi tạo với IP và Port.");

                _instance = new AuctionClient(ip, port);
            }

            return _instance;
        }


        public AuctionClient(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
            ConnectAsync();
        }

        private async void ConnectAsync()
        {
            if (_isConnected || _isConnecting) return;
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(IPAddress.Parse(_serverIp), _serverPort);
                stream = _tcpClient.GetStream();
                _isConnected = true;
                Console.WriteLine("Đã kết nối tới server.");

                _ = Task.Run(() => ReceiveLoop(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể kết nối đến server: {ex.Message}");
            }
            finally
            {
                _isConnecting = false;
            }
        }

        private async Task ReceiveLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    byte[] lengthBuffer = new byte[4];
                    stream.Read(lengthBuffer, 0, 4);
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    byte[] buffer = new byte[messageLength];
                    int bytesRead = 0;
                    while (bytesRead < messageLength)
                    {
                        bytesRead += stream.Read(buffer, bytesRead, messageLength - bytesRead);
                    }

                    var message = new Message(buffer);

                    Controller.HandleMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kết nối bị đóng: " + ex.Message);
                Disconnected?.Invoke(this, EventArgs.Empty);
                _isConnected = false;
            }
        }


        public static void SendMessage(Message message)
        {
            if (stream == null)
                throw new InvalidOperationException("Stream chưa được khởi tạo.");

            try
            {
                byte[] payload = message.GetBytes();
                byte[] lengthPrefix = BitConverter.GetBytes(payload.Length);

                stream.Write(lengthPrefix, 0, 4);
                stream.Write(payload, 0, payload.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi tin nhắn: {ex.Message}");
            }
        }

        public void RegisterHandler(sbyte commandId, Action<Message> handler)
        {
            if (!_messageHandlers.ContainsKey(commandId))
                _messageHandlers.Add(commandId, handler);
            else
                _messageHandlers[commandId] = handler;
        }
    }
}