using BookApp.Data;
using BookApp.Extensions;
using BookApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookApp.Services;

public class BookFilterOptions
{
    public BooksFilterBy FilterBy { get; set; }
    public string FilterValue { get; set; } = "";
    public OrderByOptions OrderBy { get; set; }
    public int PageNum { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalRows { get; private set; }
    public int TotalPages { get; private set; }

    public void SetTotal(int totalRows)
    {
        TotalRows = totalRows;
        TotalPages = Math.Max(1, (int)Math.Ceiling(totalRows / (double)PageSize));
        PageNum = Math.Clamp(PageNum, 1, TotalPages);
    }
}

public class BookListService(BookDbContext db)
{
    public async Task<List<BookListDto>> SortFilterPageAsync(BookFilterOptions options)
    {
        var query = db.Books.AsNoTracking()
            .MapBookToDto()
            .FilterBooksBy(options.FilterBy, options.FilterValue)
            .OrderBooksBy(options.OrderBy);

        // Câu SQL thứ nhất: Đếm tổng số dòng (để tính phân trang)
        options.SetTotal(await query.CountAsync());

        // Câu SQL thứ hai: Lấy dữ liệu của trang hiện tại
        return await query
            .Page(options.PageNum - 1, options.PageSize)
            .ToListAsync();
    }
}