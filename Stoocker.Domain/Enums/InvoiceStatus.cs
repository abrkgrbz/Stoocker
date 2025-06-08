using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,          // Taslak
        Pending = 2,        // Beklemede
        Approved = 3,       // Onaylandı
        Shipped = 4,        // Sevk edildi
        Delivered = 5,      // Teslim edildi
        PartiallyPaid = 6,  // Kısmen ödendi
        Paid = 7,           // Ödendi
        Cancelled = 8,      // İptal
        Returned = 9        // İade
    }
}
