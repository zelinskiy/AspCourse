using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.AccountViewModels
{
    public class ProfileViewModel
    {

        public ApplicationUser User { get; set; }

        //for setting
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public string Color { get; set; }
    }
}
