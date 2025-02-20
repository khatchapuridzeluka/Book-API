using Book_Management_API.Model;
using Microsoft.EntityFrameworkCore;

namespace Book_Management_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        
        }   
        public DbSet<Book> Books { get; set; }
    }
}
