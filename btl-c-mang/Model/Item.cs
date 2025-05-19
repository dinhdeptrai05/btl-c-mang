using System;

namespace Client.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double StartingPrice { get; set; }
        public double BuyNowPrice { get; set; }
        public string ImageURL { get; set; }
        public DateTime EndTime { get; set; }
        public bool isSold { get; set; }
        public double LastestBidPrice { get; set; }
        public int LastestBidderId { get; set; }
        public string LastestBidderName { get; set; }

        public Item(int id, string name, string description, double startingPrice, DateTime endTime)
        {
            Id = id;
            Name = name;
            Description = description;
            StartingPrice = startingPrice;
            EndTime = endTime;
            isSold = true;
        }

        public Item(int id, int lastestBidderId, string name, string description, string imgURL, double startingPrice, double buyNowPrice,
            double lastestBidPrice, bool isSold, DateTime endTime)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageURL = imgURL;
            BuyNowPrice = buyNowPrice;
            this.isSold = isSold;
            LastestBidderId = lastestBidderId;
            LastestBidPrice = lastestBidPrice;
            StartingPrice = startingPrice;
            EndTime = endTime;
        }
    }
}
