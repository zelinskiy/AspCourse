using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Models.ChatModels
{
    public class Message
    {
        //DB fields go there
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }

        //End of db fields



        public Message(string user, string text)
        {
            if(user == null || text == null)
            {
                throw new NullReferenceException("Author and/or text cant be null");
            }

            this.AuthorId = user;
            this.Text = text;
        }
    }
}
