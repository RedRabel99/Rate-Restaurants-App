using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rate_Restaurants_App.Models;

namespace Rate_Restaurants_App.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .Property(e => e.FirstName)
            .HasMaxLength(250);
        builder.Entity<ApplicationUser>()
            .Property(e => e.LastName)
            .HasMaxLength(250);

        builder.ApplyConfiguration(new RestaurantEntityTypeConfiguration());
        builder.ApplyConfiguration(new ReviewEntityTypeConfiguration());
        
    }

    public DbSet<Rate_Restaurants_App.Models.Restaurant>? Restaurant { get; set; }
}

public class RestaurantEntityTypeConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        builder.HasKey(x => x.RestaurantId);
    }
}

public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne(b => b.Restaurant)
            .WithMany(r => r.Reviews)
            .HasForeignKey(b => b.RestaurantId);

        builder.Property(b => b.Text).HasMaxLength(500);
        builder.Property(b => b.Rating).IsRequired(true);

        builder.HasOne(b => b.Author)
            .WithMany(u => u.Reviews).IsRequired(true).OnDelete(DeleteBehavior.Cascade);

    }
}
