using System;
using System.ComponentModel.DataAnnotations;

namespace datingapp.api.Dtos
{
    public class UserForRegisterDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "password is between 4 to 8 characters")]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        public UserForRegisterDTO()
        {
            this.Created = DateTime.Now;
            this.LastActive = DateTime.Now;
        }
    }
}