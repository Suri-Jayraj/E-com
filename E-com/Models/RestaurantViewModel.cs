using System.ComponentModel.DataAnnotations;

namespace E_com.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RestaurantViewModel
    {



        [Key]
        public int RestaurantId { get; set; }
        public string? Name { get; set; }
        public string? PersonalInfo { get; set; } // This is stored as a JSON string
        public string? Description { get; set; }
        public string? Cuisine { get; set; }
        public string? Location { get; set; }
        public decimal? Rating { get; set; }
        public string? ImageUrl { get; set; }
        public string? DeliveryAreas { get; set; }
        public decimal? DeliveryFee { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public string? OperatingHours { get; set; }
        public bool? IsActive { get; set; }

       
    }

   

}




