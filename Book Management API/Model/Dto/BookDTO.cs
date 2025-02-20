using System.ComponentModel.DataAnnotations;

namespace Book_Management_API.Model.Dto
{
    public class BookDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]

        [Range(1000,2025)]
        public int PublicationYear { get; set; }

        [Required]
        public string AuthorName { get; set; }

    }
}
