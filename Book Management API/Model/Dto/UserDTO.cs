using System.ComponentModel.DataAnnotations;

namespace Book_Management_API.Model.Dto
{
    public class UserDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
