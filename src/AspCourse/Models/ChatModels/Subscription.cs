using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TopicId { get; set; }
    }
}
