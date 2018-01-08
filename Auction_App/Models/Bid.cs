using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class Bid: BaseEntity
    {
        public int BidID { get; set;}
        public int BidderId { get; set; }
        public User Bidder { get; set; }
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
        public double Amount { get; set; }

        public Bid(){
            User Bidder = new User();
        }
    }

    public class ViewBid: BaseEntity{
        public int AuctionId { get; set; }
        public double Amount { get; set; }
    }
}