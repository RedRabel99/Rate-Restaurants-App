using Rate_Restaurants_App.Data;

namespace Rate_Restaurants_App.Models;

public class Review
{
    public int ReviewId { get; set; }
    public int Rating { get; set; }
    public string? Text { get; set; }
    
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }


    public string AuthorId { get; set; } = null!;
    public ApplicationUser? Author { get; set; }
    
}