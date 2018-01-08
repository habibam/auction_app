using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class Auction: BaseEntity
    {
        [Key]
        public int AuctionId { get; set; }
        public int SellerId { get; set; }
        public User Seller{ get; set; }
        public double StartingBid { get; set; }
        public DateTime EndDate { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }

        public List<Bid> Bids { get; set; }

        [NotMapped]
        public int DaysRemaining { get; set; }

        public Auction(){
            Bids = new List<Bid>();
            // DaysRemaining = (int)(EndDate-DateTime.Now).TotalDays;
        }
    }
    public class CreaAuction: BaseEntity
    {
        [Required(ErrorMessage="Starting Bid is required")]
        [Display(Name = "Starting Bid")]
        public double CreStartingBid { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [FutureDate]
        public DateTime CreEndDate { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        [MinLength(3, ErrorMessage="Product Names must be more than 3 characters")]
        public string CreProduct { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        [MinLength(3, ErrorMessage="Product Names must be more than 3 characters")]
        public string CreDescription { get; set; }

        public class FutureDate : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime date = (DateTime)value;
                return date < DateTime.Now ? new ValidationResult("Date must be in future.") : ValidationResult.Success;
            }
        }
    }
}