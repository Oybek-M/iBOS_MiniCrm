namespace SmartCrm.Service.Common.Utils;

public class PageModel<T>
{
    public int PageNumber { get; set; } // joriy sahifa (1‐dan)
    public int PageSize { get; set; } // sahifada qancha element (masalan, 10)
    public int TotalPagesCount { get; set; } // jami sahifalar soni
    public int TotalItemsCount { get; set; } // jami elementlar soni
    public List<T> Items { get; set; } = new List<T>();


    public PageModel(List<T> fullList, int pageNumber, int pageSize = 10)
    {
        if (pageNumber < 1) throw new ArgumentException("PageNumber must be >= 1");
        if (pageSize < 1) throw new ArgumentException("PageSize must be >= 1");

        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItemsCount = fullList.Count;
        TotalPagesCount = (int)Math.Ceiling(TotalItemsCount / (double)pageSize);
        Items = fullList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}
