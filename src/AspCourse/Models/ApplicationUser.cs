using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AspCourse.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string NickName { get; set; } = "Anonymous";

        public string AvatarUrl { get; set; } = "https://i.imgur.com/op52w9Q.jpg";

        public string Color { get; set; }

        public bool IsBanned { get; set; }
        public DateTime BannedUntil { get; set; }

        public bool IsMuted { get; set; }
        public DateTime MutedUntil { get; set; }
        

        public DateTime RegisteredAt { get; set; }
        public DateTime LastSeenAt { get; set; }


    }
}
