using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResearchHub.Data;
using ResearchHub.Models;

namespace ResearchHub.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quiz
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Quiz.Include(q => q.ResearchPaper);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Quiz/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz
                .Include(q => q.ResearchPaper)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        public async Task<IActionResult> StartQuiz(int paperID, double rating)
        {
            if (rating < 0 || rating > 10)
            {
                ErrorViewModel errorViewModel = new ErrorViewModel();
                errorViewModel.RequestId = "Bad rating value (must be double between 0 and 10)";
                return View("Error", errorViewModel);
            }
            //get all questions that are relevant for this quiz
            int quizID = _context.Quiz.ToList().Find(q => q.researchPaperID == paperID).ID;
            List<Questions> questions = _context.Questions.ToList().FindAll(question => question.quizID == quizID);

            var tuple = new Tuple<List<Questions>, int, double>(questions, paperID, rating);

            return View("Display", tuple);
        }

        public async Task<IActionResult> CheckAnswers(int paperID, double rating, string[] labeledQuestions) 
        {
            //all questions that are labeled, are ones that we think have positive answer 
            var quiz = _context.Quiz.ToList().Find(q => q.researchPaperID == paperID);
            List<Questions> questions = _context.Questions.ToList().FindAll(question => question.quizID == quiz.ID);

            int numberOfQuestions = questions.Count;
            var trueQuestions = questions.Where(question => question.answer == true).ToList();
            var falseQuestions = questions.Where(question => question.answer == false).ToList();


            var unlabeledQuestions = questions.Where(question => !labeledQuestions.Contains(question.question)).ToList();
            quiz.numberOfCorrectAnswers = 0;

            foreach (string question in labeledQuestions)
            {
                if (trueQuestions.Find(qu => qu.question == question) != null)
                    quiz.numberOfCorrectAnswers++;
            }
            
            foreach (var quest in unlabeledQuestions)
            {
                if (falseQuestions.Find(q => q.question == quest.question) != null)
                    quiz.numberOfCorrectAnswers++;
            }


            double maxPoints = numberOfQuestions * quiz.pointsPerQuestion;
            double pointsMade = quiz.numberOfCorrectAnswers * quiz.pointsPerQuestion;
            double percentage = (pointsMade / pointsMade) * 100;

           if (percentage >= quiz.minimumScoreNeeded)
           {
                var researchPaper = _context.ResearchPaper.ToList().Find(rp => rp.ID == paperID);
                if (researchPaper.rating == null)
                    researchPaper.rating = 0;
                if (researchPaper.numberOfRatings == null)
                    researchPaper.numberOfRatings = 0;

                researchPaper.rating = researchPaper.rating*(researchPaper.numberOfRatings/((double)researchPaper.numberOfRatings + 1.00)) + rating/(researchPaper.numberOfRatings + 1.00);
                researchPaper.numberOfRatings++;

                _context.Update(researchPaper);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ResearchPaper");
           } else
           {
                ErrorViewModel errorViewModel = new ErrorViewModel();
                errorViewModel.RequestId = "You scored " + pointsMade + " / " + maxPoints + ", but quiz requires minimum number of " + (quiz.minimumScoreNeeded / 100.0) * maxPoints + " points to verify that you have read this paper.";
                return View("Error", errorViewModel);
           }
        }


        // GET: Quiz/Create/researchPaperID
        public IActionResult Create(int id)
        {
            var quizes = _context.Quiz.ToList();
            foreach (var currentQuiz in quizes)
            {
                if (currentQuiz.researchPaperID == id) 
                { 
                    var error = new ErrorViewModel();
                    error.RequestId = "Cannot have more than one quiz for one research paper";
                    //we cannot have multiple quizes for one research paper
                    return View("Error", error);
                }
            }

            var researches = _context.ResearchPaper.ToList();
            var count = 0;
            foreach(var research in researches)
            {
                if (research.ID == id)
                {
                    count++;
                    break;
                }
            }

            if (count == 0)
            {
                var error = new ErrorViewModel();
                error.RequestId = "There is no research paper with specified ID (" + id + ")";
                //we cannot have multiple quizes for one research paper
                return View("Error", error);
            }

            Quiz quiz = new Quiz();
            quiz.researchPaperID = id;
            quiz.pointsPerQuestion = 1;
            quiz.minimumScoreNeeded = 50; //percentages.                       

            return View(quiz);
        }

        // POST: Quiz/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("minimumScoreNeeded,pointsPerQuestion, researchPaperID")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quiz);
                await _context.SaveChangesAsync(); //because of database conflict.

                //Going to question adding controller:

                return RedirectToAction("Create", "Questions", new {id = quiz.ID});
            }
            ViewData["researchPaperID"] = new SelectList(_context.ResearchPaper, "ID", "ID", quiz.researchPaperID);
            return View(quiz);
        }

        // GET: Quiz/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            ViewData["researchPaperID"] = new SelectList(_context.ResearchPaper, "ID", "ID", quiz.researchPaperID);
            return View(quiz);
        }

        // POST: Quiz/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,numberOfCorrectAnswers,minimumScoreNeeded,pointsPerQuestion,researchPaperID")] Quiz quiz)
        {
            if (id != quiz.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.ID))
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
            ViewData["researchPaperID"] = new SelectList(_context.ResearchPaper, "ID", "ID", quiz.researchPaperID);
            return View(quiz);
        }

        // GET: Quiz/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz
                .Include(q => q.ResearchPaper)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        // POST: Quiz/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quiz.FindAsync(id);
            _context.Quiz.Remove(quiz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizExists(int id)
        {
            return _context.Quiz.Any(e => e.ID == id);
        }
    }
}
