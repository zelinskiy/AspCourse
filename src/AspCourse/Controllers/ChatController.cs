using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspCourse.Models.ChatModels.ChatViewModels;
using AspCourse.Models.ChatModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using AspCourse.Services;
using Microsoft.AspNetCore.Identity;
using AspCourse.Models;
using AspCourse.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AspCourse.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {

        public ApplicationDbContext _context;
        UserManager<ApplicationUser> userManager;

        public ChatController(ApplicationDbContext context,
            UserManager<ApplicationUser> _userManager)
        {
            _context = context;
            userManager = _userManager;
        }


        private ApplicationUser Me
        {
            get
            {
                return userManager.Users.First(u => u.UserName == User.Identity.Name);
            }
        }


        [HttpGet]
         public IActionResult Index()
        {
            var previews = new List<Tuple<Topic, List<Message>, DateTime>>();

            foreach(Topic t in _context.Topics
                .Include(x=>x.Messages)                
                .Include(x=>x.Author)
                .ToList())
            {
                var messagesInTopic = _context.Messages
                    .Include(m => m.Author)
                    .Include(m => m.Topic)
                    .Include(m=>m.Likes)
                    .Where(m=>t.Id == m.Topic.Id);
                
                previews.Add(new Tuple<Topic, List<Message>, DateTime>(
                    t,
                    messagesInTopic.ToList(),
                    messagesInTopic.Count()>0? t.Messages.Last().CreatedAt:DateTime.MinValue
                    ));
            }

            IndexViewModel model = new IndexViewModel()
            {
                Previews = previews,
                IsModer = User.IsInRole("moder"),                
            };           

            return View(model);
        }



        [HttpGet]
        public IActionResult Topic(int id)
        {
            Topic topic = _context.Topics.FirstOrDefault(t => t.Id == id);

            if (topic == null) return RedirectToAction("Index");

            TopicViewModel model = new TopicViewModel()
            {
                Topic = topic,
                Messages = _context.Messages
                    .Include(m=>m.Author)
                    .Include(m => m.Likes)
                    .Where(m=>m.Topic.Id == topic.Id)
                    .ToList(),
                IsModer = User.IsInRole("moder"),
            };
                        

            return View(model);
        }


        //***************************************


        [HttpPost]
        public async Task<IActionResult> AddNewMessage(TopicViewModel model)
        {
            var user = userManager.Users.First(u => u.UserName == User.Identity.Name);
            if (user.IsMuted)
            {
                if (user.MutedUntil.ToUniversalTime() < DateTime.UtcNow)
                {
                    user.IsMuted = false;
                    await userManager.UpdateAsync(user);
                }
                else
                {
                    return Content("YOU ARE MUTED");
                }                
            }

            var topic = _context.Topics.First(t => t.Id == model.NewMessageTopicId);
            if (topic==null)
            {
                return Content("TOPIC NOT FOUND");
            }
            else if (topic.IsClosed)
            {
                return Content("YOU CANT POST TO CLOSED TOPIC");
            }


            Message newMsg = new Message()
            {
                Text = model.NewMessageText,
                Author = _context.Users.First(u => u.UserName == User.Identity.Name),
                CreatedAt = DateTime.UtcNow,
                PictureUrl = model.NewMessagePictureUrl,
                Topic = topic
            };
            _context.Messages.Add(newMsg);
            _context.SaveChanges();

            return Content("OK");
        }

        [HttpPost]
        public IActionResult AddNewTopic(TopicViewModel model)
        {
            var user = userManager.Users.First(u => u.UserName == User.Identity.Name);
            if (user.IsMuted)
            {
                return Json("YOU ARE MUTED");
            }

            Topic newTopic = new Topic()
            {
                Title = model.NewTopicTitle,
                Author = user, 
                IsClosed = false,
                IsSticky = false,
            };
            _context.Topics.Add(newTopic);
            _context.SaveChanges();

            Message opMessage = new Message()
            {
                Text = model.NewMessageText,
                CreatedAt = DateTime.UtcNow,
                Author = user,
                Topic = _context.Topics.First(t => t.Id == newTopic.Id),
                PictureUrl = model.NewMessagePictureUrl
            };

            
            _context.Messages.Add(opMessage);            
            _context.SaveChanges();
            
            

            return Content($"Topic #{newTopic.Id} added!");
            
        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        public IActionResult RemoveMessage(int id)
        {
            var msg = _context.Messages
                .Include(m => m.Topic)
                .Include(m=>m.Topic.Messages)                
                .First(m => m.Id == id);

            if(msg.Topic.Messages.Count == 1)
            {
                return RemoveTopic(msg.Topic.Id);
            }

            var likesToRemove = _context.Likes.Where(l => l.Message.Id == id);
            _context.Likes.RemoveRange(likesToRemove);

            _context.Messages.Remove(msg);
            _context.SaveChanges();            

            return Content("Message removed");
            
        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        public IActionResult RemoveTopic(int id)
        {
            var topic = _context.Topics
                .Include(t=>t.Messages)
                .First(t => t.Id == id);    

            var likesToRemove = _context.Likes.Where(l => l.Message.Topic.Id == topic.Id);
            _context.Likes.RemoveRange(likesToRemove);

            _context.Messages.RemoveRange(topic.Messages);            
            _context.Topics.Remove(topic);
            _context.SaveChanges();
            return Content("Topic removed");
                     
        }

        [HttpPost]
        [Authorize(Roles ="moder")]
        public IActionResult ToggleTopicSticky(int id)
        {
            var topic = _context.Topics.First(t => t.Id == id);
            topic.IsSticky = !topic.IsSticky;
            _context.Topics.Update(topic);
            _context.SaveChanges();
            return Content($"Topic sticky: {topic.IsSticky}");            
        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        public IActionResult ToggleTopicClosed(int id)
        {
            var topic = _context.Topics.First(t => t.Id == id);
            topic.IsClosed = !topic.IsClosed;
            _context.Topics.Update(topic);
            _context.SaveChanges();
            return Content($"Topic closed: {topic.IsClosed}");
            
        }



        [HttpPost]
        public async Task<IActionResult> ToggleLikeMessage(int id)
        {
            var msg = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (msg == null) return Content("Message not found");
            

            var oldLike = _context.Likes.FirstOrDefault(l => l.Message.Id == msg.Id && l.User.Id == Me.Id);

            if (oldLike == null)
            {
                _context.Likes.Add(new Like()
                {
                    Type = "Like",
                    Message = msg,
                    User = Me,
                });
                _context.SaveChanges();
                return Content("Like Added");
            }
            else
            {
                _context.Likes.Remove(oldLike);
                _context.SaveChanges();
                return Content("Like Removed");
            }
        }

        

    }
}