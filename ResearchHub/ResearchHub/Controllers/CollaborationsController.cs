using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class CollaborationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        

        public CollaborationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class DummyInput {
            public DummyInput()
            {

            }

            [EmailAddress]
            [Display(Name ="Enter username of desired colleague:")]
            public string username { get; set; }
        }

        [BindProperty]
        public DummyInput Username { get; set; }

        // GET: Collaborations
        public async Task<IActionResult> Index()
        {
            Username = new DummyInput();
            Username.username = "";
            return View(Username);
        }

        // GET: Collaborations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collaborations = await _context.Collaborations
                .Include(c => c.collaboratee)
                .Include(c => c.collaborator)
                .FirstOrDefaultAsync(m => m.id == id);
            if (collaborations == null)
            {
                return NotFound();
            }

            return View(new Tuple <Collaborations, string>(collaborations, Username.username));
        }

        // GET: Collaborations/Create
        public IActionResult Create()
        {
            ViewData["collaborateeID"] = new SelectList(_context.User, "id", "id");
            ViewData["collaboratorID"] = new SelectList(_context.User, "id", "id");
            ViewData["Title"] = "ma ja ba";
            Username.username = "";
            return View(Username);
        }

        // POST: Collaborations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,collaboratorID,collaborateeID,timeRequestMade")] Collaborations collaborations, string myUsername)
        {
            //find collaboratee id - the one who we want to collaborate with
            var selectedUsers = _userManager.Users.ToList().Where(usr => usr.UserName == myUsername).ToList();
            var collaborateeID = 0;

            if (selectedUsers.Count == 0)
            {
                ErrorViewModel model = new ErrorViewModel();
                model.RequestId = "Searched colleague username does not exist";
                return View("Error", model);
            } else
            {
                collaborateeID = UserController.GetNormalUser(selectedUsers.First().Id, _context.User.ToList()).id;
            }

            //get current user normal ID
            var currentAspUser = await _userManager.GetUserAsync(HttpContext.User);
            var collaboratorID = UserController.GetNormalUser(currentAspUser.Id, _context.User.ToList()).id;

            Collaborations colab = new Collaborations();

            colab.collaboratorID = collaboratorID;
            colab.collaborateeID = collaborateeID;
            colab.timeRequestMade = DateTime.Now;

            _context.Add(colab);
            await _context.SaveChangesAsync();
            
            return View("Index", new DummyInput()
            {
                username = "Collaboration request made successfully!"
            });
        }

        // GET: Collaborations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collaborations = await _context.Collaborations.FindAsync(id);
            if (collaborations == null)
            {
                return NotFound();
            }
            ViewData["collaborateeID"] = new SelectList(_context.User, "id", "id", collaborations.collaborateeID);
            ViewData["collaboratorID"] = new SelectList(_context.User, "id", "id", collaborations.collaboratorID);
            return View(collaborations);
        }

        // POST: Collaborations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,collaboratorID,collaborateeID,timeRequestMade")] Collaborations collaborations)
        {
            if (id != collaborations.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collaborations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollaborationsExists(collaborations.id))
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
            ViewData["collaborateeID"] = new SelectList(_context.User, "id", "id", collaborations.collaborateeID);
            ViewData["collaboratorID"] = new SelectList(_context.User, "id", "id", collaborations.collaboratorID);
            return View(collaborations);
        }

        // GET: Collaborations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collaborations = await _context.Collaborations
                .Include(c => c.collaboratee)
                .Include(c => c.collaborator)
                .FirstOrDefaultAsync(m => m.id == id);
            if (collaborations == null)
            {
                return NotFound();
            }

            return View(collaborations);
        }

        // POST: Collaborations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collaborations = await _context.Collaborations.FindAsync(id);
            _context.Collaborations.Remove(collaborations);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollaborationsExists(int id)
        {
            return _context.Collaborations.Any(e => e.id == id);
        }
    }
}
