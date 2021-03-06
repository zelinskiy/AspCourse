﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels.ChatViewModels
{
    public class TopicViewModel
    {
        public bool IsModer { get; set; }

        public Topic Topic { get; set; }
        public List<Message> Messages { get; set; }

        public string NewMessageText { get; set; }
        public int NewMessageTopicId { get; set; }
        public string NewMessagePictureUrl { get; set; }
        public string NewTopicTitle { get; set; }

        
    }
}
