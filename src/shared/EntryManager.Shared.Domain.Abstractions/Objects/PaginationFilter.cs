namespace EntryManager.Shared.Domain.Abstractions.Objects;

public struct PaginationFilter
{
    private int _pageNumber;
    private int _pageSize;

    /// <summary>
    /// The current page number. Defaults to 1.
    /// </summary>
    public int PageNumber
    {
        readonly get => _pageNumber <= 0 ? 1 : _pageNumber;
        set => _pageNumber = value;
    }

    /// <summary>
    /// The number of items per page. Defaults to 10.
    /// </summary>
    public int PageSize
    {
        readonly get => _pageSize <= 0 ? 10 : _pageSize;
        set => _pageSize = value;
    }

    /// <summary>
    /// The number of items to skip based on PageNumber and PageSize.
    /// </summary>
    public readonly int SkipLength => (PageNumber - 1) * PageSize;
}