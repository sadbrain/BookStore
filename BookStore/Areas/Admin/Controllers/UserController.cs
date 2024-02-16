using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Contracts;

namespace BookStore.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_User_Admin)]
public class UserController(ApplicationDbContext db) : Controller
{
    private readonly ApplicationDbContext _db = db;
    [BindProperty]
    public ApplicationUser ApplictionUser { get; set; }
    public IActionResult Index()
    {

        return View();
    }
  

    [HttpGet]
    public IActionResult GetAll()
    {
        var objApplicationUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
        var userRoles = _db.UserRoles.ToList();
        var roles = _db.Roles;
        foreach (var user in objApplicationUserList)
        {
            var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            
            if (user.Company == null)
            {
                user.Company = new Company()
                {
                    Name = ""
                };
             }

        }
        return Json(new { data = objApplicationUserList });
    }
    [HttpPost]
    public IActionResult LockUnlock([FromBody]string id)
    {
        var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        if(objFromDb == null)
        {
            return Json(new { success = false, message = "Error while Loking/UnLocking" });
        }
        
        //lock and unclock
        if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
        {
            //user is currently locked and we need to unlock them
            objFromDb.LockoutEnd = DateTime.Now;
        }
        else
        {
            objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
        }
        _db.SaveChanges();
        return Json(new { success = true, message = "Operation Successful" });

    }
}
