using Microsoft.AspNetCore.Mvc.Rendering;
using Rate_Restaurants_App.Models;

namespace Rate_Restaurants_App.ViewModels;

public class RestaurantTagViewModel
{
  public int TagId { get; set; }
  public string TagName { get; set; }
  public bool Selected { get; set; }
}   