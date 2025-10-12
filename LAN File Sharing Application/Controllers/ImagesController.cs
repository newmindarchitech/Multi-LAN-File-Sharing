using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LAN_File_Sharing_Application.Models;
using Microsoft.EntityFrameworkCore;
namespace LAN_File_Sharing_Application.Controllers
{
    [Authorize(Policy ="UserAuth")]
    
    public class ImagesController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _environment;
        public ImagesController(DatabaseContext database,IWebHostEnvironment ev) {
            this._db = database;
            this._environment = ev;
        }
        [Route("{id:guid}/Images")]
        [HttpGet]
        public IActionResult Index(Guid id)
        {
            var list=_db.Images.Where(i=>i.FolderID==id).OrderByDescending(o=>o.ImageName).ToList();
            return View(list);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(Guid id,ImageSubmitForm model)
        {
            var check=_db.Images.FirstOrDefaultAsync(i=>i.ImageName==model.File.FileName);
            if (check == null) {
                var newImage = new Images
                {
                    ID = Guid.NewGuid(),
                    ImageName = model.File.FileName,
                    FolderID = id
                };
                try
                {
                    _db.Images.Add(newImage);
                    await _db.SaveChangesAsync();
                    var findFolder=_db.UserFolders.FirstOrDefault(i=>i.ID==id);
                    string newImagePath = Path.Combine(_environment.WebRootPath + "/UserFolders/"+ findFolder.FolderName,model.File.FileName);
                    newImagePath += Path.GetExtension(model.File!.FileName);
                    using (var stream = System.IO.File.Create(newImagePath))
                    {
                        model.File.CopyTo(stream);
                    }
                        ModelState.Clear();
                    return RedirectToAction("Index");
                }catch(DbUpdateException e)
                {
                    e.ToString();
                }
            }
            else
            {
                ModelState.AddModelError("", "Image already exists");
            }
            return View(model);
        }
    }
}
