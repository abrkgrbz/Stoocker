using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class ProductStock : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public int CurrentStock { get; set; } = 0;
        public int ReservedStock { get; set; } = 0;
        public int IncomingStock { get; set; } = 0; // Sipariş verilen ancak henüz gelmeyen
        public int AvailableStock => CurrentStock - ReservedStock;
        public decimal AverageCost { get; set; } = 0; // FIFO/LIFO hesaplaması için
        public decimal LastPurchasePrice { get; set; } = 0;
        public DateTime LastMovementDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastCountDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastSaleDate { get; set; }
         
        public int? MinimumStock { get; set; }
        public int? MaximumStock { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; } = null!;
        public virtual Warehouse Warehouse { get; set; } = null!;
    }

}
