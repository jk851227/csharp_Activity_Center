using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ActivityCenter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private ActivityCenterContext db;
        public HomeController(ActivityCenterContext context)
        {
            db = context;
        }

        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser)
        {

            
            if(ModelState.IsValid == false)
            {
                
                return View("Index");
            }

            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already Exists");
                    return View("Index");
                }
                
                
            }

            Regex letters = new Regex(@"[a-zA-Z]");
            Regex hasNumber = new Regex(@"[0-9]");
            Regex hasSpecialChar = new Regex(@"[\W]");

            bool isValidated = letters.IsMatch(newUser.Password) && hasNumber.IsMatch(newUser.Password) && hasSpecialChar.IsMatch(newUser.Password);

            if (!isValidated)
            {
                ModelState.AddModelError("Password", "Password must contain at least 1 number, 1 special character and upper and lowercase letters");
                return View("Index");
            }


            

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("UserName", newUser.FirstName);

            return RedirectToAction("Dashboard");
        }

        [HttpPost("/login")]
        public IActionResult Login(LoginUser loginUser)
        {
            string loginError = "Invalid Email or Password";
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            User dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);

            if (dbUser == null)
            {
                ModelState.AddModelError("LoginEmail", loginError);
                return View("Index");
            }

            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            PasswordVerificationResult pwCompareResult = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

            if (pwCompareResult == 0)
            {
                ModelState.AddModelError("LoginPassword", loginError);
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("UserName", dbUser.FirstName);

            return RedirectToAction("Dashboard");
        }

        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            List<Models.Activity> allActivities = db.Activities
                .Include(a => a.Coordinator)
                .Include(a => a.Participants)
                .ThenInclude(p => p.Activity)
                .OrderBy(a => a.Date)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            return View("Dashboard", allActivities);
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
