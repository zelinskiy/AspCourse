﻿using AspCourse.Models.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.AccountViewModels
{
    public class ProfileViewModel
    {

        public ApplicationUser User { get; set; }
        public List<Message> UserMessages { get; set; }

        public bool IsMyself { get; set; }
        public bool IsModer { get; set; }

        //for setting
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public string Color { get; set; }
                
    }
}
