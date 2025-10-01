using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IDocumentShareRepository _documentShareRepo;
        private readonly IWebHostEnvironment _env;

        public DocumentController(IDocumentRepository documentRepo, IUserRepository userRepo, IDocumentShareRepository documentShareRepo, IWebHostEnvironment env)
        {
            _documentRepo = documentRepo;
            _userRepo = userRepo;
            _documentShareRepo = documentShareRepo;
            _env = env;
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

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

                int? selectedCategoryId = null;
                if(model.CategoryId.HasValue && model.CategoryId.Value >= 0)
                {
                    selectedCategoryId = model.CategoryId.Value;
                }

                var document = new Document
                {
                    Title = model.Title,
                    Description = model.Description,
                    FileName = fileName,
                    FilePath = "/uploads/" + fileName,
                    FileType = Path.GetExtension(fileName),
                    OwnerId = userId.Value,
                    CategoryId = selectedCategoryId
                };

                _documentRepo.Add(document);

                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "User")
                    return RedirectToAction("UserIndex", "User");
                else
                    return RedirectToAction("Index", "Admin");
            }
            
            ModelState.AddModelError("", "File upload failed.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();


            var model = new DocumentEditViewModel
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                CategoryId = document.CategoryId

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var document = _documentRepo.GetById(model.Id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            document.Title = model.Title;
            document.Description = model.Description;

            int? selectedCategoryId = null;
            if (model.CategoryId.HasValue && model.CategoryId.Value >= 0)
            {
                selectedCategoryId = model.CategoryId.Value;
            }

            if (model.File != null && model.File.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var oldFilePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);

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

            document.CategoryId = selectedCategoryId;

            _documentRepo.Update(document);
            var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "User")
                return RedirectToAction("UserIndex", "User");
            else
                return RedirectToAction("Index", "Admin");
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            return View(document);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            return View(document);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var document = _documentRepo.GetById(id);
            if (document == null || document.OwnerId != userId.Value) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            _documentRepo.Delete(document.Id);
            var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "User")
                return RedirectToAction("UserIndex", "User");
            else
                return RedirectToAction("Index", "Admin");

        }

        [HttpGet]
        public IActionResult ViewShared(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var hasAccess = _documentShareRepo.GetAll()
                .Any(ds => ds.DocumentId == id && ds.ShareWithUserId == userId.Value && ds.IsActive);

            if (!hasAccess)
            {
                return NotFound();
            }

            var document = _documentRepo.GetById(id);
            if (document == null) return NotFound();

            return View("Details", document);
        }

        [HttpGet]
        public IActionResult DownloadShared(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var hasAccess = _documentShareRepo.GetAll()
                .Any(ds => ds.DocumentId == id && ds.ShareWithUserId == userId.Value && ds.IsActive);

            if (!hasAccess) return NotFound();

            var document = _documentRepo.GetById(id);
            if (document == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", document.FileName);
        }
    }
}
