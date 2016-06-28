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

namespace AspCourse.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {

        public ChatDbContext chatContext;
        UserManager<ApplicationUser> userManager;

        public ChatController(ChatDbContext context, UserManager<ApplicationUser> _userManager)
        {
            chatContext = context;
            userManager = _userManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                Topics = chatContext.Topics.ToList(),
                IsModer = User.IsInRole("moder"),                
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Topic(int id)
        {
            Topic topic = chatContext.Topics.FirstOrDefault(t => t.Id == id);

            if (topic == null) return RedirectToAction("Index");

            TopicViewModel model = new TopicViewModel()
            {
                Topic = topic,
                Messages = chatContext.Messages
                    .Where(m=>m.TopicId==topic.Id)
                    .ToList(),
                IsModer = User.IsInRole("moder"),
            };

            foreach(Message m in model.Messages)
            {
                m.Author = userManager.Users.FirstOrDefault(u => u.UserName == m.AuthorId);
            }

            return View(model);
        }


        //***************************************


        [HttpPost]
        public IActionResult AddNewMessage(TopicViewModel model)
        {
            if (userManager.Users.First(u => u.UserName == User.Identity.Name).IsMuted)
            {
                return Json("YOU ARE MUTED");
            }

            Message newMsg = new Message()
            {
                Text = model.NewMessageText,
                TopicId = model.NewMessageTopicId,
                AuthorId = User.Identity.Name,
                CreatedAt = DateTime.UtcNow
            };
            chatContext.Messages.Add(newMsg);
            chatContext.SaveChanges();

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
            chatContext.Topics.Add(newTopic);
            chatContext.SaveChanges();

            Message opMessage = new Message()
            {
                Text = model.NewMessageText,
                TopicId = newTopic.Id,
                AuthorId = User.Identity.Name,
                CreatedAt = DateTime.UtcNow
            };             
            chatContext.Messages.Add(opMessage);
            newTopic.OpPostId = opMessage.Id;
            chatContext.Update(newTopic);

            chatContext.SaveChanges();
            

            return Json($"Topic #{newTopic.Id} added!");
            
        }

        [HttpDelete]
        public IActionResult RemoveMessage(int id)
        {
            if (User.IsInRole("moder"))
            {
                var msg = chatContext.Messages.First(m => m.Id == id);
                chatContext.Messages.Remove(msg);
                chatContext.SaveChanges();
                return Json("Message removed");
            }
            else
            {
                return Json("Not allowed");
            }
        }

        [HttpDelete]
        public IActionResult RemoveTopic(int id)
        {
            if (User.IsInRole("moder"))
            {
                var topic = chatContext.Topics.First(t => t.Id == id);
                chatContext.Topics.Remove(topic);
                chatContext.SaveChanges();
                return Json("Topic removed");
            }
            else
            {
                return Json("Not allowed");
            }
            
        }





    }
}