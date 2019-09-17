using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Helpers
{
    public class PageList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public PageList(List<T> items, int count, int pageNum, int pageSize)
        {
            this.TotalPages = (int)(Math.Ceiling((double)count/pageSize));
            this.CurrentPage = pageNum;
            this.PageSize = pageSize;
            this.AddRange(items);
            this.TotalCount = count;
        }

        public static async Task<PageList<T>> CreateAsync(IQueryable<T> items,
        int pageNumber, int pageSize){
            var count = await items.CountAsync();
            var filteredItems = await items.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToListAsync();
            PageList<T> pl = new PageList<T>(filteredItems, count, pageNumber, pageSize);
            return pl;
        }
    }
}