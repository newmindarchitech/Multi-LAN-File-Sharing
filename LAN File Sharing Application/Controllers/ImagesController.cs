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
        public IActionResult Index(Guid id)
        {
            TempData["id"] = id;
            TempData["Folder"] = id;
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
            if (model.File == null)
            {
                ModelState.AddModelError("", "An Image is required");
            }
            else
            {
                var folderid = TempData["id"];
                    var newImage = new Images
                    {
                        ID = Guid.NewGuid(),
                        ImageName = model.File.FileName,
                        FolderID = Guid.Parse(folderid.ToString()),
                    };
                    try
                    {
                        _db.Images.Add(newImage);
                        await _db.SaveChangesAsync();
                        var findFolder = _db.UserFolders.FirstOrDefault(i => i.ID == id);
                        string newImagePath = Path.Combine(_environment.WebRootPath + "/UserFolders/", findFolder.FolderName, model.File.FileName);
                        using (var stream = System.IO.File.Create(newImagePath))
                        {
                            model.File.CopyTo(stream);
                        }
                        ModelState.Clear();
                        return RedirectToAction("Index", "Images", new {id=id});
                    }
                    catch (DbUpdateException e)
                    {
                        e.ToString();
                    }
            }
            return View(model);
        }

        public IActionResult Delete(Guid id) {

            var delete=_db.Images.Find(id);
            var holder2 = delete.FolderID;
            var holder = TempData["Folder"];
            var findfolder = _db.UserFolders.FirstOrDefault(id => id.ID == Guid.Parse(holder.ToString()));
            string folderName = findfolder.FolderName;
            try
                {
                string imagepath = Path.Combine(_environment.WebRootPath + "/UserFolders/", folderName, delete.ImageName);
                System.IO.File.Delete(imagepath);
                _db.Images.Remove(delete);
                _db.SaveChanges();
            }
                catch (DbUpdateException e) { 
                    e.ToString() ;
                }
            
            return RedirectToAction("Index", "Images", new {id=holder2});
        }
    }
}
