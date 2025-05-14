namespace Client.enums
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

        public const sbyte CreateRoom = 8;

        public const sbyte AddItemToRoom = 9;

        public const sbyte StartAuction = 10;

        public const sbyte BuyNow = 11;

        public const sbyte Logout = 12;

        public const sbyte UpdateProfile = 13;

        // Từ Server đến Client
        public const sbyte LoginResponse = -1;
        public const sbyte RegisterResponse = -2;
        public const sbyte getAllRoomResponse = -3;
        public const sbyte JoinRoomResponse = -4;
        public const sbyte ChatMessageReceived = -5;
        public const sbyte UserJoinRoom = -6;
        public const sbyte UserLeaveRoom = -7;
        public const sbyte CreateRoomResponse = -8;
        public const sbyte AddItemResponse = -9;
        public const sbyte StartAuctionResponse = -10;
        public const sbyte AuctionStarted = -11;
        public const sbyte BuyNowResponse = -12;
        public const sbyte LogoutResponse = -13;
        public const sbyte UpdateProfileResponse = -14;
    }
}