using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult(List<T> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items.AsReadOnly();
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        
       
    }

    public class PagedRequest
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public string? SearchTerm { get; set; }
    }
}
