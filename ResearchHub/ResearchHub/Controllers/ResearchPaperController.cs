using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ResearchHub.Data;
using ResearchHub.Models;

namespace ResearchHub.Controllers
{
    public class ResearchPaperController : Controller
    {
        private List<ResearchType> researchTypes = new List<ResearchType>();
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ResearchPaperController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
        }

        // GET: ResearchPaper
        public async Task<IActionResult> Index()
        {
            return View(await _context.ResearchPaper.ToListAsync());
        }
        
        // Helpers: for displaying names of enumerations
        public static string GetTypeName(ResearchType researchType)
        {
            return researchType.GetType().GetMember(researchType.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>()?.GetName();
        }
        public static string GetTopicName(ResearchTopic researchTopic)
        {
            return researchTopic.GetType().GetMember(researchTopic.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>()?.GetName();
        }

        // GET: ResearchPaper/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var researchPaper = await _context.ResearchPaper
                .FirstOrDefaultAsync(m => m.ID == id);
            if (researchPaper == null)
            {
                return NotFound();
            }

            //get needed information for detailed display of paper:
            var allTopics = _context.ResearchTopicsPaper.ToList();
            var allTypes = _context.PaperType.ToList();

            var paperTopics = " ";
            var paperTypes = " ";

            foreach (var topic in allTopics)
            {
                if (topic.paperID == researchPaper.ID)
                {
                    paperTopics += ", " + GetTopicName((ResearchTopic)topic.topic);
                }
            }

            if (paperTopics.Length > 2)
            paperTopics = paperTopics.Substring(2);

            foreach (var type in allTypes)
            {
                if (type.paperID == researchPaper.ID)
                {
                    paperTypes += ", " + GetTypeName((ResearchType)type.type);
                }
            }

            if (paperTypes.Length > 2)
                paperTypes = paperTypes.Substring(2);
            
            var tuple = new Tuple<ResearchPaper, string, string>(researchPaper, paperTypes, paperTopics);

            return View("Details", tuple);
        }


        // GET: ResearchPaper/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: ResearchPaper/Search/query
        public async Task<IActionResult> Search(string? query)
        {
            List<ResearchPaper> acceptedPapers = new List<ResearchPaper>();

            //Separating words of our search
            List<string> words = query.Split(" ").ToList();

            
            
            //searching for words in abstract, types, topics, authors, or title:
            //authors - implement this later.

            var allPapers = _context.ResearchPaper.ToList();

            foreach (var paper in allPapers)
            {
                //preprocess types and topics for current research paper
                var allTopics = _context.ResearchTopicsPaper.ToList();
                var allTypes = _context.PaperType.ToList();

                var paperTopics = "";
                var paperTypes = "";

                foreach (var topic in allTopics)
                {
                    if (topic.paperID == paper.ID)
                    {
                        paperTopics += ((ResearchTopic)topic.topic).ToString();
                    }
                }

                foreach (var type in allTypes)
                {
                    if (type.paperID == paper.ID)
                    {
                        paperTypes += ((ResearchType)type.type).ToString();
                    }
                }

                foreach(var word in words)
                {                 
                    if ((paper.title != null && paper.title.ToLower().Contains(word.ToLower()) || (paper.paperAbstract != null && paper.paperAbstract.ToLower().Contains(word.ToLower())) || paperTopics.ToLower().Contains(word.ToLower()) || paperTypes.ToLower().Contains(word.ToLower())))
                    {
                        acceptedPapers.Add(paper);
                        break;
                    }                        
                }
            }
       
            return View("Display", acceptedPapers);
        }

        //helper function
        private bool ContainsAny(string originalString, List<string> substrings)
        {
            if (originalString == null || substrings == null)
                return false;
            foreach(var substring in substrings)
            {
                if (originalString.ToLower().Contains(substring.ToLower()))
                    return true;
            }
            return false;
        }

        public async Task<IActionResult> SearchDetailed(string? title, string? types, string? topics, string? authors, string? keywords)
        {
            var acceptedPapers = new List<ResearchPaper>();
            var allPapers = _context.ResearchPaper.ToList();

            //paper needs to have at least one word in its description 
            //from whole input data, to be classified as accepted

            List<string> titleWords = title == null ? null : title.ToLower().Split(" ").ToList();
            List<string> typeWords = types == null ? null : types.ToLower().Split(" ").ToList();
            List<string> topicWords = topics == null ? null : topics.ToLower().Split(" ").ToList();
            List<string> authorWords =  authors == null ? null :  authors.ToLower().Split(" ").ToList();
            List<string> keyWords = keywords == null ? null :  keywords.ToLower().Split(" ").ToList();

            foreach (var paper in allPapers)
            {
                //preprocess types and topics for current research paper
                var allTopics = _context.ResearchTopicsPaper.ToList();
                var allTypes = _context.PaperType.ToList();

                var paperTopics = "";
                var paperTypes = "";

                foreach (var topic in allTopics)
                {
                    if (topic.paperID == paper.ID)
                    {
                        paperTopics += ((ResearchTopic)topic.topic).ToString();
                    }
                }

                foreach (var type in allTypes)
                {
                    if (type.paperID == paper.ID)
                    {
                        paperTypes += ((ResearchType)type.type).ToString();
                    }
                }

                //needs to be checked for authors also, but that'll be implemented later.
                if (ContainsAny(paper.title, titleWords) || ContainsAny(paperTypes, typeWords) || ContainsAny(paperTopics, topicWords) || ContainsAny(paper.paperAbstract, keyWords))
                {
                    acceptedPapers.Add(paper);                    
                }
            }

            return View("Display", acceptedPapers);
        }

        // POST: ResearchPaper/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,title,rating,paperAbstract,hasPdf,pdfFileUrl, pdfFile")] ResearchPaper researchPaper, string[] types, string[] topics)
        {   
            if (ModelState.IsValid)
            {
                //upload pdf
                if (researchPaper.pdfFile != null)
                {
                    
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(researchPaper.pdfFile.FileName);
                    string extension = Path.GetExtension(researchPaper.pdfFile.FileName);
                    var imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    researchPaper.pdfFileUrl = Path.Combine(wwwRootPath + "\\Papers\\", fileName);
                    using (var fileStream = new FileStream(researchPaper.pdfFileUrl, FileMode.Create))
                    {
                        await researchPaper.pdfFile.CopyToAsync(fileStream);
                    }
                    researchPaper.hasPdf = true;
                } else
                {
                    researchPaper.hasPdf = false;
                }

                //upload row to database
                researchPaper.rating = 0;
                _context.Add(researchPaper);                

                //add research topics and types to ResearchTopicsPaper and PaperType
                foreach(string type in types)
                {
                    PaperType paperType = new PaperType();
                    paperType.paperID = researchPaper.ID;
                    paperType.type = Convert.ToInt32(type);
                    paperType.ResearchPaper = researchPaper; // not gonna be in database

                    _context.Add(paperType);
                }
                
                foreach(string topic in topics)
                {
                    ResearchTopicsPaper researchTopic = new ResearchTopicsPaper();
                    researchTopic.paperID = researchPaper.ID;
                    researchTopic.topic = Convert.ToInt32(topic);
                    researchTopic.ResearchPaper = researchPaper;

                    _context.Add(researchTopic);
                }

                //PaperAuthor, PublishedPapers - next time.

                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Quiz",new { id=researchPaper.ID });
            }

            return View(researchPaper);
        }

        // GET: ResearchPaper/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var researchPaper = await _context.ResearchPaper.FindAsync(id);
            if (researchPaper == null)
            {
                return NotFound();
            }
            return View(researchPaper);
        }

        // POST: ResearchPaper/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,title,rating,paperAbstract,hasPdf,pdfFileUrl")] ResearchPaper researchPaper)
        {
            if (id != researchPaper.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(researchPaper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResearchPaperExists(researchPaper.ID))
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
            return View(researchPaper);
        }

        // GET: ResearchPaper/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var researchPaper = await _context.ResearchPaper
                .FirstOrDefaultAsync(m => m.ID == id);
            if (researchPaper == null)
            {
                return NotFound();
            }

            return View(researchPaper);
        }

        // POST: ResearchPaper/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //deleting research paper from ResearchPaper
            var researchPaper = await _context.ResearchPaper.FindAsync(id);
            _context.ResearchPaper.Remove(researchPaper);
            

            //deleting pdf that is uploaded, if one exists
            if (System.IO.File.Exists(researchPaper.pdfFileUrl))
                System.IO.File.Delete(researchPaper.pdfFileUrl);

            //deleting topics from ResearchTopicsPaper, where we have specific ID
            var topicsToRemove = new List<ResearchTopicsPaper>();
            var allTopics = _context.ResearchTopicsPaper.ToList();

            foreach (var topic in allTopics)
            {
                if (topic.paperID == researchPaper.ID)
                    topicsToRemove.Add(topic);
            }

            _context.ResearchTopicsPaper.RemoveRange(topicsToRemove);


            //deleting types from PaperType - where specific ID is encountered

            var rowsToRemove = new List<PaperType>();
            var allTypeRows = _context.PaperType.ToList();

            foreach(var row in allTypeRows)
                if (row.paperID == researchPaper.ID)
                    rowsToRemove.Add(row);

            _context.PaperType.RemoveRange(rowsToRemove);

            //Deleting appropriate quiz with all of its questions:
            //One paper has only one quiz
            var allQuizes = _context.Quiz.ToList();
            var quizToRemove = new Quiz();

            foreach(var quiz in allQuizes)
            {
                if (quiz.researchPaperID == researchPaper.ID)
                {
                    quizToRemove = quiz;
                    break;
                }
                    
            }

            _context.Quiz.Remove(quizToRemove);

            //deleting questions that come with this quiz:
            var allQuestions = _context.Questions.ToList();
            var questionsToRemove = new List<Questions>();

            foreach(var question in allQuestions)
            {
                if (question.quizID == quizToRemove.ID)
                {
                    questionsToRemove.Add(question);
                }
            }

            _context.Questions.RemoveRange(questionsToRemove);


            //PaperAuthors, publishedPapers - next time.
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResearchPaperExists(int id)
        {
            return _context.ResearchPaper.Any(e => e.ID == id);
        }
    }
}
