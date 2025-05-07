namespace AuctionServer
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double StartingPrice { get; set; }
        public double BuyNowPrice { get; set; }
        public int LatestBidderId { get; set; }
        public double LatestBidPrice { get; set; }
        public bool IsSold { get; set; }
        public DateTime EndTime { get; set; }

        public Item(int id, string name, string description, string imageUrl, double startingPrice, double buyNowPrice, int latestBidderId, double latestBidPrice, bool isSold, DateTime endTime)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            StartingPrice = startingPrice;
            BuyNowPrice = buyNowPrice;
            LatestBidderId = latestBidderId;
            LatestBidPrice = latestBidPrice;
            IsSold = isSold;
            EndTime = endTime;
        }
    }
}