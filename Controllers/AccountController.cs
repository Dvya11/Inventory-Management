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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var role = HttpContext.Session.GetString("UserRole")?.ToLower();
                if (role == "user")
                    return RedirectToAction("Dashboard", "User");

                return RedirectToAction("Index", "Product");
            }

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

            try
            {
                var normalizedEmail = email.Trim().ToLower();
                var enteredPassword = password.Trim();

                var user = _context.Users
                    .FirstOrDefault(u => u.Email != null && u.Email.Trim().ToLower() == normalizedEmail);

                if (user == null || string.IsNullOrWhiteSpace(user.Password) || user.Password.Trim() != enteredPassword)
                {
                    ViewBag.Error = "Invalid login credentials";
                    return View();
                }

                var role = string.IsNullOrWhiteSpace(user.Role) ? "user" : user.Role.Trim().ToLower();

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", role);

                if (role == "user")
                    return RedirectToAction("Dashboard", "User");

                return RedirectToAction("Index", "Product");
            }
            catch (Exception)
            {
                ViewBag.Error = "Database error occurred while login.";
                return View();
            }
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
        [ValidateAntiForgeryToken]
        public IActionResult Register(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Email and Password are required";
                    return View();
                }

                var normalizedEmail = email.Trim().ToLower();
                var normalizedPassword = password.Trim();

                if (_context.Users.Any(u => u.Email != null && u.Email.Trim().ToLower() == normalizedEmail))
                {
                    ViewBag.Error = "Email is already registered";
                    return View();
                }

                var newUser = new User
                {
                    Email = normalizedEmail,
                    Password = normalizedPassword,
                    Role = "user",
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                TempData["Success"] = "Registration Successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                ViewBag.Error = "Database error occurred. Make sure your database is updated/migrated.";
                return View();
            }
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

            var normalizedEmail = email.Trim().ToLower();
            var user = _context.Users.FirstOrDefault(u => u.Email != null && u.Email.Trim().ToLower() == normalizedEmail);
            if (user == null)
            {
                ViewBag.Message = "Email not found!";
                return View();
            }

            // Generate reset token
            var token = Guid.NewGuid().ToString();

            // Example reset link
            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = token },
                Request.Scheme
            );

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

            TempData["Success"] = "Password reset successful!";
            return RedirectToAction("Login");
        }
    }
}