namespace Rate_Restaurants_App.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public virtual ICollection<Restaurant> Restaurants { get; set; }

    public Tag()
    {
        Restaurants = new HashSet<Restaurant>();
    }
}