using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspCourse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AspCourse.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UserManager<ApplicationUser> userManager;

        public HomeController(UserManager<ApplicationUser> _userManager)
        {
            userManager = _userManager;
        }


        public IActionResult Index()
        {

            var user = userManager.Users.First(u => u.UserName == User.Identity.Name);

            user.LastSeenAt = DateTime.UtcNow;
            userManager.UpdateAsync(user);

            

            return View();
        }

        

        public IActionResult Error()
        {
            return View();
        }
    }
}
