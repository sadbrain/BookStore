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
public class UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    [BindProperty]
    public RoleManagmentVM RoleManagmentVM { get; set; }
    public IActionResult Index()
    {

        return View();
    }
    public IActionResult RoleManagment(string userId)
    {
        var roles = _roleManager.Roles;
        RoleManagmentVM = new()
        {
            ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties:"Company"),
            RoleList = roles.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Name,
            }),
            CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString(),
            }),
        };
        RoleManagmentVM.ApplicationUser.Role = _userManager.GetRolesAsync
         (_unitOfWork.ApplicationUser.Get(u => u.Id == userId))
          .GetAwaiter().GetResult().FirstOrDefault();
        return View(RoleManagmentVM);
    }
    [HttpPost]
    public IActionResult RoleManagment()
    {

        var roles = _roleManager.Roles;
        var oldRole =  _userManager.GetRolesAsync(
            _unitOfWork.ApplicationUser.Get(u => u.Id == RoleManagmentVM.ApplicationUser.Id))
            .GetAwaiter().GetResult().FirstOrDefault();

        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == RoleManagmentVM.ApplicationUser.Id);

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
            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();
            _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(applicationUser, RoleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

        }
        else
        {
            if(oldRole == SD.Role_User_Comp && RoleManagmentVM.ApplicationUser.CompanyId != applicationUser.CompanyId)
            {
                applicationUser.CompanyId = RoleManagmentVM.ApplicationUser.CompanyId;
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
            }
        }
        return RedirectToAction(nameof(Index));
    }

        [HttpGet]
    public IActionResult GetAll()
    {
        var objApplicationUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Company");
        
        foreach (var user in objApplicationUserList)
        {
            user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            
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
        var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
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
        _unitOfWork.ApplicationUser.Update(objFromDb);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Operation Successful" });

    }
}
