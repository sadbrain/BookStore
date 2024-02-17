using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Contracts;

namespace BookStore.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_User_Admin)]
public class UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : Controller
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly ApplicationDbContext _db = db;
    [BindProperty]
    public RoleManagmentVM RoleManagmentVM { get; set; }
    public IActionResult Index()
    {

        return View();
    }
    public IActionResult RoleManagment(string userId)
    {
        var userRoles = _db.UserRoles.ToList();
        var roles = _db.Roles;
        var roleId = userRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
        RoleManagmentVM = new()
        {
            ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
            RoleList = roles.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Name,
            }),
            CompanyList = _db.Companies.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString(),
            }),
        };
        RoleManagmentVM.ApplicationUser.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
        return View(RoleManagmentVM);
    }
    [HttpPost]
    public IActionResult RoleManagment()
    {
        var userRoles = _db.UserRoles.ToList();
        var roles = _db.Roles.ToList();
        string roleId = userRoles.FirstOrDefault(u => u.UserId == RoleManagmentVM.ApplicationUser.Id).RoleId;
        var oldRole = roles.FirstOrDefault(u => u.Id == roleId).Name;

        ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == RoleManagmentVM.ApplicationUser.Id);

        if (RoleManagmentVM.ApplicationUser.Role != oldRole)
        {
            if (RoleManagmentVM.ApplicationUser.Role == SD.Role_User_Comp)
            {
                applicationUser.CompanyId = RoleManagmentVM.ApplicationUser.CompanyId;
            }
            if (oldRole == SD.Role_User_Comp)
            {
                applicationUser.CompanyId = null;
            }
            _db.SaveChanges();
            _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(applicationUser, RoleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

        }
        else
        {
            if(oldRole == SD.Role_User_Comp && RoleManagmentVM.ApplicationUser.CompanyId != applicationUser.CompanyId)
            {
                applicationUser.CompanyId = RoleManagmentVM.ApplicationUser.CompanyId;
                _db.SaveChanges();
            }
        }
        return RedirectToAction(nameof(Index));
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
