using Microsoft.AspNetCore.Mvc;
using LAN_File_Sharing_Application.Models;

namespace LAN_File_Sharing_Application.Controllers
{
    public class FolderController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _env;

        public FolderController(DatabaseContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Hiển thị danh sách folder
        public IActionResult Index()
        {
            var folders = _db.UserFolders.ToList();
            return View(folders);
        }

        // Hiển thị form upload ảnh
        [HttpGet]
        public IActionResult Upload(Guid id)
        {
            var folder = _db.UserFolders.Find(id);
            if (folder == null)
                return NotFound();

            ViewBag.Folder = folder;
            return View();
        }

        // Upload ảnh vào folder
        [HttpPost]
        public async Task<IActionResult> Upload(Guid id, List<IFormFile> images)
        {
            var folder = _db.UserFolders.Find(id);
            if (folder == null)
                return NotFound();

            string uploadPath = Path.Combine(_env.WebRootPath, "uploads", folder.FolderName);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var img in images)
            {
                string filePath = Path.Combine(uploadPath, img.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                var image = new Images
                {
                    FileName = img.FileName,
                    FilePath = Path.Combine("uploads", folder.FolderName, img.FileName),
                    FolderID = folder.ID,
                    UploadedAt = DateTime.Now
                };
                _db.Images.Add(image);
            }

            await _db.SaveChangesAsync();
            TempData["Message"] = "Đã tải lên thành công!";
            return RedirectToAction("ViewImages", new { id = folder.ID });
        }

        // Xem ảnh trong folder
        public IActionResult ViewImages(Guid id)
        {
            var folder = _db.UserFolders.Find(id);
            if (folder == null)
                return NotFound();

            var images = _db.Images.Where(i => i.FolderID == id).ToList();
            ViewBag.Folder = folder;
            return View(images);
        }
    }
}
