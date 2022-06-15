using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public ResearchPaperController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
            _userManager = userManager;
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
            var authors = " ";

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

            var pubPaper = _context.PublishedPapers.Where(pp => pp.paperID == researchPaper.ID).ToList().FirstOrDefault();
            var datePublished = pubPaper.datePublished.ToString();
            var aspNetID = _context.User.Where(usr => usr.id == pubPaper.userID).ToList().FirstOrDefault().aspNetID;

            authors = _userManager.Users.Where(usr => usr.Id == aspNetID).ToList().FirstOrDefault().UserName;

            var userss = _userManager.Users.ToList();
            var tableUsers = _context.User.ToList();

            var paperAuthorRows = _context.PaperAuthor.Where(pa => pa.paperID == researchPaper.ID).ToList();

            foreach (var row in paperAuthorRows)
            {
                aspNetID = tableUsers.Where(usr => usr.id == row.authorID).ToList().FirstOrDefault().aspNetID;
                authors += ", " + userss.Where(usr => usr.Id == aspNetID).ToList().FirstOrDefault().UserName;
            }
            
            var tuple = new Tuple<ResearchPaper, string, string, string, string>(researchPaper, paperTypes, paperTopics, authors, datePublished);

            return View("Details", tuple);
        }


        // GET: ResearchPaper/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // GET: ResearchPaper/Search/query
        public async Task<IActionResult> Search(string? query)
        {
            var acceptedPapers = new List<Tuple<ResearchPaper, string>>();

            //Separating words of our search
            List<string> words = query.Split(" ").ToList();



            //searching for words in abstract, types, topics, authors, or title:
            //authors - implement this later.
            
            var users = _context.User.ToList();
            var allPapers = _context.ResearchPaper.ToList();
            var publishedPapers = _context.PublishedPapers.ToList();
            var paperAuthors = _context.PaperAuthor.ToList();
            var allUsr = _userManager.Users.ToList();

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
                        var myID = publishedPapers.Where(pp => pp.paperID == paper.ID).ToList().FirstOrDefault().userID;
                        var aspNetID = users.Where(usr => usr.id == myID).ToList().FirstOrDefault().aspNetID;
                        string name = allUsr.Where(usr => usr.Id == aspNetID).ToList().FirstOrDefault().UserName;

                        acceptedPapers.Add(new Tuple<ResearchPaper, string>(paper, name));
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
            var acceptedPapers = new List<Tuple<ResearchPaper, string>>();
            var allPapers = _context.ResearchPaper.ToList();

            //paper needs to have at least one word in its description 
            //from whole input data, to be classified as accepted

            List<string> titleWords = title == null ? null : title.ToLower().Split(" ").ToList();
            List<string> typeWords = types == null ? null : types.ToLower().Split(" ").ToList();
            List<string> topicWords = topics == null ? null : topics.ToLower().Split(" ").ToList();
            List<string> authorWords =  authors == null ? null :  authors.ToLower().Split(" ").ToList();
            List<string> keyWords = keywords == null ? null :  keywords.ToLower().Split(" ").ToList();

            var users = _context.User.ToList();
            var publishedPapers = _context.PublishedPapers.ToList();
            var paperAuthors = _context.PaperAuthor.ToList();
            var allUsr = _userManager.Users.ToList();

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

                List<string> authorsUsernames = new List<string>();
                

                foreach (var user in _userManager.Users)
                {
                    var normalID = users.Where(usr => usr.aspNetID == user.Id).ToList().FirstOrDefault().id;
                    
                    if ((publishedPapers.Where(pp => pp.paperID == paper.ID && pp.userID == normalID).ToList().Count > 0) || (paperAuthors.Where(pp => pp.paperID == paper.ID && pp.authorID == normalID).ToList().Count > 0))
                        authorsUsernames.Add(user.UserName);
                }

                
                if (ContainsAny(paper.title, titleWords) || ContainsAny(paperTypes, typeWords) || ContainsAny(paperTopics, topicWords) || ContainsAny(paper.paperAbstract, keyWords) || ContainsAny(authors, authorsUsernames))
                {
                    var myID = publishedPapers.Where(pp => pp.paperID == paper.ID).ToList().FirstOrDefault().userID;
                    var aspNetID = users.Where(usr => usr.id == myID).ToList().FirstOrDefault().aspNetID;
                    string name = allUsr.Where(usr => usr.Id == aspNetID).ToList().FirstOrDefault().UserName;
                    
                    acceptedPapers.Add(new Tuple<ResearchPaper, string>(paper, name));                    
                }
            }

            return View("Display", acceptedPapers);
        }

        // POST: ResearchPaper/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,title,rating,paperAbstract,hasPdf,pdfFileUrl, pdfFile")] ResearchPaper researchPaper, string[] types, string[] topics, string usernames)
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

                //because of foreign key constraint.
                await _context.SaveChangesAsync();

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

                //Get current user, so we know who is marked as publisher
                IdentityUser user = await _userManager.GetUserAsync(HttpContext.User);

                //Get all users, to be able to find authors of this paper
                var allUsers = _userManager.Users.ToList();


                foreach (var user1 in allUsers)
                {
                    if (usernames.ToLower().Contains(user1.UserName.ToLower()))
                    {
                        PaperAuthor paperAuthor = new PaperAuthor();
                        paperAuthor.authorID = _context.User.Where(us => us.aspNetID == user1.Id).ToList().First().id;
                        paperAuthor.paperID = researchPaper.ID;
                        _context.Add(paperAuthor);
                    }
                }

                PublishedPapers publishedPaper = new PublishedPapers();
                publishedPaper.userID = _context.User.Where(us => us.aspNetID == user.Id).ToList().First().id;
                publishedPaper.paperID = researchPaper.ID;
                publishedPaper.datePublished = DateTime.Now;

                _context.Add(publishedPaper);

                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Quiz",new { id=researchPaper.ID });
            }

            return View(researchPaper);
        }

        // GET: ResearchPaper/Edit/5
        [Authorize]
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
        [Authorize] //Also you can edit only your research papers
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
        [Authorize]
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
        [Authorize]
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
