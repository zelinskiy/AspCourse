using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Topic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public List<Message> Messages { get; set; }

        public virtual List<Subscription> Subscriptions { get; set; }

        [Required]
        public string Title { get; set; }

        public bool IsSticky { get; set; }

        public bool IsClosed { get; set; }
        
    }
}
