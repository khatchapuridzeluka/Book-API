using Book_Management_API.Data;
using Book_Management_API.Model;
using Book_Management_API.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Management_API.Controllers
{
    [Authorize]
    [Route("api/BookAPI")]
    [ApiController]
    public class BookAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BookAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ADDING THE BOOK
        [Authorize]
        [HttpPost("CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] BookDTO bookDTO)
        {
            if (bookDTO == null)
            {
                return BadRequest(bookDTO);
            }

            //Validation if the book already exists in the database
            if (await _db.Books.AnyAsync
               (b => b.Title == bookDTO.Title
                && b.AuthorName == bookDTO.AuthorName
                && b.PublicationYear == bookDTO.PublicationYear))
            {
                return BadRequest("The book Already Exists in the database");
            }

            //Creating A book based on dto
            Book model = new()
            {
                Title = bookDTO.Title,
                PublicationYear = bookDTO.PublicationYear,
                AuthorName = bookDTO.AuthorName,
                ViewsCount = 0
            };
            await _db.Books.AddAsync(model);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("GetBook", new { id = model.Id }, model);
        }

        //CREATE BOOKS (bulk)
        [HttpPost("CreateBooks",Name ="CreateBooks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateBooks([FromBody] IEnumerable<BookDTO> books)
        {
            if( books == null || !books.Any())
            {
                return BadRequest("The books List Is empty");
            }
            //getting the data before foreach loop
            var existingBooks =await _db.Books
                .Select(b => new { b.Title, b.AuthorName, b.PublicationYear })
                .ToListAsync();
            var booksToAdd = new HashSet<Book>();

            foreach (var bookDTO in books)
            {
                //checking if the book already exists, to be skipped and continue the request
                // or if we are adding the same books
                if(existingBooks.Contains(new {bookDTO.Title, bookDTO.AuthorName, bookDTO.PublicationYear})
                 || booksToAdd.Any(b => b.Title == bookDTO.Title && b.AuthorName == bookDTO.AuthorName && b.PublicationYear == bookDTO.PublicationYear))
                {
                    continue;
                }

                booksToAdd.Add(new Book
                {
                    Title = bookDTO.Title,
                    PublicationYear = bookDTO.PublicationYear,
                    AuthorName = bookDTO.AuthorName,
                    ViewsCount = 0
                });
            }

            if(!booksToAdd.Any())
            {
                return BadRequest("No Books Were added!");
            }
            var resultBooksToAdd = booksToAdd.ToHashSet();
            await _db.Books.AddRangeAsync(booksToAdd);
            await _db.SaveChangesAsync();
            return Created();
        }


        //Updating the BOOK
        [HttpPut("UpdateBook/{id:int}",Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateBook(int id, [FromBody] BookDTO bookDTO)
        {
            if(bookDTO == null || id <= 0)
            {
                return BadRequest();
            }

            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if( book == null )
            {
                return NotFound();
            }
            book.Title = bookDTO.Title;
            book.AuthorName = bookDTO.AuthorName;
            book.PublicationYear = bookDTO.PublicationYear;
            _db.Books.Update(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }



        //Deleting the book
        [HttpDelete("DeleteBook/{id:int}",Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBook(int id)
        {
            if (id <=0)
            {
                return BadRequest();
            }
            var book = await _db.Books.FirstOrDefaultAsync(b =>b.Id == id);
            if(book == null)
            {
                return NotFound();
            }
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Deleting several books (bulk)
        [HttpDelete("DeleteBooks",Name ="DeleteBooks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBooks([FromBody] List<int> ids)
        {
            if(ids == null || !ids.Any())
            {
                return BadRequest("The ids are empty");
            }
            var deleteBooksList = await _db.Books.Where(b => ids.Contains(b.Id)).ToListAsync();
            if (!deleteBooksList.Any())
            {
                return NotFound("No matching books");
            }
            _db.RemoveRange(deleteBooksList);
            await _db.SaveChangesAsync();
            return NoContent();
        }



        //List Of Books only Titles
        [HttpGet("GetBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBooks()
        {
            var titles = await _db.Books
                .OrderByDescending(b => b.ViewsCount)
                .Select(b => b.Title)
                .ToListAsync();
            return Ok(titles);
        }


        // GETTING THE BOOK BY ID
        [HttpGet("GetBookDetails/{id}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetBookDetails(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            book.ViewsCount++;
            await _db.SaveChangesAsync();

            //Calculating popularityScore and create anonymous obj for response
            var popularityScore = book.ViewsCount * 1.0 / 2 + (DateTime.Now.Year - book.PublicationYear) * 2;

            var responseObj = new
            {
                book.Id,
                book.Title,
                book.AuthorName,
                book.PublicationYear,
                book.ViewsCount,
                PopularityScore = popularityScore
            };
            return Ok(responseObj);
        }
    }
}
