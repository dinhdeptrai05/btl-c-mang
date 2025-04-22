using System;
using System.Collections.Generic;

namespace Client.Core
{
    public class LoginResponseEventArgs : EventArgs
    {
        public bool Success { get; }
        public int UserId { get; }
        public string Username { get; }

        public LoginResponseEventArgs(bool success, int userId, string username)
        {
            Success = success;
            UserId = userId;
            Username = username;
        }
    }

    public class AuctionsListEventArgs : EventArgs
    {
        public List<AuctionItem> Auctions { get; }

        public AuctionsListEventArgs(List<AuctionItem> auctions)
        {
            Auctions = auctions;
        }
    }

    public class BidResponseEventArgs : EventArgs
    {
        public bool Success { get; }
        public string Message { get; }

        public BidResponseEventArgs(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class AuctionEventArgs : EventArgs
    {
        public AuctionItem Auction { get; }

        public AuctionEventArgs(AuctionItem auction)
        {
            Auction = auction;
        }
    }

    public class AuctionUpdateEventArgs : EventArgs
    {
        public int AuctionId { get; }
        public decimal CurrentPrice { get; }
        public int HighestBidderId { get; }
        public string HighestBidderName { get; }

        public AuctionUpdateEventArgs(int auctionId, decimal currentPrice, int highestBidderId, string highestBidderName)
        {
            AuctionId = auctionId;
            CurrentPrice = currentPrice;
            HighestBidderId = highestBidderId;
            HighestBidderName = highestBidderName;
        }
    }
}