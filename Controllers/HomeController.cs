using Airbnb.Models;
using Airbnbfinal.Data;
using Airbnbfinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System;
using System.Diagnostics;

namespace Airbnbfinal.Controllers
{
    public class HomeController : Controller
    {
        private Graduationproject1Context db;
        private UserManager<ApplicationUser> userManager;
        SignInManager<ApplicationUser> signInManager;

        public HomeController(Graduationproject1Context db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            List<Hotel> H = db.Hotels.Include(a => a.Images).Include(a => a.Rooms).ToList();
            return View(H);
        }

        public IActionResult Search(string searching)
        {
            if (!string.IsNullOrEmpty(searching))
            {
                List<Hotel> H = db.Hotels.Include(a => a.City).Include(a => a.Images).Where(a => a.City.CityName == searching || a.Name == searching).ToList();

                return View(H);
            }
            else
            {
                return View();
            }

        }
        
        [Authorize]
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            SelectList cities = new SelectList(db.Cities.ToList(), "CityId", "CityName");
            ViewBag.city = cities;

            SelectList categ = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            ViewBag.category = categ;



            return View();
        }
        [HttpPost]
        [Authorize]
        
        public async Task<IActionResult> Create(Hotel h /*,int[] FacilitiesToAdd*/)
        {
            SelectList cities = new SelectList(db.Cities.ToList(), "CityId", "CityName");
            ViewBag.city = cities;

            SelectList categ = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            ViewBag.category = categ;
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id;
            h.Hotel_admin= userid;
            if (ModelState.IsValid)
            {
                db.Add(h);
                db.SaveChanges();
                TempData["Hid"] = h.ID;
                return RedirectToAction("Facilities");
            }

            return View(h);
        }

        [HttpGet]
        public IActionResult Facilities()
        {
            ViewBag.fac = new SelectList(db.Facilities.ToList(), "FacilityId", "FacilityType");
            return View();
        }


        [HttpPost]
        public IActionResult Facilities(int[] facilit)
        {
            int myData = (int)TempData["Hid"];
            TempData.Keep("Hid");

            ViewBag.Id = myData;

            Hotel h = db.Hotels.Include(a => a.Facilities).FirstOrDefault(a => a.ID == myData);

            if (ModelState.IsValid)
            {
                foreach (var item in facilit)
                {
                    h.Facilities.Add(db.Facilities.FirstOrDefault(a => a.FacilityId == item));
                }
                db.SaveChanges();
                return RedirectToAction("ImagesAdd");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public IActionResult ImagesAdd()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ImagesAdd(List<IFormFile> files)
        {
                int myData = (int)TempData["Hid"];
            TempData.Keep("Hid");

            var imageCount = 1;
           
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        var fileExtension = Path.GetExtension(fileName);
                        var newFileName = $"{myData}-{imageCount}{fileExtension}";
                        var Fname = $"/photos/{myData}-{imageCount}{fileExtension}";
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", newFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var img = new Image { hotel_id = myData, img = Fname };
                        
                        db.Images.Add(img);
                        db.SaveChangesAsync();
                        imageCount++;
                    }

                }

                return RedirectToAction("RoomAdd");
           
        }

            [HttpGet]
        public IActionResult RoomAdd()
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult RoomAdd(Room rooms)
        {
            int myData = (int)TempData["Hid"];
            TempData.Keep("Hid");
            rooms.Hotel_Id=myData;
            db.Rooms.Add(rooms);
            db.SaveChanges();
            ModelState.Clear();
            return View();
        }


        public IActionResult Success()
        {
            return View();
        }



        public IActionResult category(int cid)
        {
            List<Hotel> H = db.Hotels.Include(a => a.Category).Include(a => a.Images).Where(a =>a.Category_Id==cid).ToList();

            return View(H);  
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Messages()
        {
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id;
            var username = user.UserName;
            ViewBag.Username = username;
            var messages = await db.Messages
         .Where(m => m.HotelmangerId == userid)
         .ToListAsync();
            var groupedMessages = new Dictionary<string, List<Message>>();
            foreach (var message in messages)
            {
                if (!groupedMessages.ContainsKey(message.UserId))
                {
                    groupedMessages[message.UserId] = new List<Message>();
                }
                groupedMessages[message.UserId].Add(message);
            }
            var viewModel = new ViewMessagesViewModel
            {
                UserId = userid,
                GroupedMessages = groupedMessages
            };

            return View(viewModel);

        }

        [Authorize]
        [HttpPost]
        public IActionResult SendMessage(string receiverId, string senderId, string message)
        {
            var newMessage = new Message
            {
                HotelmangerId = receiverId,
                UserId = senderId,
                Message1 = message,
                
            };

            db.Messages.Add(newMessage);
            db.SaveChanges();

            return RedirectToAction("Messages", new { userId = receiverId });
        }
    }
}