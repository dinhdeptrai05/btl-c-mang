using System;
using System.IO;
using System.Net.Sockets;
using Client.Core;

namespace Client.Core
{
    public class ClientConnection : IDisposable
    {
        private TcpClient _client;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        public ClientConnection(string host, int port)
        {
            _client = new TcpClient(host, port);
            NetworkStream stream = _client.GetStream();
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);
        }

        public string SendMessage(Message message)
{
    try
        {
            // Serialize the message
            sbyte commandId = message._commandId; // Assuming Message has a CommandId property
            byte[] payload = message.GetBytes(); // Assuming Message has a GetBytes method to get the payload as a byte array

            // Write the command ID
            _writer.Write(commandId);

            // Write the payload length
            _writer.Write(payload.Length);

            // Write the payload
            _writer.Write(payload);

            // Flush the writer to ensure data is sent
            _writer.Flush();

            // Read the response from the server
            string response = _reader.ReadString();

            return response;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }



        public void Dispose()
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _client?.Dispose();
        }
    }
}