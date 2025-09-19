using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IWebHostEnvironment _env;

        public DocumentController(IDocumentRepository documentRepo, IUserRepository userRepo, IWebHostEnvironment env)
        {
            _documentRepo = documentRepo;
            _userRepo = userRepo;
            _env = env;
        }

        // GET: Create document
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create document
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                ModelState.AddModelError("", "You must be logged in to upload documents.");
                return View(model);
            }

            if (model.File != null && model.File.Length > 0)
            {
                var fileName = Path.GetFileName(model.File.FileName);
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                var document = new Document
                {
                    Title = model.Title,
                    Description = model.Description,
                    FileName = fileName,
                    FilePath = "/uploads/" + fileName,
                    FileType = Path.GetExtension(fileName),
                    OwnerId = userId.Value
                };

                _documentRepo.Add(document);
                return RedirectToAction("UserIndex", "User");
            }

            ModelState.AddModelError("", "File upload failed.");
            return View(model);
        }

        // GET: Edit document
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            var model = new DocumentEditViewModel
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description
            };
            return View(model);
        }

        // POST: Edit document
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var document = _documentRepo.GetById(model.Id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            document.Title = model.Title;
            document.Description = model.Description;

            if (model.File != null && model.File.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                // Delete old file
                var oldFilePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);

                // Save new file
                var fileName = Path.GetFileName(model.File.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                document.FileName = fileName;
                document.FilePath = "/uploads/" + fileName;
                document.FileType = Path.GetExtension(fileName);
            }

            _documentRepo.Update(document);
            return RedirectToAction("UserIndex", "User");
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            return View(document);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            _documentRepo.Delete(document.Id);
            return RedirectToAction("UserIndex", "User");
        }
    }
}
