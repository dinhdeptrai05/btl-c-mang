using System.IO;
using Server.Core;

namespace AuctionServer
{
    public static class MessageExtensions
    {
        public static Stream GetStream(this Message message)
        {
            // Sử dụng reflection để truy cập vào stream private trong Message
            var type = typeof(Message);
            var fieldInfo = type.GetField("_stream", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (Stream)fieldInfo.GetValue(message);
        }
    }
}