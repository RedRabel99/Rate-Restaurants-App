using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rate_Restaurants_App.Data;
using Rate_Restaurants_App.Models;
using Rate_Restaurants_App.ViewModels;

namespace Rate_Restaurants_App.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantController(ApplicationDbContext context)
        {
            _context = context;

        }

        // GET: Restaurant
        public async Task<IActionResult> Index(int? TagId)
        {
            ViewBag.TagId = new SelectList(_context.Tag.OrderBy(c => c.Name).ToList(), "Id", "Name");
            if (_context.Restaurant == null)  return Problem("Entity set 'ApplicationDbContext.Restaurant'  is null.");
            var restaurant = await _context.Restaurant.Include(p => p.Tags).ToListAsync();
            if (TagId.HasValue)
            {
                restaurant = restaurant.Where(p => p.Tags.Any(c => c.Id == TagId)).ToList();
            }

            return View(restaurant);
        }

        // GET: Restaurant/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Restaurant == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurant.Include(p => p.Tags).SingleAsync(i => i.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            restaurant.Reviews = await _context.Review.Where(x => x.RestaurantId == restaurant.RestaurantId)
                .Include(e => e.Restaurant).ToListAsync();


            foreach (var review in restaurant.Reviews)
            {
                review.Author = _context.Users.Find(review.AuthorId);
            }

            return View(restaurant);
        }

        // GET: Restaurant/Create
        [Authorize]
        public IActionResult Create()
        {
            List<SelectListItem> selectListItems = _context.Tag.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            }).ToList();
            var restaurant = new Restaurant();
            PopulateSelectedTagData(restaurant);
            return View(restaurant);
        }

        // POST: Restaurant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("RestaurantId,Name, selectedTags")] Restaurant restaurant, string[] selectedTags)
        {
            if (ModelState.IsValid)
            {
                restaurant.Reviews = new List<Review>();
                UpdateRestaurantTags(selectedTags, restaurant);
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(restaurant);
        }

        // GET: Restaurant/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Restaurant == null)
            {
                return NotFound();
            }


            var restaurant = _context.Restaurant.Include(p => p.Tags)
                .Single(i => i.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            /*
            var allTagsList = _context.Tag.ToList();
            restaurantTagViewModel.AllTags = allTagsList;
            */
            PopulateSelectedTagData(restaurant);
            return View(restaurant);
        }

        // POST: Restaurant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id,
            [Bind("RestaurantId, Name, selectedTags")] Restaurant restaurant, string[] selectedTags)
        {
            var restaurantToUpdate = _context.Restaurant.Include(p => p.Tags)
                .Single(i => i.RestaurantId == id);
            restaurantToUpdate.Name = restaurant.Name;
            if (id != restaurantToUpdate.RestaurantId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    
                    //restaurant.Tags = new HashSet<Tag>(restaurant.Tags.Select(b => b.Id));
                    UpdateRestaurantTags(selectedTags, restaurantToUpdate);
                    _context.Update(restaurantToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurantToUpdate.RestaurantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        private void UpdateRestaurantTags(string[] selectedTags, Restaurant restaurant)
        {
            if (selectedTags == null)
            {
                restaurant.Tags = new List<Tag>();
                return;
            }

            var selectedTagsHS = new HashSet<string>(selectedTags);
            var restaurantTags = new HashSet<int>(restaurant.Tags.Select(b => b.Id));

            foreach (var tag in _context.Tag)
            {
                if (selectedTagsHS.Contains(tag.Id.ToString()))
                {
                    if (!restaurantTags.Contains(tag.Id))
                    {
                        restaurant.Tags.Add(tag);
                    }
                }
                else
                {
                    if (restaurantTags.Contains(tag.Id))
                    {
                        restaurant.Tags.Remove(tag);
                    }
                }
            }
        }

        // GET: Restaurant/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Restaurant == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: Restaurant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Restaurant == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Restaurant'  is null.");
            }

            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurant.Remove(restaurant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurant?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }


        public IActionResult AddReview()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["RestaurantId"] = new SelectList(_context.Restaurant, "RestaurantId", "Name");

            return View(new Review());
        }


        // POST: Restaurant/AddReview/5
        [HttpPost, ActionName("AddReview")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(int id, Review review)
        {
            /*
                       if (_context.Restaurant == null)
                       {
                           return Problem("Entity set 'ApplicationDbContext.Restaurant'  is null.");
                       }
                       var restaurant = await _context.Restaurant.FindAsync(id);
                       if (restaurant != null)
                       {
                           _context.Restaurant.Remove(restaurant);
                       }
                       
                       await _context.SaveChangesAsync();
                       return RedirectToAction(nameof(Index)); */
            if (_context.Restaurant == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Restaurant'  is null.");
            }

            var restaurant = await _context.Restaurant.FindAsync(id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId == null) return RedirectToAction(nameof(Index));
            review.Restaurant = restaurant;
            review.RestaurantId = restaurant.RestaurantId;
            review.Author = _context.Users.Find(userId);
            review.AuthorId = userId;

            _context.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", review.AuthorId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurant, "RestaurantId", "Name", review.RestaurantId);
            return View(review);
        }

        private void PopulateSelectedTagData(Restaurant restaurant)
        {
            var allTags = _context.Tag;
            var restaurantTags = new HashSet<int>(restaurant.Tags.Select(b => b.Id));
            var viewModel = new List<RestaurantTagViewModel>();

            foreach (var tag in allTags)
            {
                viewModel.Add(new RestaurantTagViewModel
                {
                    TagId = tag.Id,
                    TagName = tag.Name,
                    Selected = restaurantTags.Contains(tag.Id)
                });
            }

            ViewBag.Tags = viewModel;
        }

        private void /*Task<List<Tag>>*/ GetSelectedTags(int id)
        {
            Restaurant restaurant = _context.Restaurant.Single(r => r.RestaurantId == id);
            var allTags = _context.Tag;
            var restaurantTags = new HashSet<int>(restaurant.Tags.Select(b => b.Id));
            var viewModel = new List<RestaurantTagViewModel>();
            
            foreach (var tag in allTags)
            {
                viewModel.Add(new RestaurantTagViewModel
                {
                    TagId = tag.Id,
                    TagName = tag.Name,
                    Selected = restaurantTags.Contains(tag.Id)
                });
            }

            ViewBag.Tags = viewModel;
            
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
