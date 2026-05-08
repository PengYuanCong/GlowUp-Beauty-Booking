using FlexSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization; // 處理權限需要的

namespace FlexSpace.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // 透過建構子注入資料庫上下文
        public HomeController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // 🌟 加入 .Include(s => s.Beautician) 讓系統一併把美容師的名字抓出來！
            var beautyServices = await _context.BeautyServices
                .Include(s => s.Beautician)
                .ToListAsync();

            // 將資料傳給 View (Index.cshtml)
            return View(beautyServices);
        }

        // 預約表單提交的 Action
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitBooking(int BeautyServiceId, DateTime BookingDate, TimeSpan StartTime)
        {
            // 🌟 修正 1：載入美容服務時，使用 Include 一併把「營業時間」抓出來

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge(); // 確保使用者存在


            var beautyService = await _context.BeautyServices
                .Include(s => s.OpeningHours)
                .FirstOrDefaultAsync(v => v.Id == BeautyServiceId);

            if (beautyService == null)
            {
                TempData["ErrorMessage"] = "系統發生錯誤：找不到該美容服務。";
                return RedirectToAction("Index");
            }

            // 🌟 自動計算開始與結束時間
            DateTime startDateTime = BookingDate.Date + StartTime;
            DateTime endDateTime = startDateTime.AddMinutes(beautyService.DurationMinutes);

            if (startDateTime < DateTime.Now)
            {
                TempData["ErrorMessage"] = "時光機還沒發明喔！請選擇未來的日期與時間。";
                return RedirectToAction("Index");
            }


            // 🌟 新增邏輯：檢查營業時間 (判斷店有沒有開)
            var dayOfWeek = startDateTime.DayOfWeek;

            // 從該服務的設定中，找出客人預約的「星期幾」的營業時間
            var openingHour = beautyService.OpeningHours.FirstOrDefault(oh => oh.DayOfWeek == dayOfWeek);

            // 如果找不到設定、或是設定為 IsClosed (店休)、或是預約時間超出了營業範圍
            if (openingHour == null || openingHour.IsClosed ||
                StartTime < openingHour.OpenTime || endDateTime.TimeOfDay > openingHour.CloseTime)
            {
                // 判斷是否為店休，給予不同的錯誤提示
                if (openingHour == null || openingHour.IsClosed)
                {
                    TempData["ErrorMessage"] = $"抱歉，{BookingDate:yyyy-MM-dd} 為本店公休日，請選擇其他日期。";
                }
                else
                {
                    TempData["ErrorMessage"] = $"抱歉，該時段非營業時間。本店該日營業時間為：{openingHour.OpenTime:hh\\:mm} ~ {openingHour.CloseTime:hh\\:mm}";
                }
                return RedirectToAction("Index");
            }

            // 2. 核心邏輯：檢查預約衝突 (有沒有跟別的客人重疊)
            bool hasConflict = await _context.Bookings.AnyAsync(b =>
                b.BeautyServiceId == BeautyServiceId &&
                b.Status == 1 &&
                startDateTime < b.EndTime &&
                endDateTime > b.StartTime);

            if (hasConflict)
            {
                TempData["ErrorMessage"] = "手腳太慢啦！該時段美容師已經被約走了。";
                return RedirectToAction("Index");
            }

            // 3. 建立新的預約紀錄
            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                BeautyServiceId = BeautyServiceId,
                UserId = currentUser.Id,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Status = 1,
                TotalPrice = beautyService.Price // 直接存入服務價格
            };

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            // 4. 成功訊息顯示
            TempData["SuccessMessage"] = $"預約成功！您已保留 {BookingDate:yyyy-MM-dd} 的 {startDateTime:HH:mm} ~ {endDateTime:HH:mm} ({beautyService.Name})。總金額：NT$ {beautyService.Price:N0}";
            return RedirectToAction("Index");
        }


        [Authorize] // 必須登入才能看
        public async Task<IActionResult> MyBookings()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // 抓出屬於這個人的所有預約，並把 BeautyService 的資料一起 Include 進來
            var myBookings = await _context.Bookings
                .Include(b => b.BeautyService)
                .Where(b => b.UserId == currentUser.Id)
                .OrderByDescending(b => b.StartTime) // 最新的預約排在最上面
                .ToListAsync();

            return View(myBookings);
        }

        // 🌟 新增：取消預約功能
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            // 1. 取得目前登入的使用者
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            // 2. 去資料庫尋找這筆預約
            // ⚠️ 安全性防護：必須同時核對 Booking.Id 和 UserId，防止駭客亂刪別人的預約！
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUser.Id);

            if (booking != null)
            {
                // 3. 找到的話就把它從資料庫刪除，藉此釋放該時段
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "已成功取消該筆預約，期待您下次光臨！";
            }
            else
            {
                // 如果找不到，或是這筆訂單根本不是他的
                TempData["ErrorMessage"] = "取消失敗：找不到該筆預約，或您無權限取消。";
            }

            // 4. 重新導向回「我的預約」頁面
            return RedirectToAction("MyBookings");
        }

        // 🌟 新增：視覺化美容師時間表
        // 🌟 進階版：視覺化美容師時間表 (支援彈窗預約)
        public async Task<IActionResult> TimeTable(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today.AddDays(1);
            ViewBag.SelectedDate = selectedDate;

            // 1. 🌟 關鍵修正：必須 Include BeautyServices！
            // 這樣前端的彈窗才能根據不同的美容師，顯示他們專屬的服務項目
            var beauticians = await _context.Beauticians
                .Include(b => b.BeautyServices)
                .ToListAsync();

            var bookingsOnDate = await _context.Bookings
                .Include(b => b.BeautyService)
                .Where(b => b.StartTime.Date == selectedDate.Date)
                .ToListAsync();

            var timeSlots = new List<TimeSpan>();
            for (int hour = 10; hour <= 20; hour++)
            {
                timeSlots.Add(new TimeSpan(hour, 0, 0));
                if (hour < 20)
                {
                    timeSlots.Add(new TimeSpan(hour, 30, 0));
                }
            }

            ViewBag.TimeSlots = timeSlots;
            ViewBag.Bookings = bookingsOnDate;

            return View(beauticians);
        }



        public IActionResult About()
        {
            return View();
        }
    }
}
