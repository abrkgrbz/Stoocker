using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Response
{
    public class TenantStatsResponse
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;

        // Kullanıcı istatistikleri
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }

        // Rol istatistikleri
        public int TotalRoles { get; set; }
        public int ActiveRoles { get; set; }

        // Aktivite istatistikleri
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastUserCreatedAt { get; set; }
        public int LoginsThisMonth { get; set; }

        // Tarih bilgileri
        public DateTime CreatedAt { get; set; }
        public int DaysSinceCreation { get; set; }

        // Trend verileri
        public List<MonthlyStats> MonthlyUserGrowth { get; set; } = new();
        public List<MonthlyStats> MonthlyLoginStats { get; set; } = new();
    }

    public class MonthlyStats
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal PercentageChange { get; set; }
    }
}
