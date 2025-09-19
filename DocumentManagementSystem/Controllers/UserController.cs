using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        readonly private IUserRepository _userRepo;
        public UserController(IUserRepository _userRepo) {
            this._userRepo = _userRepo;
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
                if (user.Role == UserRole.Admin) RedirectToAction("AdminIndex", "User", new {name = user.FullName,email = user.Email} );
                else if (user.Role == UserRole.User) RedirectToAction("UserIndex", "User", new { name = user.FullName, email = user.Email });
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
                    return RedirectToAction("AdminIndex", "User");
                else
                    return RedirectToAction("UserIndex", "User");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }


        [HttpGet]
        public IActionResult AdminIndex(string name,string email)
        {
            ViewBag.Name = name;
            return View();
        }

        [HttpGet]
        public IActionResult UserIndex()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _userRepo.GetWithDocuments(userId.Value);

            if (user == null) return RedirectToAction("Login");

            ViewBag.Name = HttpContext.Session.GetString("UserName");
            return View(user.Documents);
        }



    }
}
