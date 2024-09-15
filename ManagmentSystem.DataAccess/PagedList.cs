using Microsoft.EntityFrameworkCore;

namespace ManagmentSystem.DataAccess;

/// <summary>
/// представляє сторінку даних та містить додаткову інформацію для керування навігацією по сторінках
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedList<T> 
{
    public PagedList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// список елементів на поточній сторінці.
    /// </summary>
    public List<T> Items { get; }

    /// <summary>
    /// поточний номер сторінки.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// кількість елементів на сторінці.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// загальна кількість елементів у запиті.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// визначає, чи є наступна сторінка (якщо є більше елементів, ніж вміщується на поточну сторінку)
    /// </summary>
    public bool HasNextPage => Page * PageSize < TotalCount;

    /// <summary>
    /// визначає, чи є попередня сторінка.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// рахує загальну кількість елементів у запиті, вибирає елементи для поточної 
    /// сторінки (з урахуванням номера сторінки і розміру сторінки), 
    /// та повертає об'єкт PagedList<T>, що містить ці елементи і інформацію 
    /// про сторінки (загальна кількість, номер сторінки, тощо).
    /// </summary>
    /// <param name="query"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new(items, page, pageSize, totalCount);
    }
}
    