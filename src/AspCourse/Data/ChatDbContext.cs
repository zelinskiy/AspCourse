using AspCourse.Models.ChatModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCourse.Data
{
    public class ChatDbContext:DbContext
    {
        

        public ChatDbContext(DbContextOptions<ChatDbContext> options) 
            : base(options) {  }
        



        public DbSet<Message> Messages { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

    }
}
