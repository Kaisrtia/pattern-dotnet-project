using Microsoft.EntityFrameworkCore;

namespace pattern_project.Database {
  public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
  }
}
