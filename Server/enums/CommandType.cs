namespace AuctionServer
{
    public static class CommandType
    {
        // Từ Client đến Server
        public const sbyte Login = 1;

        public const sbyte Register = 2;

        public const sbyte getAllRoom = 3;

        public const sbyte JoinRoom = 4;

        public const sbyte PlaceBid = 5;

        public const sbyte LeaveRoom = 6;

        public const sbyte SendChatMessage = 7;

        // Từ Server đến Client
        public const sbyte LoginResponse = -1;
        public const sbyte RegisterResponse = -2;
        public const sbyte getAllRoomResponse = -3;
        public const sbyte JoinRoomResponse = -4;
        public const sbyte ChatMessageReceived = -5;
        public const sbyte UserJoinRoom = -6;
        public const sbyte UserLeaveRoom = -7;

    }
}