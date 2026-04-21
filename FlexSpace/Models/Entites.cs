using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static FlexSpace.Models.Booking;

namespace FlexSpace.Models // 記得把 "你的專案名稱" 換成你實際的 Namespace
{
    public class BeautyService
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty; // 類別：美甲、美睫、臉部保養

        public int DurationMinutes { get; set; } // 🌟 新增：服務預計花費分鐘數

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } // 價格

        public string ProviderId { get; set; } = string.Empty; // 美容師/店鋪 ID

        public int? BeauticianId { get; set; }
        public Beautician? Beautician { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<OpeningHour> OpeningHours { get; set; } = new List<OpeningHour>();



    }

    public class OpeningHour
    {
        public int Id { get; set; }

        // 🌟 原本的 VenueId 改成 BeautyServiceId
        public int BeautyServiceId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }

        // 🌟 原本的 Venue 改成 BeautyService
        public BeautyService? BeautyService { get; set; }
    }

    public class Booking
    {
        [Key]
        public Guid Id { get; set; }

        public int BeautyServiceId { get; set; } // 關聯到 BeautyService

        public string UserId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public BeautyService? BeautyService { get; set; } // 導覽屬性

        public class Beautician
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Specialty { get; set; } = string.Empty; // 專長
            public bool IsActive { get; set; } = true;

            // 一個美容師可以負責多項服務
            public ICollection<BeautyService> BeautyServices { get; set; } = new List<BeautyService>();
        }
    }
}
