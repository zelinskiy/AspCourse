using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public int OpPostId { get; set; }

        public bool IsSticky { get; set; }
        public bool IsCLosed { get; set; }
    }
}
