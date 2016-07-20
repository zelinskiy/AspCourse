using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Subscription
    {
        public int Id { get; set; }

        public virtual Topic Topic { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}
