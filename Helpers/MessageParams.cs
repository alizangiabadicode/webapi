namespace datingapp.api.Helpers
{
    public class MessageParams
    {
        public const int MaxSize = 50;
        public int PageNumber { get; set; } = 1;
        public int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxSize ? MaxSize : value); }
        }
        public string Gender { get; set; }
        public int UserId { get; set; }
        public string MessageContainer { get; set;}
    }
}