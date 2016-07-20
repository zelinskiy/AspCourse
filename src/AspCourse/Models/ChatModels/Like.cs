using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Like
    {
        public int Id { get; set; }

        public string Type { get; set; }
        
        public virtual Message Message { get; set; }
        
        public virtual ApplicationUser User { get; set; }

    }
}
