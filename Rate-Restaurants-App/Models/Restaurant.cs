using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rate_Restaurants_App.Models;

public class Restaurant
{
    [Key]
    public int RestaurantId { get; set; }
    public string? Name { get; set; }
    
    public List<Review>? Reviews { get; set; }
}