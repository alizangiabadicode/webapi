using System;
using Microsoft.AspNetCore.Http;

namespace datingapp.api.Dtos
{
    public class PhotoForCreationDTO
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public PhotoForCreationDTO()
        {
            this.DateAdded = DateTime.Now;
        }
    }
}