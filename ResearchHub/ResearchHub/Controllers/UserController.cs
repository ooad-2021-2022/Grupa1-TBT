using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResearchHub.Data;
using ResearchHub.Models;

namespace ResearchHub.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //getting different kinds of users based on keys from other tables:
        public static User GetNormalUser(string aspNetID, List<User> regularUsers)
        {
            return regularUsers.Where(user => user.aspNetID == aspNetID).ToList().FirstOrDefault();
        }

        public static IdentityUser GetAspNetUser(int regularID, List<User> regularUsers, List<IdentityUser> aspNetUsers)
        {
            string aspNetID = regularUsers.Where(usr => usr.id == regularID).ToList().FirstOrDefault().aspNetID;
            return aspNetUsers.Where(usr => usr.Id == aspNetID).ToList().FirstOrDefault();
        } 

        //public static string GetLocationProperty()
        //{
          //  GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
           // watcher.Start(); //started watcher
            //GeoCoordinate coord = watcher.Position.Location;
            //if (!watcher.Position.Location.IsUnknown)
            //{
              //  double lat = coord.Latitude; //latitude
                //double long = coord.Longitude;  //logitude
            //}
      //  }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View();
        }


        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Find
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find(string? username)
        {
            //find requestee id - the one who we want to go out with
            var selectedUsers = _userManager.Users.ToList().Where(usr => usr.UserName == username).ToList();
            User requestee = null;

            if (selectedUsers.Count == 0)
            {
                ErrorViewModel model = new ErrorViewModel();
                model.RequestId = "Searched colleague username does not exist";
                return View("Error", model);
            }
            else
            {
                requestee = GetNormalUser(selectedUsers.First().Id, _context.User.ToList());
                var aspRequestee = GetAspNetUser(requestee.id, _context.User.ToList(), _userManager.Users.ToList());

                if (! await _userManager.IsInRoleAsync(aspRequestee, "VIP User"))
                {
                    ErrorViewModel error = new ErrorViewModel();
                    error.RequestId = "User you are trying to get is not VIP, which implies that he doesn't have privileges for this feature.";
                    return View("Error", error);
                }
            }


            return View("Display", requestee);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/CoffeeTime/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoffeeTime(int id, string message)
        {
            //get current user normal ID
            var currentAspUser = await _userManager.GetUserAsync(HttpContext.User);
            User requester = GetNormalUser(currentAspUser.Id, _context.User.ToList());

            Requests request = new Requests();

            request.requestBody = message;
            request.requesteeID = id;
            request.requesterID = requester.id;
            request.timeRequestMade = DateTime.Now;

            _context.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ResearchPaper");
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.id == id);
        }
    }
}
