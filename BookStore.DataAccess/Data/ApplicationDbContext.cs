using BookStore.Models;
using Microsoft.EntityFrameworkCore;
namespace BookStore.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> o) : base(o)
    {
        
    }
    public DbSet<Category> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
             new Category { Id = 1, Name = "Action" },
            new Category { Id = 2, Name = "SciFi" },
            new Category { Id = 3, Name = "History" });
    }
}
