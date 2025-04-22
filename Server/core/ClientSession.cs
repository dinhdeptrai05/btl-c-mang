// using System;
// using System.IO;
// using System.Net.Sockets;
// using System.Threading;
// using System.Threading.Tasks;
// using Server.Core;

// namespace AuctionServer
// {
//     public class ClientSession
//     {
//         public TcpClient TcpClient { get; private set; }
//         private NetworkStream _stream;
//         private readonly Server _server;
//         private readonly MessageController _messageController;
//         private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
//         private bool _isConnected = true;

//         public int UserId { get; set; }
//         public string Username { get; set; }
//         public bool IsConnected => _isConnected;

//         public ClientSession(TcpClient client, Server server, MessageController messageController)
//         {
//             TcpClient = client;
//             _stream = client.GetStream();
//             _server = server;
//             _messageController = messageController;
//         }

//         public async Task StartAsync()
//         {
//             try
//             {
//                 // Gửi thông báo chào mừng
//                 var welcomeMessage = new Message(CommandType.Welcome);
//                 welcomeMessage.WriteUTF("Chào mừng đến với hệ thống đấu giá trực tuyến!");
//                 SendMessage(welcomeMessage);

//                 // Bắt đầu đọc data từ client
//                 byte[] buffer = new byte[4096];

//                 while (_isConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
//                 {
//                     // Đọc độ dài của message
//                     byte[] lengthBuffer = new byte[4];
//                     int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4, _cancellationTokenSource.Token);

//                     if (bytesRead < 4)
//                     {
//                         // Kết nối đã bị đóng
//                         break;
//                     }

//                     int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

//                     if (messageLength <= 0 || messageLength > 1024 * 1024) // Giới hạn 1MB
//                     {
//                         // Message không hợp lệ
//                         continue;
//                     }

//                     // Đọc nội dung message
//                     byte[] messageBuffer = new byte[messageLength];
//                     int bytesReadTotal = 0;

//                     while (bytesReadTotal < messageLength)
//                     {
//                         int bytesRemaining = messageLength - bytesReadTotal;
//                         int bytesReceived = await _stream.ReadAsync(messageBuffer, bytesReadTotal, bytesRemaining, _cancellationTokenSource.Token);

//                         if (bytesReceived == 0)
//                         {
//                             // Kết nối đã bị đóng
//                             _isConnected = false;
//                             break;
//                         }

//                         bytesReadTotal += bytesReceived;
//                     }

//                     if (!_isConnected)
//                         break;

//                     // Tạo message từ buffer
//                     using (var ms = new MemoryStream(messageBuffer))
//                     {
//                         // Đọc command ID
//                         sbyte commandId = (sbyte)ms.ReadByte();

//                         // Tạo message từ dữ liệu nhận được
//                         var message = new Message(commandId);

//                         // Copy dữ liệu từ buffer vào message
//                         ms.Position = 0;
//                         byte[] fullBuffer = ms.ToArray();
//                         message.WriteBytes(fullBuffer, 0, fullBuffer.Length);

//                         // Xử lý message
//                         _messageController.HandleMessage(message, this);
//                     }
//                 }
//             }
//             catch (Exception ex) when (ex is SocketException || ex is IOException)
//             {
//                 // Kết nối đã bị đóng
//                 _isConnected = false;
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Lỗi khi xử lý client session: {ex.Message}");
//                 _isConnected = false;
//             }
//             finally
//             {
//                 Close();
//                 _server.RemoveClient(this);
//             }
//         }

//         public void SendMessage(Message message)
//         {
//             try
//             {
//                 if (!_isConnected)
//                     return;

//                 // Lấy dữ liệu từ message
//                 byte[] messageData = ((MemoryStream)message.GetStream()).ToArray();
//                 int messageLength = messageData.Length;

//                 // Gửi độ dài trước
//                 byte[] lengthBytes = BitConverter.GetBytes(messageLength);
//                 _stream.Write(lengthBytes, 0, lengthBytes.Length);

//                 // Gửi dữ liệu message
//                 _stream.Write(messageData, 0, messageData.Length);
//                 _stream.Flush();
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Lỗi khi gửi message: {ex.Message}");
//                 _isConnected = false;
//             }
//         }

//         public void Close()
//         {
//             _isConnected = false;
//             _cancellationTokenSource.Cancel();

//             try
//             {
//                 _stream?.Close();
//                 TcpClient?.Close();
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Lỗi khi đóng kết nối: {ex.Message}");
//             }
//         }
//     }
// }