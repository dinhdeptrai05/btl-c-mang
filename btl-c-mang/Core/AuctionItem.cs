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
        public bool isSold { get; set; }
        public int HighestBidderId { get; set; }
        public string HighestBidderName { get; set; }

        public double LastestBidPrice { get; set; }

        public int LastestBidderId { get; set; }
        public string LastestBidderName { get; set; }

        public AuctionItem(int id, string name, string description, decimal startingPrice, DateTime endTime)
        {
            Id = id;
            Name = name;
            Description = description;
            StartingPrice = startingPrice;
            CurrentPrice = startingPrice;
            EndTime = endTime;
            isSold = true;
        }

        public AuctionItem(int id, string name, string description, decimal startingPrice, decimal currentPrice, DateTime endTime, bool isSold, int highestBidderId, string highestBidderName, double lastestBidPrice, int lastestBidderId, string lastestBidderName)
        {
            Id = id;
            Name = name;
            Description = description;
            StartingPrice = startingPrice;
            CurrentPrice = currentPrice;
            EndTime = endTime;
            this.isSold = isSold;
            HighestBidderId = highestBidderId;
            HighestBidderName = highestBidderName;
            LastestBidPrice = lastestBidPrice;
            LastestBidderId = lastestBidderId;
            LastestBidderName = lastestBidderName;
        }
    }
}