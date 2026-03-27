using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LOGIN =================

        // GET: Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Invalid login credentials";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid login credentials";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Dashboard", "User");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ================= REGISTER =================

        // GET: Register Page
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(string email, string password)
        {
            TempData["Success"] = "Registration Successful!";
            return RedirectToAction("Login");
        }

        // ================= FORGOT PASSWORD =================

        // GET: Forgot Password Page
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Email is required!";
                return View();
            }

            // ?? Dummy check (replace with DB later)
            if (email != "admin@gmail.com")
            {
                ViewBag.Message = "Email not found!";
                return View();
            }

            // Generate reset token
            var token = Guid.NewGuid().ToString();

            // ?? Example reset link
            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = token },
                Request.Scheme
            );

            // ?? (Future) Send Email here using SMTP

            // For now just show link on screen (for testing)
            ViewBag.Message = "Password reset link generated!";
            ViewBag.ResetLink = resetLink;

            return View();
        }

        // ================= RESET PASSWORD =================

        // GET: Reset Password Page
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Token = token;
            return View();
        }

        // POST: Reset Password
        [HttpPost]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                ViewBag.Message = "Password cannot be empty!";
                return View();
            }

            // ?? Here you will update password in DB using token

            TempData["Success"] = "Password reset successful!";
            return RedirectToAction("Login");
        }
    }
}