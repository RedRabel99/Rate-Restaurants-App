using Microsoft.AspNetCore.Identity;

namespace Rate_Restaurants_App.Data;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
}