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

namespace AspCourse.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {       
        public ChatDbContext chatContext;
        UserManager<ApplicationUser> userManager;

        public ProfileController(ChatDbContext context, UserManager<ApplicationUser> _userManager)
        {
            chatContext = context;
            userManager = _userManager;
        }
        


        [HttpGet]
        public IActionResult Index()
        {

            var model = new ProfileViewModel();
            model.User = userManager.Users.First(u => u.UserName == User.Identity.Name);
            
            return View(model);
   
        }

        [HttpGet]
        public IActionResult GetUser(string username)
        {

            var model = new ProfileViewModel();
            model.User = userManager.Users.FirstOrDefault(u => u.UserName==username);

            if(model.User == null)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            return View("~/Views/Profile/Index.cshtml", model);

        }


        [HttpPost]
        public IActionResult UpdateUser(ProfileViewModel model)
        {
            
            var me = userManager.Users.First(u => u.UserName == User.Identity.Name);
            me.Color = model.Color;
            me.AvatarUrl = model.AvatarUrl;
            me.NickName = model.NickName;
            userManager.UpdateAsync(me);

            return Json($"User {me.UserName} updated ");

        }
    }
}