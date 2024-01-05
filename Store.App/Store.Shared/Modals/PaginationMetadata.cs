namespace Store.Shared.Modals
{
    public class PaginationMetadata
    {
        public int TotalItemCount { get; set; }

        public int TotalPageCount { get; set; }

        public int PageSize { get; set; } = 10;

        public int CurrentPage { get; set; } = 1;

        public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }

        public PaginationMetadata()
        {
        }
    }
}
