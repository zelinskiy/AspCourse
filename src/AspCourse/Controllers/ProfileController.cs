using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspCourse.Models;
using AspCourse.Data;
using Microsoft.AspNetCore.Identity;
using AspCourse.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using AspCourse.Models.ChatModels;
using Microsoft.EntityFrameworkCore;

namespace AspCourse.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {       
        public ApplicationDbContext _context;
        UserManager<ApplicationUser> userManager;
        IServiceProvider serviceProvider;
        RoleManager<IdentityRole> roleManager;


        public ProfileController(ApplicationDbContext context,
            IServiceProvider _serviceProvider,
            UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager
            )
        {
            _context = context;
            userManager = _userManager;
            serviceProvider = _serviceProvider;
            roleManager = _roleManager;
                        
        }
        
        


        [HttpGet]
        public IActionResult Index()
        {
            return GetUser(User.Identity.Name);   
        }

        [HttpGet]
        public IActionResult GetUser(string username)
        {           

            var user = userManager.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null) return View("~/Views/Shared/Error.cshtml");
            
            var model = new ProfileViewModel();
            model.IsMyself = username == User.Identity.Name;
            model.IsModer = User.IsInRole("moder");
            model.User = user;
            
            model.UserTopics = _context.Topics
                .Include(t => t.Messages)
                .Where(t=>t.Author.Id==user.Id)
                .ToList();
                    
           

            return View("~/Views/Profile/Index.cshtml", model);

        }

        [HttpGet]
        public IActionResult AllProfiles()
        {
            var model = new AllProfilesViewModel()
            {
                Users = userManager.Users.ToList(),
                Messages = _context.Messages.ToList()
            };

            ViewData["ListTitle"] = "All Users";
            return View("~/Views/Profile/AllProfiles.cshtml", model);

        }

        [HttpGet]
        public async  Task<IActionResult> AllModers()
        {
            var model = new AllProfilesViewModel()
            {
                Users = (await userManager.GetUsersInRoleAsync("moder")).ToList(),
                Messages = _context.Messages.ToList()
            };

            
            ViewData["ListTitle"] = "Moderators";
            return View("~/Views/Profile/AllProfiles.cshtml", model);

        }

        //**************************************

        [HttpPost]
        public IActionResult UpdateUser(ProfileViewModel model)
        {
            
            var me = userManager.Users.First(u => u.UserName == User.Identity.Name);
            me.Color = model.Color;
            me.AvatarUrl = model.AvatarUrl;
            me.NickName = model.NickName;
            userManager.UpdateAsync(me).Wait();


            return Json($"User {me.UserName} updated ");

        }
        
        [HttpPost]
        [Authorize(Roles = "moder")]
        async public Task<IActionResult> Mute(string username, int time)
        {
            var user = userManager.Users.First(u => u.UserName == username);

            if (User.IsInRole("moder"))
            {
                user.IsMuted = true;
                user.MutedUntil = DateTime.Now.AddMinutes(time);
                await userManager.UpdateAsync(user);
            }
            

            return Json("Muted");

        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        async public Task<IActionResult> Ban(string username, int time)
        {
            var user = userManager.Users.First(u => u.UserName == username);
            user.IsBanned = true;
            user.BannedUntil = DateTime.Now.AddMinutes(time);
            await userManager.UpdateAsync(user);        
            return Json("Banned");

        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        async public Task<IActionResult> UnMute(string username)
        {
            var user = userManager.Users.First(u => u.UserName == username);

            
            user.IsMuted = false;
            await userManager.UpdateAsync(user);
            

            return Json("UnMuted");

        }

        [HttpPost]
        [Authorize(Roles ="moder")]
        async public Task<IActionResult> UnBan(string username)
        {
            var user = userManager.Users.First(u => u.UserName == username);
                        
            user.IsBanned = false;
            await userManager.UpdateAsync(user);
            
            return Json("UnBanned");

        }


        [HttpPost]
        async public Task<IActionResult> IsModer(string username)
        {
            var user = userManager.Users.First(u => u.UserName == username);
            return Json(await userManager.IsInRoleAsync(user, "moder"));
        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        async public Task<IActionResult> ToggleModer(string username)
        {            
            var user = userManager.Users.First(u => u.UserName == username);
            bool isModer = await userManager.IsInRoleAsync(user, "moder");
            if (isModer)
            {
                await userManager.RemoveFromRoleAsync(user, "moder");
            }
            else
            {
                await userManager.AddToRoleAsync(user, "moder");
            }

            return Json("Swithed to:"+(await userManager.IsInRoleAsync(user, "moder")));            
            
        }


    }
}