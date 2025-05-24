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
        public long TimeLeft { get; set; }
        public bool isSold { get; set; }
        public double LastestBidPrice { get; set; }
        public int LastestBidderId { get; set; }
        public string LastestBidderName { get; set; }

        public Item(int id, string name, string description, double startingPrice, long timeLeft)
        {
            Id = id;
            Name = name;
            Description = description;
            StartingPrice = startingPrice;
            TimeLeft = timeLeft;
            isSold = true;
        }

        public Item(int id, int lastestBidderId, string name, string description, string imgURL, double startingPrice, double buyNowPrice,
            double lastestBidPrice, bool isSold, long timeLeft)
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
            TimeLeft = timeLeft;
        }


    }
}
