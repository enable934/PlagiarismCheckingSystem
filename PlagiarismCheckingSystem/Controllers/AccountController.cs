using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlagiarismCheckingSystem.Data;
using PlagiarismCheckingSystem.Models;
using PlagiarismCheckingSystem.Services;
using PlagiarismCheckingSystem.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Controllers
{
    public class AccountController : Controller
    {
        private UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.GetUserByEmailAndLogin(model.Email, model.Password) != null)
                {
                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "LaboratoryWorks");
                }
                ModelState.AddModelError("", "Неправильні логін та(або) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.GetUser(model.Email) == null)
                {
                    _userService.Register(model);

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "LaboratoryWorks");
                }
                else
                    ModelState.AddModelError("", "Неправильні логін та(або) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
