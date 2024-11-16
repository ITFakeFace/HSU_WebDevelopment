using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.AuthenticationModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm user bằng tên người dùng hoặc email
            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail) ??
                       await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng.");
                return View(model);
            }
            else
            {
                Console.WriteLine(user.Email);
            }

            // Đăng nhập
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng.");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Fullname = model.Fullname
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Gửi email xác thực
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>link</a>");

                    // Đăng nhập ngay sau khi đăng ký
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("RequestConfirmEmail", "Account", new { email = model.Email });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmEmailSuccess");
            }
            return View("Error");
        }

        public IActionResult RequestConfirmEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email không hợp lệ.");
            }

            // Lấy ba ký tự đầu
            var emailPrefix = email.Substring(0, 3);

            // Xác định vị trí của ký tự @
            int atIndex = email.IndexOf('@');
            if (atIndex == -1)
            {
                return BadRequest("Email không hợp lệ.");
            }

            // Tạo chuỗi ẩn bằng dấu *
            var maskedEmail = emailPrefix + new string('*', atIndex - 3) + email.Substring(atIndex);

            ViewBag.MaskedEmail = maskedEmail;
            return View();
        }


        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [ValidateAntiForgeryToken]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }
    }
}
