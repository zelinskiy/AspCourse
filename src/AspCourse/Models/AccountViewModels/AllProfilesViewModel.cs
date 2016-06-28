using AspCourse.Models.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.AccountViewModels
{
    public class AllProfilesViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<Message> Messages { get; set; }
    }
    
}
