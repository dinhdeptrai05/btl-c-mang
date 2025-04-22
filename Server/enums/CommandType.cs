namespace AuctionServer
{
    public static class CommandType
    {
        // Từ Client đến Server
        public const sbyte Login = 1;
        public const sbyte GetAllAuctions = 2;
        public const sbyte PlaceBid = 3;
        public const sbyte CreateAuction = 4;
        
        // Từ Server đến Client
        public const sbyte Welcome = 101;
        public const sbyte LoginResponse = 102;
        public const sbyte AllAuctionsResponse = 103;
        public const sbyte BidResponse = 104;
        public const sbyte NewAuction = 105;
        public const sbyte AuctionUpdate = 106;
        public const sbyte CreateAuctionResponse = 107;
    }
}