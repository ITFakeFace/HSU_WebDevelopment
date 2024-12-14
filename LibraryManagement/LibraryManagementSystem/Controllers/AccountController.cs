using LibraryManagementSystem.Helper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.AuthenticationModels;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly LibraryDbContext _ctx;
        private static string GeneratedOTP = "";
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IEmailSender emailSender, LibraryDbContext ctx)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _ctx = ctx;
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
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                    return View(model);
                }
                var user = new User
                {
                    Pid = model.PId,
                    PhoneNumber = model.Phone,
                    UserName = model.UserName,
                    Email = model.Email,
                    Status = 1,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
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

        public async Task<IActionResult> RequestConfirmEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("email không hợp lệ.");
            }

            var user = await _signInManager.UserManager.GetUserAsync(User);
            // Gửi email xác thực
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>link</a>");


            // Lấy ba ký tự đầu
            var emailPrefix = email.Substring(0, 3);

            // Xác định vị trí của ký tự @
            int atIndex = email.IndexOf('@');
            if (atIndex == -1)
            {
                return BadRequest("email không hợp lệ.");
            }

            // Tạo chuỗi ẩn bằng dấu *
            var maskedEmail = emailPrefix + new string('*', atIndex - 3) + email.Substring(atIndex);

            ViewBag.MaskedEmail = maskedEmail;
            return View();
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Id != id && !_userManager.GetRolesAsync(user).Result.Contains("ADMINISTRATOR"))
            {
                return RedirectToAction("AccessDenied");
            }
            AddressService addrService = new AddressService(_ctx);

            var userProfileModel = new UserProfileModel
            {
                UserName = user.UserName,
                CoverAvatar = user.CoverAvatar,
                Avatar = user.Avatar,
                Gender = user.Gender,
                Address = await addrService.GetAddressString(user.Address),
                Dob = user.Dob,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Fullname = user.Fullname,
                Pid = user.Pid,
                Status = user.Status
            };
            return View(userProfileModel);
        }

        [HttpPost]
        public async Task<String> SendOTP([FromBody] EmailConfirmRequest request)
        {
            Console.WriteLine($"\n\n{request.Email}\n\n");
            // Tạo mã OTP ngẫu nhiên
            var random = new Random();
            GeneratedOTP = random.Next(100000, 999999).ToString(); // Tạo OTP 6 chữ số

            // Gửi OTP qua email
            try
            {
                await _emailSender.SendEmailAsync(request.Email, "Confirm your email", GeneratedOTP);
                return JsonConvert.SerializeObject(new ResponseHandler<string>
                {
                    IsSuccess = true,
                    StatusCode = "200",
                    Message = "OTP Success",
                    Data = ""
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ResponseHandler<string>
                {
                    IsSuccess = false,
                    StatusCode = "500",
                    Message = "OTP Failed",
                    Data = ""
                });
            }
        }

        [HttpPost]
        public async Task<String> CheckOTP([FromBody] EmailConfirmRequest request)
        {
            try
            {
                Console.WriteLine($"\n\n{request.OTP}  {GeneratedOTP}\n\n");

                return JsonConvert.SerializeObject(new ResponseHandler<string>
                {
                    IsSuccess = request.OTP == GeneratedOTP,
                    StatusCode = request.OTP == GeneratedOTP ? "200" : "500",
                    Message = "OTP",
                    Data = ""
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new ResponseHandler<string>
                {
                    IsSuccess = false,
                    StatusCode = "500",
                    Message = "OTP False",
                    Data = ""
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile inpAvatar)
        {
            if (inpAvatar == null || inpAvatar.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một file ảnh hợp lệ.";
                return RedirectToAction("Profile");
            }

            try
            {
                // Kiểm tra loại file hợp lệ
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(inpAvatar.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Error"] = "Định dạng file không hợp lệ. Chỉ hỗ trợ JPG, PNG, GIF.";
                    return RedirectToAction("Profile");
                }

                // Đọc file vào mảng byte (blob)
                using var memoryStream = new MemoryStream();
                await inpAvatar.CopyToAsync(memoryStream);
                var avatarBlob = memoryStream.ToArray();

                // Lấy thông tin người dùng hiện tại
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng.";
                    return RedirectToAction("Login");
                }

                // Cập nhật Avatar
                user.Avatar = avatarBlob;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Avatar đã được cập nhật thành công.";
                }
                else
                {
                    TempData["Error"] = "Đã xảy ra lỗi khi cập nhật avatar.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi không mong muốn.";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePersonalInfo(UserProfileModel profileModel)
        {
            var user = await _ctx.Users.Where(u => u.Id == _signInManager.UserManager.GetUserId(User)).FirstOrDefaultAsync();
            user.Pid = profileModel.Pid;
            user.Gender = profileModel.Gender;
            user.Dob = profileModel.Dob;
            user.Fullname = profileModel.Fullname;
            await _signInManager.UserManager.UpdateAsync(user);
            return RedirectToAction("Profile", new { id = _userManager.GetUserId(User) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(UserProfileModel profileModel)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            Console.WriteLine($"\n\n{profileModel.OTP}  {GeneratedOTP}\n\n");
            Console.WriteLine($"\n\n{profileModel.NewEmail}\n\n");
            if (profileModel.NewEmail == null)
            {
                return RedirectToAction("Profile", new { id = _userManager.GetUserId(User) });
            }
            if (profileModel.OTP.Trim() == GeneratedOTP.Trim())
            {
                Console.WriteLine("\n\nOTP Confirmed\n\n");
                user.Email = profileModel.NewEmail;
                user.EmailConfirmed = true;
                Console.WriteLine($"\n\n{user.Email}\n\n");
                await _signInManager.UserManager.UpdateAsync(user);
                return RedirectToAction("Profile", new { id = _userManager.GetUserId(User) });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhone(UserProfileModel profileModel)
        {
            var user = await _ctx.Users.Where(u => u.Id == _signInManager.UserManager.GetUserId(User)).FirstOrDefaultAsync();
            user.PhoneNumber = profileModel.NewPhone;
            await _signInManager.UserManager.UpdateAsync(user);
            return RedirectToAction("Profile", new { id = _userManager.GetUserId(User) });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            return RedirectToAction("ChangePassword", new { email = model.Email });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(ForgotPasswordModel model)
        {
            var random = new Random();
            GeneratedOTP = random.Next(100000, 999999).ToString(); // Tạo OTP 6 chữ số

            var user = await _userManager.FindByEmailAsync(model.Email);

            await _emailSender.SendEmailAsync(user.Email, "Change Password", GeneratedOTP);

            // Lấy ba ký tự đầu
            var emailPrefix = user.Email.Substring(0, 3);

            // Xác định vị trí của ký tự @
            int atIndex = user.Email.IndexOf('@');
            if (atIndex == -1)
            {
                return BadRequest("Email không hợp lệ.");
            }

            // Tạo chuỗi ẩn bằng dấu *
            var maskedEmail = emailPrefix + new string('*', atIndex - 3) + user.Email.Substring(atIndex);

            ViewBag.MaskedEmail = maskedEmail;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (model.ConfirmPassword == model.NewPassword && model.OTP == GeneratedOTP)
                {
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, model.NewPassword);
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }
    }
}
