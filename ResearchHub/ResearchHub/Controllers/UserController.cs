using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: User
        public async Task<IActionResult> DeleteAccounts()
        {
            return View("DeleteAccounts");
        }


        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: User/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? username)
        {
            var selectedUsers = _userManager.Users.ToList().Where(usr => usr.UserName == username).ToList();
            

            if (selectedUsers.Count == 0)
            {
                ErrorViewModel model = new ErrorViewModel();
                model.RequestId = "Account with this username does not exist";
                return View("Error", model);
            }
            else
            {
                var userToDelete = GetNormalUser(selectedUsers.First().Id, _context.User.ToList());
                var aspUserToDelete = GetAspNetUser(userToDelete.id, _context.User.ToList(), _userManager.Users.ToList());

                //delete everything that has relationships with this user

                //remove his roles:
                if (await _userManager.IsInRoleAsync(aspUserToDelete, "VIP User"))
                {
                    await _userManager.RemoveFromRoleAsync(aspUserToDelete, "VIP User");
                } else if (await _userManager.IsInRoleAsync(aspUserToDelete, "Registered user"))
                {
                    await _userManager.RemoveFromRoleAsync(aspUserToDelete, "Registered user");
                }

                await _userManager.DeleteAsync(aspUserToDelete);

                var collaborations = _context.Collaborations.Where(col => col.collaborateeID == userToDelete.id || col.collaboratorID == userToDelete.id).ToList();
                _context.RemoveRange(collaborations);

                var paperAuthors = _context.PaperAuthor.Where(pa => pa.authorID == userToDelete.id).ToList();
                _context.RemoveRange(paperAuthors);

                var publishedPapers = _context.PublishedPapers.Where(pa => pa.userID == userToDelete.id).ToList();
                _context.RemoveRange(publishedPapers);

                List<int> paperIDs = new List<int>();

                foreach (var pp in publishedPapers)
                    paperIDs.Add(pp.paperID);

                var papers = _context.ResearchPaper.Where(pp => paperIDs.Contains(pp.ID)).ToList();

                _context.RemoveRange(papers);

                _context.Remove(userToDelete);

                await _context.SaveChangesAsync();
                //here we have no more research papers, or this author,
                //nor roles, nor asp users associated with our user.
            }


            return RedirectToAction("Index", "ResearchPaper");
        }

        //method for mail sending
        public async static void Email(string receiverMail, string htmlMessage)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("researchub6969@gmail.com");
                message.To.Add(new MailAddress("apetrovic1@etf.unsa.ba"));
                message.Subject = "Daily report";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlMessage;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("researchub6969@gmail.com", "researchub123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception) { }
        }

        public string Report(User user)
        {
            var colabs = _context.Collaborations.Where(colab => colab.collaborateeID == user.id).ToList();
            string report = "Here is your daily report: <br /> Collaborations: <br />";

            foreach(var colab in colabs)
            {
                var collaborator = GetAspNetUser((int)colab.collaboratorID, _context.User.ToList(), _userManager.Users.ToList());
                if (collaborator == null) continue;
                report += " - User '" + collaborator.UserName + "' wants to collaborate on some project with you! <br />";
            }


            var ratings = _context.Ratings.Where(rating => rating.receiverID == user.id).ToList();
            report += "Ratings: <br />";

            foreach(var rat in ratings)
            {
                var giverAsp = GetAspNetUser((int)rat.giverID, _context.User.ToList(), _userManager.Users.ToList());
                if (giverAsp == null) continue;
                var researchPaper = _context.ResearchPaper.Where(rp => rp.ID == rat.paperID).ToList().FirstOrDefault();
                if (researchPaper == null) continue;
                report += " - User '" + giverAsp.UserName + "' gave you rating of " + rat.rating + " for you research paper called '" + researchPaper.title + "' <br />";                
            }

            var requests = _context.Requests.Where(req=> req.requesteeID == user.id).ToList();
            report += "Requests: <br />";

            foreach (var rat in requests)
            {
                var giverAsp = GetAspNetUser((int)rat.requesterID, _context.User.ToList(), _userManager.Users.ToList());
                if (giverAsp == null) continue;
                report += " - User '" + giverAsp.UserName + "' says: '" + rat.requestBody + "' <br />";
            }

            report += "<h4>Cheers mate!</h4> <br />";
            return report;
        }

        //GET User/ReportProblem

        public async Task<IActionResult> ReportProblem()
        {
            return View("ReportProblem");
        }

        //GET User/ReportProblem

        public async Task<IActionResult> DailyReport()
        {
            var currentAspUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUser = GetNormalUser(currentAspUser.Id, _context.User.ToList());

            var htmlBody = Report(currentUser);
            Email(currentAspUser.UserName, htmlBody);

            return View("SuccessReport", new Tuple<string>(htmlBody));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Moderator")]
        //POST User/ReportProblem2
        public async Task<IActionResult> ReportProblem(string? username, string? error)
        {
            //find requestee id - the one who we want to go out with
            var selectedUsers = _userManager.Users.ToList().Where(usr => usr.UserName == username).ToList();
            User user = null;

            if (selectedUsers.Count == 0)
            {
                ErrorViewModel model = new ErrorViewModel();
                model.RequestId = "Searched colleague username does not exist";
                return View("Error", model);
            }
            else
            {
                user = GetNormalUser(selectedUsers.First().Id, _context.User.ToList());
                var aspUser = GetAspNetUser(user.id, _context.User.ToList(), _userManager.Users.ToList());
                var moderatorID = 24;

                Requests request = new Requests();

                request.requesterID = moderatorID;
                request.requesteeID = user.id;
                request.timeRequestMade = DateTime.Now;
                request.requestBody = error;

                _context.Add(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "ResearchPaper");    
         
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

        
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.id == id);
        }
    }
}
