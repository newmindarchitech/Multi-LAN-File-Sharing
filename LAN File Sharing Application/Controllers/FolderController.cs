using LAN_File_Sharing_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace LAN_File_Sharing_Application.Controllers
{
    [Authorize(Policy = "UserAuth")]
    public class FolderController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _environment;
        public FolderController(DatabaseContext database, IWebHostEnvironment environment) {
            this._db = database;
            this._environment = environment;
        }
        public IActionResult Index()
        {
            var checkbucket = _db.Users.FirstOrDefault(u => u.EmailAddress == User.Identity.Name);
            var listFolders = _db.UserFolders.Where(fo=>fo.BucketID==checkbucket.BucketID ).OrderByDescending(i=>i.FolderName).ToList();
            return View(listFolders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(FolderCreationForm model)
        {
            var checkFolder = await _db.UserFolders.FirstOrDefaultAsync(f => f.FolderName == model.FolderName);
            if (checkFolder == null) {
                var currentuser = await _db.Users.FirstOrDefaultAsync(u => u.EmailAddress == User.Identity.Name);
                if (ModelState.IsValid) {
                    var newFolder = new userFolder
                    {
                        ID = Guid.NewGuid(),
                        FolderName = model.FolderName,
                        Description = model.FolderDescription,
                        IsGlobal=Boolean.Parse(model.IsGlobal),
                        BucketID = currentuser.BucketID
                    };
                    try
                    {
                        _db.UserFolders.Add(newFolder);
                        await _db.SaveChangesAsync();
                        string newfolderPath = Path.Combine(_environment.WebRootPath + "/UserFolders", model.FolderName);
                        Directory.CreateDirectory(newfolderPath);
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException e)
                    {
                        e.ToString();
                    }
                    catch (Exception ex) {
                        ex.ToString();
                    }
                }
                
            }
            else
            {
                ModelState.AddModelError("", "Folder already exists");
            }
            return View(model);
        }

        public IActionResult Edit(Guid id)
        {
            var fill = _db.UserFolders.Find(id);
            if (fill == null)
            {
                return RedirectToAction("Index", "Folder");
            }
            
            
                var FolderForm = new FolderCreationForm
                {
                    FolderName = fill.FolderName,
                    BucketID = fill.BucketID,
                    FolderDescription = fill.Description,
                    IsGlobal= fill.IsGlobal.ToString(),
                    FolderID = fill.ID
                };
            
            return View(FolderForm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FolderCreationForm model)
        {

            var fillFolder = _db.UserFolders.Find(id);
            var folderName = fillFolder.FolderName;
            try
            {
                var editFolder = await _db.UserFolders.FindAsync(id);
                if (editFolder.FolderName != model.FolderName)
                {
                    editFolder.FolderName = model.FolderName;
                    editFolder.Description = model.FolderDescription;
                    editFolder.IsGlobal = Boolean.Parse(model.IsGlobal.ToString());
                    await _db.SaveChangesAsync();
                    string findFolder = Path.Combine(_environment.WebRootPath + "/UserFolders/", folderName);
                    string changeFolderName = Path.Combine(_environment.WebRootPath + "/UserFolders/", editFolder.FolderName);
                    if (Directory.Exists(findFolder))
                    {
                        Directory.Move(findFolder, changeFolderName);
                        return RedirectToAction("Index");
                    }
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    editFolder.FolderName = model.FolderName;
                    editFolder.Description = model.FolderDescription;
                    editFolder.IsGlobal = Boolean.Parse(model.IsGlobal.ToString());
                    await _db.SaveChangesAsync();
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException e) {
                e.ToString();
            }
            return View(model);
        }
        public IActionResult Delete(Guid id) {
            var deleteFolder = _db.UserFolders.Find(id);
            if (deleteFolder == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    List<Images> imagesDeleteAfter=_db.Images.Where(i=>i.FolderID==deleteFolder.ID).ToList();
                    if (imagesDeleteAfter.Count == 0)
                    {
                        _db.UserFolders.Remove(deleteFolder);
                        _db.SaveChanges();
                        string findFolder = Path.Combine(_environment.WebRootPath + "/UserFolders/", deleteFolder.FolderName);
                        string deleteFolderName = Path.Combine(_environment.WebRootPath + "/UserFolders/", deleteFolder.FolderName);
                        if (Directory.Exists(findFolder))
                        {
                            Directory.Delete(deleteFolderName);
                            return RedirectToAction("Index", "Folder");
                        }
                    }
                    else
                    {
                        _db.Images.RemoveRange(imagesDeleteAfter);

                        _db.UserFolders.Remove(deleteFolder);
                        _db.SaveChanges();
                        string findFolder = Path.Combine(_environment.WebRootPath + "/UserFolders/", deleteFolder.FolderName);
                        string deleteFolderName = Path.Combine(_environment.WebRootPath + "/UserFolders/", deleteFolder.FolderName);
                        if (Directory.Exists(findFolder))
                        {
                            Directory.Delete(deleteFolderName, true);
                            return RedirectToAction("Index", "Folder");
                        }
                    }
                }
                catch (DbUpdateException e)
                {
                    e.ToString();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Redirect(Guid id)
        {
            return RedirectToAction("Index", "Images", new { id = id });
        }
        public IActionResult GlobalList()
        {
            var checkbucket = _db.Users.FirstOrDefault(u => u.EmailAddress == User.Identity.Name);
            var listFolders = _db.UserFolders.Where(fo =>  fo.IsGlobal ==true).OrderByDescending(i => i.FolderName).ToList();
            return View(listFolders);
        }
    }
        
}
