using FlexSpace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexSpace.Controllers
{
    // 🌟 [Authorize] 確保連進來的人「至少有登入」
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 老闆的後台儀表板
        public async Task<IActionResult> Dashboard()
        {
            if (User.Identity?.Name != "admin@glowup.com")
            {
                TempData["ErrorMessage"] = "⛔ 權限不足";
                return RedirectToAction("Index", "Home");
            }

            var allBookings = await _context.Bookings
                .Include(b => b.BeautyService)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            // 🌟 計算統計指標
            ViewBag.TotalRevenue = allBookings.Sum(b => b.TotalPrice);
            ViewBag.TotalBookings = allBookings.Count;
            ViewBag.AvgOrderValue = allBookings.Any() ? allBookings.Average(b => b.TotalPrice) : 0;

            // 🌟 準備圓餅圖數據：按服務類別分組
            var categoryStats = allBookings
                .Where(b => b.BeautyService != null)
                .GroupBy(b => b.BeautyService.Category)
                .Select(g => new {
                    Category = g.Key,
                    Total = g.Sum(b => b.TotalPrice)
                })
                .ToList();

            // 將數據轉為前端 JavaScript 容易讀取的格式
            ViewBag.ChartLabels = categoryStats.Select(x => x.Category).ToArray();
            ViewBag.ChartData = categoryStats.Select(x => x.Total).ToArray();

            return View(allBookings);
        }

        // ==========================================
        // 1. 服務項目管理列表
        // ==========================================
        public async Task<IActionResult> ManageServices()
        {
            if (User.Identity?.Name != "admin@glowup.com") return RedirectToAction("Index", "Home");

            // 抓出所有美容服務
            var services = await _context.BeautyServices.ToListAsync();
            return View(services);
        }

        // ==========================================
        // 2. 新增服務畫面 (GET)
        // ==========================================
        [HttpGet]
        public IActionResult CreateService()
        {
            if (User.Identity?.Name != "admin@glowup.com") return RedirectToAction("Index", "Home");
            return View();
        }

        // ==========================================
        // 3. 處理新增服務邏輯 (POST)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CreateService(BeautyService model)
        {
            if (User.Identity?.Name != "admin@glowup.com") return RedirectToAction("Index", "Home");

            // 移除不需在前端驗證的關聯屬性 (避免 ModelState.IsValid 失敗)
            ModelState.Remove("Bookings");
            ModelState.Remove("OpeningHours");
            ModelState.Remove("ProviderId");

            if (ModelState.IsValid)
            {
                // 1. 設定預設值並存入資料庫
                model.ProviderId = "SystemAdmin";
                model.BeauticianId = 1; // 預設指派給第一位美容師

                _context.BeautyServices.Add(model);
                await _context.SaveChangesAsync(); // 先存檔，這樣 model 才會獲得資料庫配發的 Id！

                // 🌟 2. 自動化魔法：幫這項新服務建立「週一到週六營業、週日公休」的時間表
                for (int day = 0; day <= 6; day++)
                {
                    var openingHour = new OpeningHour
                    {
                        BeautyServiceId = model.Id, // 綁定剛剛新增的服務
                        DayOfWeek = (DayOfWeek)day,
                        IsClosed = (day == 0), // DayOfWeek.Sunday 是 0
                        OpenTime = new TimeSpan(10, 0, 0), // 早上 10 點
                        CloseTime = new TimeSpan(21, 0, 0) // 晚上 9 點
                    };
                    _context.OpeningHours.Add(openingHour);
                }

                await _context.SaveChangesAsync(); // 再次存檔保存營業時間

                TempData["SuccessMessage"] = $"成功新增美容服務：{model.Name}！";
                return RedirectToAction("ManageServices");
            }

            return View(model);
        }

        // ==========================================
        // 🌟 新增：老闆專屬的「強制取消預約」功能
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            // 1. 確認真的是老闆在操作
            if (User.Identity?.Name != "admin@glowup.com")
            {
                TempData["ErrorMessage"] = "⛔ 權限不足";
                return RedirectToAction("Index", "Home");
            }

            // 2. 去資料庫找這筆訂單 (不用像前台一樣核對 UserId，因為老闆有最高權限)
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

            if (booking != null)
            {
                // 3. 刪除該筆訂單，釋放時段
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ 已成功由後台為客人取消該筆預約，時段已重新釋出！";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ 取消失敗：找不到該筆預約紀錄。";
            }

            // 4. 操作完畢後，重新整理回儀表板
            return RedirectToAction("Dashboard");
        }
    }
}