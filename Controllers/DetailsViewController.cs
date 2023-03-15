using Airbnb.Models;
using Airbnbfinal.Data;
using Airbnbfinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Airbnbfinal.Controllers
{
    public class DetailsViewController : Controller
    {
        private Graduationproject1Context db;
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext dbContext;

        public DetailsViewController(Graduationproject1Context db, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            this.db = db;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }


        public  IActionResult Index(int id)
        {

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var hotels = db.Hotels.Include(a => a.Images).Include(a => a.Facilities).Include(a => a.Hotel_adminNavigation).Include(a => a.Reviews).ThenInclude(a => a.User).FirstOrDefault(a => a.ID == id);
                //var reviews = db.Reviews.FirstOrDefault(a => a.Hotel_Id == id);



                return View(hotels);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task< IActionResult> addReview(int ID ,string rev)
        {
            
                var hotel = db.Hotels.Include(a => a.Images).Include(a => a.Facilities).Include(a => a.Hotel_adminNavigation).Include(a => a.Reviews).ThenInclude(a => a.User).FirstOrDefault(a => a.ID == ID);
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id;
            
            var review = new Review { Hotel_Id = hotel.ID, User_Id = userid, Review1 = rev };
                db.Reviews.Add(review);
                db.SaveChanges();

            return RedirectToAction("Index",hotel);




        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Contact(int ID, string msg,string Hotel_admin)
        {
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id;
            Message mess = new Message { Message1=msg,HotelmangerId=Hotel_admin,UserId=userid};
            db.Messages.Add(mess);
            db.SaveChanges();
            var hotels = db.Hotels.Include(a => a.Images).Include(a => a.Facilities).Include(a => a.Hotel_adminNavigation).Include(a => a.Reviews).ThenInclude(a => a.User).FirstOrDefault(a => a.ID == ID);
            return RedirectToAction("Index",hotels);
        }

    }

 
}
