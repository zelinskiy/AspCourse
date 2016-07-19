using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int TopicId { get; set; }
        
        public virtual Topic Topic { get; set; }

        public string PictureUrl { get; set; }

        public string Text { get; set; }
        
        public string AuthorName { get; set; }
        
        public virtual ApplicationUser Author { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        
    }
}
