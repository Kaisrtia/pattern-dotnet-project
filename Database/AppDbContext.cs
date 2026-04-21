using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using pattern_project.Models;
using pattern_project.Services;

namespace pattern_project.Database
{
  public class AppDbContext : DbContext
  {
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<InternalTransaction> InternalTransactions => Set<InternalTransaction>();
    public DbSet<InterbankTransaction> InterbankTransactions => Set<InterbankTransaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (optionsBuilder.IsConfigured)
      {
        return;
      }

      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false)
          .AddJsonFile("appsettings.Development.json", optional: true)
          .AddEnvironmentVariables()
          .Build();

      var connectionString = configuration.GetConnectionString("DefaultConnection");
      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new InvalidOperationException("DefaultConnection is not configured.");
      }

      optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("Users");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
        entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
        entity.Property(u => u.FullName).HasMaxLength(255).IsRequired();
        entity.Property(u => u.PasswordHash).HasMaxLength(128).IsRequired();
        entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        entity.HasIndex(u => u.Username).IsUnique();
        entity.HasIndex(u => u.Email).IsUnique();
      });

      modelBuilder.Entity<Account>(entity =>
      {
        entity.ToTable("Accounts");
        entity.HasKey(a => a.Id);
        entity.Property(a => a.AccountNumber).HasMaxLength(30).IsRequired();
        entity.Property(a => a.Balance).HasPrecision(18, 2);
        entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        entity.HasIndex(a => a.AccountNumber).IsUnique();
        entity.HasIndex(a => a.UserId).IsUnique();
        entity.HasQueryFilter(a => !a.IsDeleted);
        entity.HasOne(a => a.User)
          .WithOne(u => u.Account)
          .HasForeignKey<Account>(a => a.UserId)
          .OnDelete(DeleteBehavior.Restrict);
        entity.ToTable(t => t.HasCheckConstraint("CK_Accounts_Balance", "[Balance] >= 0"));
      });

      modelBuilder.Entity<Transaction>(entity =>
      {
        entity.UseTptMappingStrategy();
        entity.ToTable("Transactions");
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
        entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        entity.HasIndex(t => new { t.OwnerUserId, t.IsDeleted, t.CreatedAt });
        entity.HasQueryFilter(t => !t.IsDeleted);
        entity.HasOne(t => t.OwnerUser)
          .WithMany(u => u.Transactions)
          .HasForeignKey(t => t.OwnerUserId)
          .OnDelete(DeleteBehavior.Restrict);
        entity.ToTable(t => t.HasCheckConstraint("CK_Transactions_Amount", "[Amount] > 0"));
      });

      modelBuilder.Entity<InternalTransaction>(entity =>
      {
        entity.ToTable("InternalTransactions");
        entity.HasOne(t => t.SourceAccount)
          .WithMany()
          .HasForeignKey(t => t.SourceAccountId)
          .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(t => t.DestinationAccount)
          .WithMany()
          .HasForeignKey(t => t.DestinationAccountId)
          .OnDelete(DeleteBehavior.Restrict);
        entity.ToTable(t => t.HasCheckConstraint(
          "CK_InternalTransactions_SourceDestination",
          "[SourceAccountId] <> [DestinationAccountId]"));
      });

      modelBuilder.Entity<InterbankTransaction>(entity =>
      {
        entity.ToTable("InterbankTransactions");
        entity.Property(t => t.SwiftCode).HasMaxLength(11).IsRequired();
        entity.Property(t => t.DestinationBankName).HasMaxLength(200).IsRequired();
        entity.Property(t => t.DestinationAccountNumber).HasMaxLength(50).IsRequired();
        entity.Property(t => t.DigitalSignature).HasMaxLength(500);
        entity.HasIndex(t => t.SwiftCode);
        entity.HasOne(t => t.SourceAccount)
          .WithMany()
          .HasForeignKey(t => t.SourceAccountId)
          .OnDelete(DeleteBehavior.Restrict);
        entity.ToTable(t => t.HasCheckConstraint(
          "CK_InterbankTransactions_SwiftCode",
          "LEN([SwiftCode]) IN (8, 11)"));
        entity.ToTable(t => t.HasCheckConstraint(
          "CK_InterbankTransactions_DigitalSignature",
          "[RequiresDigitalSignature] = 0 OR [DigitalSignature] IS NOT NULL"));
      });

      var seededAt = new DateTime(2026, 4, 21, 0, 0, 0, DateTimeKind.Utc);
      modelBuilder.Entity<User>().HasData(
        new User
        {
          Id = 1,
          Username = "admin",
          Email = "admin@bank.local",
          FullName = "System Administrator",
          PasswordHash = PasswordHasher.Hash("Admin@123"),
          Role = UserRole.Admin,
          IsActive = true,
          IsDeleted = false,
          CreatedAt = seededAt
        },
        new User
        {
          Id = 2,
          Username = "alice",
          Email = "alice@bank.local",
          FullName = "Alice User",
          PasswordHash = PasswordHasher.Hash("User@123"),
          Role = UserRole.User,
          IsActive = true,
          IsDeleted = false,
          CreatedAt = seededAt
        },
        new User
        {
          Id = 3,
          Username = "bob",
          Email = "bob@bank.local",
          FullName = "Bob User",
          PasswordHash = PasswordHasher.Hash("User@123"),
          Role = UserRole.User,
          IsActive = true,
          IsDeleted = false,
          CreatedAt = seededAt
        });

      modelBuilder.Entity<Account>().HasData(
        new Account
        {
          Id = 1,
          UserId = 2,
          AccountNumber = "970400000001",
          Balance = 500_000_000m,
          IsDeleted = false,
          CreatedAt = seededAt
        },
        new Account
        {
          Id = 2,
          UserId = 3,
          AccountNumber = "970400000002",
          Balance = 300_000_000m,
          IsDeleted = false,
          CreatedAt = seededAt
        });
    }
  }
}
