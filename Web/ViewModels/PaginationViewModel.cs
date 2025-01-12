namespace Web.ViewModels
{
    public class PaginationVM
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
    public class PaginationVM<T> : PaginationVM
    {
        public List<T> Items { get; set; } = new List<T>();
    }

}
