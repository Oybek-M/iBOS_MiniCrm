namespace SmartCrm.Service.Common.Utils;

public class PaginationParams
{
    private const int maxPageSize = 50;
    private int pageSize = 10;
    public int PageSize
    {
        get => pageSize;
        set
        {
            if (value <= maxPageSize && value > 0)
                pageSize = value;
            else throw new ArgumentException(
                $"Page size must be between 1 and {maxPageSize}.");
        }
    }

    public int PageIndex { get; set; }

    public PaginationParams()
    {
        PageIndex = 1;
        PageSize = 10; 
    }
    public PaginationParams(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public int SkipCount()
        => (PageIndex - 1) * PageSize;
}
