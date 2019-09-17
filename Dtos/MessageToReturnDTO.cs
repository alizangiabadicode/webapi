using System;
using datingapp.api.Models;

namespace datingapp.api.Dtos
{
    public class MessageToReturnDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int ReciverId { get; set; }
        public string ReciverKnownAs { get; set; }
        public string ReciverPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSent { get; set; }

    }
}