using LAN_File_Sharing_Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace LAN_File_Sharing_Application.Controllers
{
    public class FileItemController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _env;

        public FileItemController(DatabaseContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Danh sách tất cả FileItem
        public IActionResult Index()
        {
            var files = _db.FileItems.ToList();
            return View(files);
        }

        // Tạo tệp tin mới
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TempData["Error"] = "Tên tệp tin không được để trống!";
                return View();
            }

            var file = new FileItem
            {
                FileName = fileName,
                UploadedBy = User.Identity?.Name ?? "Guest",
                CreatedAt = DateTime.Now
            };

            _db.FileItems.Add(file);
            _db.SaveChanges();

            TempData["Message"] = "✅ Tạo tệp tin thành công!";
            return RedirectToAction("Index");
        }

        // Upload ảnh vào FileItem
        [HttpGet]
        public IActionResult UploadImages(int id)
        {
            var file = _db.FileItems.Find(id);
            if (file == null) return NotFound();

            ViewBag.FileItem = file;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages(int id, List<IFormFile> images)
        {
            var file = _db.FileItems.Find(id);
            if (file == null) return NotFound();

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", file.FileName);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var img in images)
            {
                var filePath = Path.Combine(uploadPath, img.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                var image = new Images
                {
                    FileName = img.FileName,
                    FilePath = Path.Combine("uploads", file.FileName, img.FileName),
                    UploadedAt = DateTime.Now,
                    FileItemId = file.Id
                };
                _db.Images.Add(image);
            }

            await _db.SaveChangesAsync();
            TempData["Message"] = $"📸 Đã thêm {images.Count} ảnh vào tệp '{file.FileName}'";
            return RedirectToAction("ViewImages", new { id = file.Id });
        }

        // Xem ảnh trong 1 tệp tin
        public IActionResult ViewImages(int id)
        {
            var file = _db.FileItems.Find(id);
            if (file == null) return NotFound();

            var images = _db.Images.Where(i => i.FileItemId == Guid.Parse(id.ToString())).ToList();
            ViewBag.FileItem = file;
            return View(images);
        }
    }
}
