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

        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            _context = context;
            userManager = _userManager;
        }



        [HttpGet]
         public IActionResult Index()
        {

            var previews = new List<Tuple<Topic, List<Message>, DateTime>>();

            foreach(Topic t in _context.Topics.Include(x=>x.Messages).ToList())
            {
                var messagesInTopic = t.Messages.ToList();
                
                previews.Add(new Tuple<Topic, List<Message>, DateTime>(
                    t,
                    t.Messages.Take(3).ToList(),
                    t.Messages.Count()>0? t.Messages.Last().CreatedAt:DateTime.MinValue
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
                    .Where(m=>m.Topic.Id==topic.Id)
                    .ToList(),
                IsModer = User.IsInRole("moder"),
            };



            foreach(Message m in model.Messages)
            {
                m.Author = userManager.Users.FirstOrDefault(u => u.UserName == m.Author.UserName);
            }

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
                    return Json("YOU ARE MUTED");
                }                
            }

            var topic = _context.Topics.First(t => t.Id == model.NewMessageTopicId);
            if (topic==null)
            {
                return Json("TOPIC NOT FOUND");
            }
            else if (topic.IsClosed)
            {
                return Json("YOU CANT POST TO CLOSED TOPIC");
            }


            Message newMsg = new Message()
            {
                Text = model.NewMessageText,
                Author = _context.Users.First(u => u.UserName == User.Identity.Name),
                CreatedAt = DateTime.UtcNow,
                PictureUrl = model.NewMessagePictureUrl
            };
            _context.Messages.Add(newMsg);
            _context.SaveChanges();

            return Json("OK");
        }

        [HttpPost]
        public IActionResult AddNewTopic(TopicViewModel model)
        {
            if(userManager.Users.First(u => u.UserName == User.Identity.Name).IsMuted)
            {
                return Json("YOU ARE MUTED");
            }

            Topic newTopic = new Topic()
            {
                Title = model.NewTopicTitle,
                IsClosed = false,
                IsSticky = false,
            };
            _context.Topics.Add(newTopic);
            _context.SaveChanges();

            Message opMessage = new Message()
            {
                Text = model.NewMessageText,
                CreatedAt = DateTime.UtcNow,
                PictureUrl = model.NewMessagePictureUrl
            };             
            _context.Messages.Add(opMessage);
            _context.Update(newTopic);

            _context.SaveChanges();
            

            return Json($"Topic #{newTopic.Id} added!");
            
        }

        [HttpDelete]
        [Authorize(Roles = "moder")]
        public IActionResult RemoveMessage(int id)
        {
            var msg = _context.Messages.First(m => m.Id == id);
            _context.Messages.Remove(msg);
            _context.SaveChanges();
            return Json("Message removed");
            
        }

        [HttpDelete]
        [Authorize(Roles = "moder")]
        public IActionResult RemoveTopic(int id)
        {
            var topic = _context.Topics.First(t => t.Id == id);
            _context.Topics.Remove(topic);
            _context.SaveChanges();
            return Json("Topic removed");
                     
        }

        [HttpPost]
        [Authorize(Roles ="moder")]
        public IActionResult ToggleTopicSticky(int id)
        {
            var topic = _context.Topics.First(t => t.Id == id);
            topic.IsSticky = !topic.IsSticky;
            _context.Topics.Update(topic);
            _context.SaveChanges();
            return Json($"Topic sticky: {topic.IsSticky}");            
        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        public IActionResult ToggleTopicClosed(int id)
        {
            var topic = _context.Topics.First(t => t.Id == id);
            topic.IsClosed = !topic.IsClosed;
            _context.Topics.Update(topic);
            _context.SaveChanges();
            return Json($"Topic closed: {topic.IsClosed}");
            
        }





    }
}