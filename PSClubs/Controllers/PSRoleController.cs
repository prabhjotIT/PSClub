using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSClubs.Controllers
{
    [Authorize (Roles= "administrators")]
    public class PSRoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<IdentityUser> _userManager;

        public PSRoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            return View(roles);
        }

        public IActionResult Create()
        {
    
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string ?name="")
        {
            
            IdentityRole role = new IdentityRole();
            
            role.Name = name;
            if (string.IsNullOrEmpty(name))
            {

                //ModelState.AddModelError("Name", $"Role name cannot be left blank");
                TempData["Message"]= $"Role name cannot be left blank";
                
                return RedirectToAction("Index");
            }
            var roleExist = _roleManager.Roles.Where(r => r.Name.Equals(role.Name)).ToList();
            
            if (roleExist.Any())
            {
                //ModelState.AddModelError("Name", $"Role {name} already exist please select a different role");
                ViewData["Message"] = $"Role { name} already exist please select a different role";
                return RedirectToAction("Index");
            }

            try
            {
                name.Trim();
            }
            catch (Exception e)
            {
                TempData["Message"] = $"{e.InnerException.ToString()}";
            }
            IdentityResult result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                //ModelState.AddModelError("Name", $"Role {name} Could not be Created {result.Errors.FirstOrDefault().Description}");
                ViewData["Message"] = $"Role {role.Name} Could not be Created {result.Errors.FirstOrDefault().Description}";
                return RedirectToAction("Index");
            }
            //ViewData["Message"] = $"Role {name} has been added";
            TempData.Keep();
            return RedirectToAction("Index");
            
            
        }

        public async Task<IActionResult> UserManager(string roleName)
        {
            List<IdentityUser> allUsersHavingThisRole = new List<IdentityUser>();
            List<IdentityUser> alluserNotHavingThisRole = new List<IdentityUser>();
            var role = _roleManager.Roles.FirstOrDefault(role=>role.Name.Equals(roleName));
            var allUsers = _userManager.Users.ToList();
            foreach (var user in allUsers)//just iterating through every user to get the details about them such as which roles they belong, they, one user may belong to more than one role
            {
                var allRolesofThisUser = (await _userManager.GetRolesAsync(user)).ToList();//all the role this user contain 
                //now match this user roles with the role we are interested and show that
                if (allRolesofThisUser.Contains(roleName))
                {
                    allUsersHavingThisRole.Add(user);//all the users that are part of the current selected role
                                                     //happy part we got a user with roleName and now added to the list;

                }
                else
                {
                    alluserNotHavingThisRole.Add(user);
                }

            }
            
            TempData["roleName"] = roleName;
            alluserNotHavingThisRole=alluserNotHavingThisRole.OrderBy(r => r.UserName).ToList();
            // i need all users than i need to remove users who are already in this role and i will get users that are not in this role
            ViewData["leftusers"] = new SelectList(alluserNotHavingThisRole,"Id","UserName");
            //ViewBag.leftusers= alluserNotHavingThisRole;
            
            return View(allUsersHavingThisRole);
        }


        public IActionResult AddUserInRole()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUserInRole(string roleName,string name = "" )
        {

            var user = await _userManager.FindByIdAsync(name);
            //add new role for this user
            var addresult=await _userManager.AddToRoleAsync(user, roleName);
            if (addresult.Succeeded)
            {

                TempData["useraddResult"] = $"The user {user.UserName} was added successfully to the role of {roleName}";
                TempData.Keep();
            }
            else
            {
                TempData["useraddResult"] = $"Failed to add user {user.UserName} to role {roleName} Error:{addresult.Errors.FirstOrDefault().Description}";
                TempData.Keep();
            }

            return RedirectToAction("UserManager",new { roleName });//here wehen going to view take roleName as parameter




        }

        
        public async Task<IActionResult> RemoveUserInRole( string id,string roleName)
        {


            var user = await _userManager.FindByIdAsync(id);
            //add new role for this user
            if (user == null)
            {
                TempData["useraddResult"] = $"The user does not exist ";
                TempData.Keep();
                return RedirectToAction("UserManager", new { roleName });
            }
            var removeresult = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (removeresult.Succeeded)
            {

                TempData["useraddResult"] = $"The user {user.UserName} was removed successfully from the role of {roleName}";
                TempData.Keep();
            }
            else
            {
                TempData["useraddResult"] = $"Failed to remove user {user.UserName} to role {roleName} Error:{removeresult.Errors.FirstOrDefault().Description}";
                TempData.Keep();
            }

            return RedirectToAction("UserManager", new { roleName });//here wehen going to view take roleName as parameter
        }

        public async Task<IActionResult> DeleteExistingRole(string roleName)
        {

            //checkin if role is administrators role and found true return to index
            if (roleName.Equals("administrators"))
            {
                TempData["Message"] = $"administrators cannot be deleted";

                return RedirectToAction("Index");
            }
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["Message"] = $"This role does not exist.";

                return RedirectToAction("Index");
            }
            var allUserInThisRole = await _userManager.GetUsersInRoleAsync(roleName);
            TempData["deleterole"] = roleName;
            if (allUserInThisRole.Count == 0)
            {
                //simply remove role
                
                    IdentityRole roleToDelete = await _roleManager.FindByNameAsync(roleName);
                    var deleteresult=await _roleManager.DeleteAsync(roleToDelete);
                if (deleteresult.Succeeded)
                {
                    TempData["Message"] = $"The role {roleName} has been deleted";

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["deletefailed"] = $"The role {roleName} has not been deleted due to error : {deleteresult.Errors.FirstOrDefault().Description}";
                    TempData["toBeDeletedRoleName"] = roleName;
                    return View(allUserInThisRole);
                }
                
            }
            else
            {
                //show list of all current users in this role before user finnaly confirms deletion
                return View(allUserInThisRole);
            }
        }
        [HttpPost]
        [ActionName("DeleteExistingRole")]
        public async Task<IActionResult> Delete(string roleName)
        {
            var allUserInThisRole = await _userManager.GetUsersInRoleAsync(roleName);
            IdentityRole roleToDelete = await _roleManager.FindByNameAsync(roleName);
            var deleteresult = await _roleManager.DeleteAsync(roleToDelete);
            if (deleteresult.Succeeded)
            {
                TempData["Message"] = $"The role {roleName} has been deleted";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["deletefailed"] = $"The role {roleName} has not been deleted due to error : {deleteresult.Errors.FirstOrDefault().Description}";

                return View(allUserInThisRole);
            }

            
        }

    }
}
