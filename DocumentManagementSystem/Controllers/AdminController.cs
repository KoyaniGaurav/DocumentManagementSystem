using DocumentManagementSystem.Models;
using DocumentManagementSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IDocumentShareRepository _documentShareRepo;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationRepository _notificationRepo;

        public AdminController(
            IDocumentRepository documentRepo, 
            IUserRepository userRepo, 
            IDocumentShareRepository documentShareRepo,
            IWebHostEnvironment env,
            INotificationRepository notificationRepo)
        {
            _documentRepo = documentRepo;
            _userRepo = userRepo;
            _documentShareRepo = documentShareRepo;
            _env = env;
            _notificationRepo = notificationRepo;
        }

        public IActionResult Index(string searchTerm = "")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userId == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var adminDocuments = _documentRepo.GetDocumentsByOwnerId(userId.Value);
            
            // If search term is provided, filter documents by title
            if (!string.IsNullOrEmpty(searchTerm))
            {
                adminDocuments = adminDocuments.Where(d => d.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            var adminDocumentsViewModel = adminDocuments.Select(d => new AdminIndexViewmodel
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                FileName = d.FileName,
                FileType = d.FileType,
                CategoryName = d.Category?.Name,
                CreatedAt = d.CreatedAt,
            }).ToList();

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

            ViewBag.SharedDocuments = sharedDocumentsViewModel;
            ViewBag.Name = HttpContext.Session.GetString("UserName");
            ViewBag.SearchTerm = searchTerm;
            return View(adminDocumentsViewModel);
        }

        public IActionResult ShareDocument(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userId == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var document = _documentRepo.GetDocumentWithOwner(id);
            if (document == null || document.OwnerId != userId.Value)
            {
                return NotFound();
            }

            var availableUsers = _documentShareRepo.GetUsersForSharing(userId.Value);
            
            var model = new DocumentShareViewModel
            {
                DocumentId = document.Id,
                DocumentTitle = document.Title,
                DocumentOwner = document.Owner?.FullName,
                AvailableUsers = availableUsers
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShareDocument(DocumentShareViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            if (userId == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            if(model.SelectedUserIds.Count == 0)
            {
                model.AvailableUsers = _documentShareRepo.GetUsersForSharing(userId.Value);
                ModelState.AddModelError("SelectedUserIds", "Select atleast one user to share document.");
            }
            if (!ModelState.IsValid)
            {
                model.AvailableUsers = _documentShareRepo.GetUsersForSharing(userId.Value);
                return View(model);
            }

            var document = _documentRepo.GetById(model.DocumentId);
            if (document == null || document.OwnerId != userId.Value)
            {
                return NotFound();
            }

            foreach (var shareWithUserId in model.SelectedUserIds)
            {
                if (!_documentShareRepo.IsDocumentSharedWithUser(model.DocumentId, shareWithUserId))
                {
                    var documentShare = new DocumentShare
                    {
                        DocumentId = model.DocumentId,
                        ShareWithUserId = shareWithUserId,
                        ShareByUserId = userId.Value,
                        SharedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var notification = new Notification
                    {
                        Title = "Document Shared",
                        Description = "Document shared to you by " + userName + " and document is " + document.Title,
                        UserId = shareWithUserId,
                        documentId = document.Id
                    };
                    _notificationRepo.Add(notification);
                    _documentShareRepo.Add(documentShare);
                }
            }

            document.IsShared = true;
            _documentRepo.Update(document);

            TempData["SuccessMessage"] = "Document shared successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult ManageSharedDocuments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var sharedDocuments = _documentShareRepo.GetSharedByUser(userId.Value);

            var sharedDocumentsViewModel = sharedDocuments
                .GroupBy(ds => ds.Document)
                .Select(g => new AdminDocumentViewModel
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    Description = g.Key.Description,
                    FileName = g.Key.FileName,
                    FileType = g.Key.FileType,
                    CreatedAt = g.Key.CreatedAt,
                    SharedWithUserNames = g.Select(x => x.ShareWithUser?.FullName ?? "Unknown User").ToList(),
                    SharedWithUsers = g.Select(x => x.ShareWithUser).ToList()
                }).ToList();

            ViewBag.Name = HttpContext.Session.GetString("UserName");
            return View(sharedDocumentsViewModel);
        }


        public IActionResult RemoveShare(int documentId, int shareWithUserId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                var documentShare = _documentShareRepo.GetAll()
                    .FirstOrDefault(ds => ds.DocumentId == documentId &&
                                          ds.ShareWithUserId == shareWithUserId &&
                                          ds.ShareByUserId == userId.Value);

                if (documentShare != null)
                {
                    _notificationRepo.RemoveByUserAndDocument(shareWithUserId, documentId);
                    TempData["SuccessMessage"] = "Document share removed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Share not found or you don't have permission to remove it.";
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while removing the share. Please try again.";
            }

            return RedirectToAction("ManageSharedDocuments");
        }


        public IActionResult RemoveAllShares(int documentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null || userRole != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                var documentShares = _documentShareRepo.GetAll()
                    .Where(ds => ds.DocumentId == documentId && ds.ShareByUserId == userId.Value)
                    .ToList();

                if (documentShares.Any())
                {
                    foreach (var share in documentShares)
                    {
                        _documentShareRepo.Delete(share.Id);
                    }
                    _notificationRepo.RemoveByDocument(documentId);

                    var document = _documentRepo.GetById(documentId);
                    if (document != null)
                    {
                        document.IsShared = false;
                        _documentRepo.Update(document);
                    }

                    TempData["SuccessMessage"] = "All shares for this document have been removed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "No shares found for this document.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while removing shares. Please try again.";
            }

            return RedirectToAction("ManageSharedDocuments");
        }

    }
}
