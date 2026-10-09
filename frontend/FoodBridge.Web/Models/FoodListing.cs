using System.ComponentModel.DataAnnotations;

namespace FoodBridge.Web.Models
{
    // same shape as the api's FoodListing so the json maps straight across
    public class FoodListing
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = "";

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Quantity { get; set; } = "";

        [Required]
        [StringLength(100)]
        [Display(Name = "Donor")]
        public string DonorName { get; set; } = "";

        [Required]
        [StringLength(150)]
        [Display(Name = "Pickup address")]
        public string PickupAddress { get; set; } = "";

        [Display(Name = "Collect by")]
        public DateTime CollectBy { get; set; }

        public ListingStatus Status { get; set; }

        [Display(Name = "Claimed by")]
        public string? ClaimedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        // still available but the collect by time has gone past
        public bool IsPastCollectBy => Status == ListingStatus.Available && CollectBy < DateTime.Now;
    }

    public enum ListingStatus
    {
        Available,
        Claimed,
        Collected
    }
}
