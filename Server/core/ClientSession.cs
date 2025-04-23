using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Core;

namespace AuctionServer
{
    public class ClientSession
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public ClientSession(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public void Start()
        {
            Task.Run(HandleClient);
        }

        private async Task HandleClient()
        {
            try
            {
                while (true)
                {
                    byte[] lengthBuffer = new byte[4];
                    int read = await _stream.ReadAsync(lengthBuffer, 0, 4);
                    if (read < 4) break; // client đóng kết nối

                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] buffer = new byte[messageLength];
                    int bytesRead = 0;

                    while (bytesRead < messageLength)
                    {
                        int currentRead = await _stream.ReadAsync(buffer, bytesRead, messageLength - bytesRead);
                        if (currentRead == 0) break;
                        bytesRead += currentRead;
                    }

                    var message = new Message(buffer);

                    // Truyền context vào xử lý nếu cần
                    Controller.HandleMessage(message, this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected: " + ex.Message);
            }
            finally
            {
                _stream.Close();
                _client.Close();
            }
        }

        public async Task SendMessage(Message message)
        {
            try
            {
                byte[] data = message.GetBytes();
                byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

                await _stream.WriteAsync(lengthPrefix, 0, 4);
                await _stream.WriteAsync(data, 0, data.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi message: " + ex.Message);
            }
        }
    }
}
