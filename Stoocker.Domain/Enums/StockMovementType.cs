using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Enums
{
    public enum StockMovementType
    {
        InitialStock = 1,      // İlk stok girişi
        Purchase = 2,          // Satın alma girişi
        Sale = 3,              // Satış çıkışı
        Transfer = 4,          // Depo transferi
        Adjustment = 5,        // Manuel düzeltme
        Return = 6,            // İade girişi
        Loss = 7,              // Fire/Kayıp çıkışı
        Production = 8,        // Üretim girişi
        Count = 9,             // Sayım düzeltmesi
        Reservation = 10,      // Rezervasyon
        UnReservation = 11,    // Rezervasyon iptali
        Damage = 12,           // Hasar çıkışı
        Expired = 13,          // Son kullanma tarihi geçen
        Assembly = 14,         // Montaj/Demontaj
        Gift = 15              // Hediye/Promosyon
    }
}
