using Konscious.Security.Cryptography;
using LAN_File_Sharing_Application.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LAN_File_Sharing_Application.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseContext db;
        private const int _MemorySize = 65536;
        private const int _DegreeOfParallelism = 2;
        private const int _Iteration = 4;
        private const int _HashSize = 32;
        private const int _SaltSize = 16;


        public UserController(DatabaseContext _context)
        {
            db = _context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            var check = await db.Users.FirstOrDefaultAsync(q => q.UserName == model.UserName);
            if (check != null)
            {
                ModelState.AddModelError("", "User Already Exists");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var salt = CreateSalt();

                    var BucketID = Guid.NewGuid();

                    var newBucket = new Bucket
                    {
                        Id = Guid.NewGuid(),
                        AccountID = Guid.NewGuid(),
                    };
                    var newAcc = new UserAccount
                    {
                        Id = newBucket.AccountID,
                        EmailAddress = model.EmailAddress,
                        UserName = model.UserName,
                        PasswordHash = HashPassword(model.Password, salt),
                        PasswordSalt = Convert.ToHexString(salt),
                        BucketID = newBucket.Id,
                    };


                    try
                    {
                        db.Users.Add(newAcc);
                        db.Buckets.Add(newBucket);
                        await db.SaveChangesAsync();
                        ModelState.Clear();
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError(e.ToString(), "Please enter unique email or Password");
                    }
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await db.Users.FirstOrDefaultAsync(q => q.EmailAddress == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found ");
                }
                else if (user != null)
                {
                    byte[] salt = Convert.FromHexString(user.PasswordSalt);
                    if (!VerifyPassword(model.Password, salt, user.PasswordHash))
                    {
                        ModelState.AddModelError("", "Password was incorrect");
                    }
                    else
                    {

                        var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name,user.EmailAddress),
                        new Claim("Name",user.UserName),
                        new Claim(ClaimTypes.Role,"User")
                        };
                        var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(model);
        }
        public IActionResult Forbidden()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        private string HashPassword(string password, byte[] salt)
        {
            byte[] passwordByte = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordByte)
            {
                Salt = salt,
                MemorySize = _MemorySize,
                DegreeOfParallelism = _DegreeOfParallelism,
                Iterations = _Iteration
            };
            byte[] hash = argon2.GetBytes(_HashSize);
            return Convert.ToHexString(hash);
        }
        private byte[] CreateSalt()
        {
            return RandomNumberGenerator.GetBytes(_SaltSize);
        }

        private bool VerifyPassword(string password, byte[] salt, string passwordhash)
        {

            string hash = HashPassword(password, salt);

            return hash == passwordhash;
        }
    }
}