using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
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
                if (user.Role == UserRole.Admin)
                    return RedirectToAction("AdminIndex", "User", new { name = user.FullName });
                else if (user.Role == UserRole.User)
                    return RedirectToAction("UserIndex", "User", new { name = user.FullName });
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public IActionResult AdminIndex(string name)
        {
            ViewBag.Name = name;
            return View();
        }

        [HttpGet]
        public IActionResult UserIndex(string name)
        {
            ViewBag.Name = name;
            return View();
        }

    }
}
