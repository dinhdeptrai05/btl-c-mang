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

        public override string ToString()
        {
            return $"Name: {Name}\n" +
             $"Des: {Description}\n" +
             $"IMG: {ImageUrl} \n" +
             $"STR: {StartingPrice} \n" +
             $"BNP: {BuyNowPrice} \n" +
             $"LBI: {LatestBidderId} \n" +
             $"LBP: {LatestBidPrice} \n" +
             $"Sold: {IsSold} \n";
        }
    }
}