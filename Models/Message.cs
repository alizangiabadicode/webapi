using System;

namespace datingapp.api.Models
{
    public class Message 
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReciverId { get; set; }
        public User Reciver { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSent { get; set; }
        public bool SenderDeleted { get; set; }
        public bool ReciverDeleted { get; set; }
    }
}