using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DocumentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        readonly private IUserRepository _userRepo;
        readonly private IDocumentShareRepository _documentShareRepo;
        private readonly INotificationRepository _notificationRepository;
        
        public UserController(IUserRepository _userRepo, 
            IDocumentShareRepository _documentShareRepo,
            INotificationRepository notificationRepository) {
            this._userRepo = _userRepo;
            this._documentShareRepo = _documentShareRepo;
            this._notificationRepository = notificationRepository;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _userRepo.GetByEmail(user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(user);
                }
                _userRepo.Add(user);
                if (user.Role == UserRole.Admin) 
                    return RedirectToAction("Index", "Admin");
                else if (user.Role == UserRole.User) 
                    return RedirectToAction("UserIndex", "User");
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and Password are required.";
                return View();
            }

            var user = _userRepo.GetByEmail(email);

            if (user != null && user.Password == password)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());

                if (user.Role == UserRole.Admin)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("UserIndex", "User");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }



        [HttpGet]
        public IActionResult UserIndex(string searchTerm = "")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _userRepo.GetWithDocuments(userId.Value);

            if (user == null) return RedirectToAction("Login");

            var sharedDocuments = _documentShareRepo.GetSharedWithUser(userId.Value);
            var sharedDocumentsViewModel = sharedDocuments.Select(ds => new SharedDocumentViewModel
            {
                Id = ds.Document.Id,
                Title = ds.Document.Title,
                Description = ds.Document.Description,
                FileName = ds.Document.FileName,
                FilePath = ds.Document.FilePath,
                FileType = ds.Document.FileType,
                OwnerName = ds.Document.Owner?.FullName,
                OwnerEmail = ds.Document.Owner?.Email,
                CategoryName = ds.Document.Category?.Name,
                CreatedAt = ds.Document.CreatedAt,
                SharedAt = ds.SharedAt,
                IsShared = true
            }).ToList();

            ViewBag.Name = HttpContext.Session.GetString("UserName");
            ViewBag.SharedDocuments = sharedDocumentsViewModel;
            ViewBag.SearchTerm = searchTerm;
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var filteredDocuments = user.Documents.Where(d => d.Title.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase)).ToList();
                return View(filteredDocuments);
            }
            
            return View(user.Documents);
        }

        [HttpGet]
        public IActionResult ShowNotification()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var notifications = _notificationRepository.GetAllWithUser(userId.Value);
            ViewBag.Name = HttpContext.Session.GetString("UserName");
            return View(notifications);
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _userRepo.Get(userId.Value);
            if (user == null) return RedirectToAction("Login");

            var model = new ProfileUpdateViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };

            ViewBag.Name = HttpContext.Session.GetString("UserName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(ProfileUpdateViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Name = HttpContext.Session.GetString("UserName");
                return View(model);
            }

            var user = _userRepo.Get(userId.Value);
            if (user == null) return RedirectToAction("Login");

            if (user.Email != model.Email)
            {
                var existingUser = _userRepo.GetByEmail(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Email is already registered to another user.");
                    ViewBag.Name = HttpContext.Session.GetString("UserName");
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to change password.");
                    ViewBag.Name = HttpContext.Session.GetString("UserName");
                    return View(model);
                }

                if (user.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    ViewBag.Name = HttpContext.Session.GetString("UserName");
                    return View(model);
                }
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.Password = model.NewPassword;
            }

            _userRepo.Update(user);

            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);

            var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "User")
                return RedirectToAction("UserIndex", "User");
            else
                return RedirectToAction("Index", "Admin");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
