using System.ComponentModel.DataAnnotations;

namespace Book_Management_API.Model
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Range(1000, 2025)]
        public int PublicationYear { get; set; }

        [Required]
        public string AuthorName { get; set; }
        public int ViewsCount { get; set; }
    }
}
