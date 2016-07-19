using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        public virtual Topic Topic { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }


    }
}
