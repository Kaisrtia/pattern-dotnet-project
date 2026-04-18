using Microsoft.EntityFrameworkCore;
using pattern_project.Models;

namespace pattern_project.Database
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Category>(entity =>
      {
        entity.ToTable("Categories");

        entity.HasKey(category => category.Id);

        entity.Property(category => category.Name)
          .IsRequired()
          .HasMaxLength(100);
      });

      modelBuilder.Entity<Product>(entity =>
      {
        entity.ToTable("Products", table =>
        {
          table.HasCheckConstraint("CK_Products_Price_Positive", "[Price] > 0");
          table.HasCheckConstraint("CK_Products_Quantity_Positive", "[Quantity] > 0");
        });

        entity.HasKey(product => product.Id);

        entity.Property(product => product.Name)
          .IsRequired()
          .HasMaxLength(150);

        entity.Property(product => product.Price)
          .HasPrecision(18, 2);

        entity.Property(product => product.Quantity)
          .IsRequired();

        entity.HasOne(product => product.Category)
          .WithMany(category => category.Products)
          .HasForeignKey(product => product.CategoryId)
          .OnDelete(DeleteBehavior.Restrict);
      });
    }
  }
}
