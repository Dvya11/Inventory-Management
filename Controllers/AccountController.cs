using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        // ================= LOGIN =================

        // GET: Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "admin@gmail.com" && password == "1234")
            {
                TempData["Success"] = "Login Successful!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid Email or Password";
                return View();
            }
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