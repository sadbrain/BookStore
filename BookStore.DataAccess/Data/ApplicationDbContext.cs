using Microsoft.EntityFrameworkCore;
namespace BookStore.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> o) : base(o)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
