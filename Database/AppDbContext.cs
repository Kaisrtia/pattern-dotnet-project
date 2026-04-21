using Microsoft.EntityFrameworkCore;
using pattern_project.Models;

namespace pattern_project.Database
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<CashAsset> CashAssets => Set<CashAsset>();

    public DbSet<CryptoAsset> CryptoAssets => Set<CryptoAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Asset>(builder =>
      {
        builder.ToTable("Assets", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_Assets_OriginalValue_NonNegative", "[OriginalValue] >= 0"));
        builder.UseTptMappingStrategy();

        builder.HasKey(asset => asset.Id);

        builder.Property(asset => asset.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(asset => asset.ResponsiblePerson)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(asset => asset.OriginalValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(asset => asset.CreatedAtUtc)
            .IsRequired();

      });

      modelBuilder.Entity<CashAsset>(builder =>
      {
        builder.ToTable("CashAssets");
      });

      modelBuilder.Entity<CryptoAsset>(builder =>
      {
        builder.ToTable("CryptoAssets", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_CryptoAssets_Multiplier_NonNegative", "[Multiplier] >= 0"));

        builder.Property(asset => asset.Multiplier)
            .HasPrecision(18, 6)
            .IsRequired();

      });
    }
  }
}
