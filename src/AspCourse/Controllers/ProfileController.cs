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

namespace AspCourse.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {       
        public ChatDbContext chatContext;
        public ApplicationDbContext appContext;
        UserManager<ApplicationUser> userManager;
        IServiceProvider serviceProvider;
        RoleManager<IdentityRole> roleManager;


        public ProfileController(ChatDbContext context,
            ApplicationDbContext _appContext,
            IServiceProvider _serviceProvider,
            UserManager<ApplicationUser> _userManager)
        {
            chatContext = context;
            appContext = _appContext;
            userManager = _userManager;
            serviceProvider = _serviceProvider;
            roleManager = (RoleManager<IdentityRole>)serviceProvider.GetService(typeof(ApplicationRoleManager));
            //SetupRoles();
        }
        
        private async void SetupRoles()
        {
            await roleManager.CreateAsync(new IdentityRole { Name = "user" });
            await roleManager.CreateAsync(new IdentityRole { Name = "moder" });
        }


        [HttpGet]
        public IActionResult Index()
        {
            var model = new ProfileViewModel();
            model.IsMyself = true;
            model.IsModer = User.IsInRole("moder");
            model.User = userManager.Users.First(u => u.UserName == User.Identity.Name);
            

            //userManager.AddClaimAsync(model.User, new Claim(ClaimTypes.Role, "moder"));
            //userManager.AddClaimAsync(model.User, new Claim(ClaimTypes.Role, "user"));

            return View(model);
   
        }

        [HttpGet]
        public IActionResult GetUser(string username)
        {

            var model = new ProfileViewModel();
            model.IsMyself = false;
            model.IsModer = User.IsInRole("moder");
            model.User = userManager.Users.FirstOrDefault(u => u.UserName==username);

            if(model.User.UserName == User.Identity.Name)
            {
                return RedirectToAction("Index");
            }

            if(model.User == null)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            return View("~/Views/Profile/Index.cshtml", model);

        }

        [HttpGet]
        public IActionResult AllProfiles()
        {
            var model = new AllProfilesViewModel()
            {
                Users = userManager.Users.ToList(),
                Messages = chatContext.Messages.ToList()
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
                Messages = chatContext.Messages.ToList()    
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
        async public Task<IActionResult> Ban(string username, int time)
        {
            var user = userManager.Users.First(u => u.UserName == username);

            if (User.IsInRole("moder"))
            {
                user.IsBanned = true;
                user.BannedUntil = DateTime.Now.AddMinutes(time);
                await userManager.UpdateAsync(user);
            }

            return Json("Banned");

        }

        [HttpPost]
        async public Task<IActionResult> UnMute(string username)
        {
            var user = userManager.Users.First(u => u.UserName == username);

            if (User.IsInRole("moder"))
            {
                user.IsMuted = false;
                await userManager.UpdateAsync(user);
            }

            return Json("UnMuted");

        }

        [HttpPost]
        async public Task<IActionResult> UnBan(string username)
        {
            var user = userManager.Users.First(u => u.UserName == username);

            if (User.IsInRole("moder"))
            {
                user.IsBanned = false;
                await userManager.UpdateAsync(user);
            }

            return Json("UnBanned");

        }


        [HttpPost]
        async public Task<IActionResult> IsModer(string username)
        {
            var user = userManager.Users.First(u => u.UserName == username);
            return Json(await userManager.IsInRoleAsync(user, "moder"));
        }

        [HttpPost]
        async public Task<IActionResult> ToggleModer(string username)
        {
            if (User.IsInRole("moder"))
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
            else
            {
                return Json("Operation not permitted");
            }
            
        }


    }
}