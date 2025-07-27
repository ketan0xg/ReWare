using System.Collections.Generic;

namespace ReWare.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Condition { get; set; }
        public string Tags { get; set; }

        // Old single image (optional remove later)
        public string ImagePath { get; set; }

        // New fields
        public string ModerationStatus { get; set; } = "Pending";   // Pending, Approved, Rejected
        public string AvailabilityStatus { get; set; } = "Available"; // Available, Reserved, Redeemed, Completed
        public bool IsRedeemable { get; set; } = false;
        public int? PointsCost { get; set; } // If null, default (e.g., 50)

        public string UploadedByUserId { get; set; }
        public virtual ApplicationUser UploadedBy { get; set; }

        // New relation: multiple images
        public virtual ICollection<ItemImage> Images { get; set; }
    }
}
