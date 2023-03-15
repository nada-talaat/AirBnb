using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Airbnb.Models;
using Airbnbfinal.Data;
using Airbnbfinal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Airbnbfinal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HotelController : Controller
    {
        private  Graduationproject1Context _context;
        private  UserManager <ApplicationUser> userManager;

        public HotelController(Graduationproject1Context context , UserManager <ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Hotel
        public async Task<IActionResult> Index()
        {
            var graduationproject1Context = _context.Hotels.Include(h => h.Facilities).Include(h => h.City).Include(h => h.Hotel_adminNavigation);
            return View(await graduationproject1Context.ToListAsync());
        }

        // GET: Hotel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Category)
                .Include(h => h.City)
                .Include(h => h.Hotel_adminNavigation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }
        
        // GET: Hotel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            ViewData["Category_Id"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", hotel.Category_Id);
            ViewData["City_Id"] = new SelectList(_context.Cities, "CityId", "CityId", hotel.City_Id);
            ViewData["Hotel_admin"] = new SelectList(_context.AspNetUsers, "Id", "Id", hotel.Hotel_admin);
            return View(hotel);
        }

        // POST: Hotel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Address,Phone,Email,Website,Rate,Is_Available,City_Id,Category_Id,Hotel_admin")] Hotel hotel)
        {
            if (id != hotel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Category_Id"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", hotel.Category_Id);
            ViewData["City_Id"] = new SelectList(_context.Cities, "CityId", "CityId", hotel.City_Id);
            ViewData["Hotel_admin"] = new SelectList(_context.AspNetUsers, "Id", "Id", hotel.Hotel_admin);
            return View(hotel);
        }

        // GET: Hotel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Hotels == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Category)
                .Include(h => h.City)
                .Include(h => h.Hotel_adminNavigation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Hotels == null)
            {
                return Problem("Entity set 'Graduationproject1Context.Hotels'  is null.");
            }
            var hotel = await _context.Hotels.Include(a => a.Reviews).Include(a => a.Images).Include(a => a.Rooms).Include(a => a.Bookings).Include(a=>a.Facilities).FirstOrDefaultAsync(a=>a.ID==id);
            //var hotel = await _context.Hotels.FindAsync(id);
            var facilityToRemove = hotel.Facilities.ToList();
           

            if (hotel != null)
            {
                foreach (var amenity in facilityToRemove)
                {
                    hotel.Facilities.Remove(amenity);
                }
                _context.Reviews.RemoveRange(hotel.Reviews);
                _context.Rooms.RemoveRange(hotel.Rooms);
                _context.Images.RemoveRange(hotel.Images);
                _context.Bookings.RemoveRange(hotel.Bookings);
                _context.Hotels.Remove(hotel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
          return _context.Hotels.Any(e => e.ID == id);
        }
        public IActionResult ManageUsers()
        {
            var users = _context.AspNetUsers.ToList();
            return View(users);
        }
        [HttpGet]
        public IActionResult EditUser (string Id)
        {
            var user = _context.AspNetUsers.FirstOrDefault(a => a.Id == Id);
            ViewBag.roles = new SelectList(_context.AspNetRoles.ToList(), "Id", "Name");
            return View(user);
        }
        [HttpPost]
        public async  Task<IActionResult> EditUser (string Id ,AspNetUser netUser, string Roles)
        {
           List< string> check = new List<string>();
            List<AspNetRole> diserd = _context.AspNetRoles.ToList();
            foreach (var item in diserd)
            {
                if (item.Id==Roles)
                {
                    check.Add( item.Name);
                }
            }
            if (Id == null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, role);
                await userManager.AddToRolesAsync(user, check);
                _context.Update(netUser);
                _context.SaveChanges();
                var users = _context.AspNetUsers.ToList();
                return RedirectToAction("ManageUsers", users);
            }
            return View(user);
            
        }
        public async Task<IActionResult> DeleteUser(string Id)
        {
            if (Id==null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(Id);
            if (user==null)
            {
                return NotFound();
            }
            await userManager.DeleteAsync(user);
            var users = _context.AspNetUsers.ToList();
            return RedirectToAction("ManageUsers",users);
        }
    }
}
