namespace datingapp.api.Helpers
{
    public class PaginationParams
    {
        public const int MaxSize = 50;
        public int PageNumber { get; set; } =1;
        public int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxSize ? MaxSize : value); }
        }
        public string Gender {get; set;}
        public int UserId { get;set;}

        public int MinAge { get;set; } = 18;
        public int MaxAge {get;set;} = 99;
        public string Order {get; set;}
        public bool showlikers {get;set;}
        public bool showlikees{get;set;}
    }
}