using BookStore.DataAccess.Data;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStoreWeb.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;

    public DbInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
    }
    public void Initialize()
    {
        //migratios if they are not applied
        try
        {
            if(_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }
        }
        catch(Exception ex)
        {

        }

        //create roles if they are not created
        if (!_roleManager.RoleExistsAsync(SD.Role_User_Cust).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Cust)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Employee)).GetAwaiter().GetResult();
            //if role are not created, then we will create admin user as wellS
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "riotgheptrannhulol@gmail.com",
                Email = "riotgheptrannhulol@gmail.com",
                Name = "Tran Trung Tinh",
                PhoneNumber = "0353537180",
                StreetAddress = "99 To Hien Thanh",
                StrictAddress = "Phuoc My",
                PostalCode = "23422",
                City = "Da Nang"
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "riotgheptrannhulol@gmail.com");
            _userManager.AddToRoleAsync(user, SD.Role_User_Admin).GetAwaiter().GetResult();

        }
        return;

    }
}
