using System;

namespace datingapp.api.Dtos
{
    public class CreateMsgDTO
    {
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public string Content {get;set;}
        public DateTime DateSent { get; set;}
        public CreateMsgDTO()
        {
            this.DateSent = DateTime.Now;
        }
    }
}