using Microsoft.AspNetCore.Identity;
using Rate_Restaurants_App.Models;

namespace Rate_Restaurants_App.Data;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public List<Review> Reviews { get; set; }

    public ApplicationUser()
    {
        Reviews = new List<Review>();
    }
}