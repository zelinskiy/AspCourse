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

            var previews = new List<Tuple<Topic, List<Message>, DateTime>>();

            foreach(Topic t in chatContext.Topics.ToList())
            {
                var messagesInTopic = chatContext
                    .Messages
                    .Where(m => m.TopicId == t.Id)
                    .OrderBy(m=>m.CreatedAt);

                foreach (Message m in messagesInTopic)
                {
                    m.Author = userManager.Users.FirstOrDefault(u => u.UserName == m.AuthorId);
                }

                previews.Add(new Tuple<Topic, List<Message>, DateTime>(
                    t,
                    messagesInTopic.Take(3).ToList(),
                    messagesInTopic.Count()>0?messagesInTopic.Last().CreatedAt:DateTime.MinValue
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

            var topic = chatContext.Topics.First(t => t.Id == model.NewMessageTopicId);
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
                TopicId = model.NewMessageTopicId,
                AuthorId = User.Identity.Name,
                CreatedAt = DateTime.UtcNow,
                PictureUrl = model.NewMessagePictureUrl
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
                CreatedAt = DateTime.UtcNow,
                PictureUrl = model.NewMessagePictureUrl
            };             
            chatContext.Messages.Add(opMessage);
            newTopic.OpPostId = opMessage.Id;
            chatContext.Update(newTopic);

            chatContext.SaveChanges();
            

            return Json($"Topic #{newTopic.Id} added!");
            
        }

        [HttpDelete]
        [Authorize(Roles = "moder")]
        public IActionResult RemoveMessage(int id)
        {
            var msg = chatContext.Messages.First(m => m.Id == id);
            chatContext.Messages.Remove(msg);
            chatContext.SaveChanges();
            return Json("Message removed");
            
        }

        [HttpDelete]
        [Authorize(Roles = "moder")]
        public IActionResult RemoveTopic(int id)
        {
            var topic = chatContext.Topics.First(t => t.Id == id);
            chatContext.Topics.Remove(topic);
            chatContext.SaveChanges();
            return Json("Topic removed");
                     
        }

        [HttpPost]
        [Authorize(Roles ="moder")]
        public IActionResult ToggleTopicSticky(int id)
        {
            var topic = chatContext.Topics.First(t => t.Id == id);
            topic.IsSticky = !topic.IsSticky;
            chatContext.Topics.Update(topic);
            chatContext.SaveChanges();
            return Json($"Topic sticky: {topic.IsSticky}");            
        }

        [HttpPost]
        [Authorize(Roles = "moder")]
        public IActionResult ToggleTopicClosed(int id)
        {
            var topic = chatContext.Topics.First(t => t.Id == id);
            topic.IsClosed = !topic.IsClosed;
            chatContext.Topics.Update(topic);
            chatContext.SaveChanges();
            return Json($"Topic closed: {topic.IsClosed}");
            
        }





    }
}