using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
namespace BookStore.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> o) : base(o)
    {

    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>().HasData(
             new Category { Id = 1, Name = "Action" },
            new Category { Id = 2, Name = "SciFi" },
            new Category { Id = 3, Name = "History" });
        modelBuilder.Entity<Company>().HasData(
               new Company
               {
                   Id = 1,
                   Name = "Tech Solution",
                   StreetAddress = "123 Tech St",
                   City = "Tech City",
                   PostalCode = "12121",
                   StrictAddress ="IL",
                   PhoneNumber = "6669990000"
               },
               new Company
               {
                   Id = 2,
                   Name = "Vivid Books",
                   StreetAddress = "999 Vid St",
                   City = "Vid City",
                   PostalCode = "66666",
                   StrictAddress = "IL",
                   PhoneNumber = "7779990000"
               },
               new Company
               {
                   Id = 3,
                   Name = "Readers Club",
                   StreetAddress = "999 Main St",
                   City = "Lala land",
                   PostalCode = "99999",
                   StrictAddress = "IL",
                   PhoneNumber = "1113335555"
               }
               );
        modelBuilder.Entity<Product>().HasData(
             new Product
             {
                 Id = 1,
                 Title = "Fortune of Time",
                 Author = "Billy Spark",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                 ISBN = "SWD9999001",
                 ListPrice = 99,
                 Price = 90,
                 Price50 = 85,
                 Price100 = 80,
                 CategoryId = 1,
                 ImageUrl = "/images/product/1136dd2a-316e-4180-84fa-fe95088e69cd.jpg"
             },
                new Product
                {
                    Id = 2,
                    Title = "Dark Skies",
                    Author = "Nancy Hoover",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,
                    ImageUrl = "/images/product/e8064fd8-4d6d-4715-8450-061cb6ea9c15.jpg"

                },
                new Product
                {
                    Id = 3,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 1,
                    ImageUrl = "\\images\\product\\41ba8176-a85b-40cc-80de-d7605bff3526.jpg"

                },
                new Product
                {
                    Id = 4,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 2,
                    ImageUrl = "\\images\\product\\90c79de3-9bcf-4a0f-a8cc-431388006a9a.jpg"
                },
                new Product
                {
                    Id = 5,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageUrl = "\\images\\product\\b3e121ee-f705-4c0c-822a-6239256f7995.jpg"

                },
                new Product
                {
                    Id = 6,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 3,
                    ImageUrl = "\\images\\product\\e1596061-9fcc-4e39-b97a-f19f0beaff39.jpg"

                }
            );
    }
}
