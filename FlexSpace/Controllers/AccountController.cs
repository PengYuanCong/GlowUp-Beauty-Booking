using FlexSpace.ViewModels; // 確保有引入你的表單模型
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlexSpace.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // 透過建構子注入 Identity 服務
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ==========================================
        // 1. 註冊功能 (Register)
        // ==========================================

        // 這個 [HttpGet] 是用來「顯示」註冊畫面的，沒有它就會 404！
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 這個 [HttpPost] 是用來接收使用者填好的表單並存入資料庫的
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 註冊成功後自動幫他登入，然後跳轉回首頁
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // 如果密碼太弱或 Email 重複，會把錯誤訊息顯示在畫面上
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // ==========================================
        // 2. 登入功能 (Login)
        // ==========================================

        // 用來「顯示」登入畫面的
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 接收登入表單並核對帳號密碼
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "登入失敗，請檢查帳號密碼。");
            }
            return View(model);
        }

        // ==========================================
        // 3. 登出功能 (Logout)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}