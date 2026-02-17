namespace ProfsMarkovHub.ViewModels;

public class BlogIndexViewModel
{
    public List<Models.Article> Articles { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string? TagFilter { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
