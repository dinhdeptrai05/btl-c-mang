// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading.Tasks;
// using Message = Server.Core.Message;
// using Server.Core;

// namespace AuctionServer
// {

//     class Server
//     {
//         static void Main()
//         {
//             TcpListener listener = new TcpListener(IPAddress.Any, 5000);
//             listener.Start();
//             Console.WriteLine("Server started...");

//             while (true)
//             {
//                 TcpClient client = listener.AcceptTcpClient();
//                 Console.WriteLine("Client connected.");

//                 Thread clientThread = new Thread(() => HandleClient(client));
//                 clientThread.Start();
//             }
//         }

//         static void HandleClient(TcpClient client)
//         {
//             NetworkStream stream = client.GetStream();

//             try
//             {
//                 while (true)
//                 {
//                     byte[] lengthBuffer = new byte[4];
//                     stream.Read(lengthBuffer, 0, 4);
//                     int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

//                     byte[] buffer = new byte[messageLength];
//                     int bytesRead = 0;
//                     while (bytesRead < messageLength)
//                     {
//                         bytesRead += stream.Read(buffer, bytesRead, messageLength - bytesRead);
//                     }

//                     using (var msgStream = new System.IO.MemoryStream(buffer))
//                     {
//                         var message = new Message((sbyte)0);
//                         msgStream.CopyTo(messageStream: messageStream);
//                         msgStream.Position = 0;

//                         // Re-read data from the message
//                         var cmd = message.ReadSByte();
//                         var value1 = message.ReadInt();
//                         var value2 = message.ReadInt();
//                         var value3 = message.ReadShort();

//                         Console.WriteLine($"Received message: cmd={cmd}, val1={value1}, val2={value2}, val3={value3}");
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Client disconnected: " + ex.Message);
//                 client.Close();
//             }
//         }
//     }
// }