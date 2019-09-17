namespace datingapp.api.Helpers
{
    public class PaginationInit
    {
        public PaginationInit(int currentPage, int totalPages, int itemsPerPage, int totalItems)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
        }

        public int CurrentPage { get; }
        public int TotalPages { get; }
        public int ItemsPerPage { get; }
        public int TotalItems { get; }
    }
}