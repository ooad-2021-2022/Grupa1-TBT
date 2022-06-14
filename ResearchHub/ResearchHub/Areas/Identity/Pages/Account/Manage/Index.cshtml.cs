using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResearchHub.Data;
using ResearchHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ResearchHub.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private bool validLoading;

        //injecting dbcontext with some other functionalities
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            validLoading = true;
            AlreadyUsedTopics = new List<int>();
        }

        public class DummyUser
        {
            public DummyUser()
            {

            }
            [StringLength(maximumLength: 100, MinimumLength = 2)]
            [Display(Name = "First name")]
            public string firstName { get; set; }

            [StringLength(maximumLength: 100, MinimumLength = 2)]
            [Display(Name = "Last name")]
            public string lastName { get; set; }
            [StringLength(maximumLength: 100, MinimumLength = 2)]
            [Display(Name = "Address")]
            public string address { get; set; }
            [DataType(DataType.Date)]
            [Display(Name = "Date of birth")]
            public DateTime dateOfBirth { get; set; }
        }

        public string Username { get; set; }

        public List<int> AlreadyUsedTopics { get; set; }

        [BindProperty]
        public DummyUser TableUser { get; set; }
        
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            validLoading = false;
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //loading additional user info based on returned current user
            var firstUser = FindUser(user);
            TableUser = new DummyUser();

            if (firstUser != null)
            {
                validLoading = true;
                TableUser.firstName = firstUser.firstName;
                TableUser.lastName = firstUser.lastName;
                TableUser.address = firstUser.address;
                TableUser.dateOfBirth = firstUser.dateOfBirth;
            } else
            {
                return;
            }

            //we need to retrieve all userskills from UserSkills table:
            var userSkillsList = _context.UserSkills.Where(skill => skill.userID == firstUser.id).ToList();
            if (userSkillsList == null)
                userSkillsList = new List<UserSkills>();

            foreach (var skill in userSkillsList)
            {
                AlreadyUsedTopics.Add(skill.skill);
            }

            //all current skills will be overwritten with new results, so we need to delete 'em
            _context.UserSkills.RemoveRange(userSkillsList);
            await _context.SaveChangesAsync();

            Username = userName; //basically email.

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public User FindUser(IdentityUser user)
        {
            if (user == null)
                return null;

            var users = _context.User.Where(usr => usr.aspNetID == user.Id).ToList();

            if (users.Count > 0)
                return users.First();
            return null;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            if (!validLoading)
            {
                //simulation of error
                var errorViewModel = new ErrorViewModel();
                errorViewModel.RequestId = "There is AspNet User, but there is no User. Aborting...";
                return null;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string[] fields)
        {            
            var user = await _userManager.GetUserAsync(User);
            var firstUser = FindUser(user);

            //memorizing all changes that happened before posting data
            firstUser.firstName = TableUser.firstName;
            firstUser.lastName = TableUser.lastName;
            firstUser.address = TableUser.address;
            firstUser.dateOfBirth = TableUser.dateOfBirth;
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            //save new TableUser, and new topics:
            foreach(var strField in fields)
            {
                int skill = Convert.ToInt32(strField);
                var newSkill = new UserSkills();
                newSkill.userID = firstUser.id;
                newSkill.skill = skill;
                _context.UserSkills.Add(newSkill);
            }

            
            _context.Update(firstUser);
            await _context.SaveChangesAsync();
            
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
