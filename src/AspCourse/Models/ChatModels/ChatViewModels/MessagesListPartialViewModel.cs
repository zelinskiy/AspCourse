﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels.ChatViewModels
{
    public class MessagesListPartialViewModel
    {
        public bool IsModer { get; set; }
        
        public List<Message> Messages { get; set; }
    }
}
