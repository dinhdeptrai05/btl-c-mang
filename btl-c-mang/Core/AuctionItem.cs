using System;

namespace Client.Core
{
    public class AuctionItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public int HighestBidderId { get; set; }
        public string HighestBidderName { get; set; }
    }
}