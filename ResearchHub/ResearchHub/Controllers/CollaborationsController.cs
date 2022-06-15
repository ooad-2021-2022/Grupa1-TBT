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
    public class CollaborationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CollaborationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Collaborations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Collaborations.Include(c => c.collaboratee).Include(c => c.collaborator);
            return View(await applicationDbContext.ToListAsync());
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

            return View(collaborations);
        }

        // GET: Collaborations/Create
        public IActionResult Create()
        {
            ViewData["collaborateeID"] = new SelectList(_context.User, "id", "id");
            ViewData["collaboratorID"] = new SelectList(_context.User, "id", "id");
            return View();
        }

        // POST: Collaborations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,collaboratorID,collaborateeID,timeRequestMade")] Collaborations collaborations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collaborations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["collaborateeID"] = new SelectList(_context.User, "id", "id", collaborations.collaborateeID);
            ViewData["collaboratorID"] = new SelectList(_context.User, "id", "id", collaborations.collaboratorID);
            return View(collaborations);
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
