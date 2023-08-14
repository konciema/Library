using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library;
using Library.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;


namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var list = await _context.Books.Select(x => new BookIndex() { BookID = x.BookID, FirstName = x.FirstName, LastName = x.LastName, Title = x.Title, BookLocation = x.BookLocation }).ToListAsync();
            return View(list);
        }

        // POST: Books/DeleteAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            // Delete all books from the database
            _context.Books.RemoveRange(_context.Books);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            if(book.Photo != null && book.Photo.Length >0)
            {
                var base64 = Convert.ToBase64String(book.Photo);
                var imgSrc = String.Format($"data:image/gif;base64,{base64}");
                ViewBag.ImgSrc = imgSrc;
            } 

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookID,FirstName,LastName,Title,LiteraryType,BookLocation,Content,BookCollection,Photo,Thumb,DateInserted")] Book book,IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                book.BookID = Guid.NewGuid();
                book.DateInserted = DateTime.Now;

                HandlePhoto<Book>(book, photo, 100);

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        private void HandlePhoto<T>(T @object, IFormFile photo, int thumbWidth)
        {
            if (photo != null && photo.Length > 0)
            {
                if (!AuxiliaryFunctions.ValidImageTypes.Contains(photo.ContentType))
                {
                    ModelState.AddModelError("Photo", "Izberite fotografijo v eni izmed naslednjih oblik: BMP, GIF, JPG, or PNG.");
                }
                else
                {
                    using (var reader = new BinaryReader(photo.OpenReadStream()))
                    {
                        if (@object is Book)
                        {
                            (@object as Book).Photo = reader.ReadBytes((int)photo.Length);
                            (@object as Book).Thumb = AuxiliaryFunctions.CreateThumbnail((@object as Book).Photo, thumbWidth);
                        }
                    }
                }
            }
          
        }


        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            if (book.Thumb != null && book.Thumb.Length > 0)
            {
                var base64 = Convert.ToBase64String(book.Thumb);
                var imgSrc = String.Format($"data:image/gif;base64,{base64}");
                ViewBag.ImgSrc = imgSrc;
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BookID,FirstName,LastName,Title,LiteraryType,BookLocation,Content,BookCollection, Photo, Thumb, DateInserted")] Book book, IFormFile? photo)
        {
            if (id != book.BookID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null)
                    {
                        HandlePhoto<Book>(book, photo, 350);
                    }
                    else
                    {
                        var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookID == book.BookID);
                        if (existingBook != null)
                        {
                            book.Photo = existingBook.Photo;
                            book.Thumb = existingBook.Thumb;
                        }
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = book.BookID });
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            if (book.Thumb != null && book.Thumb.Length > 0)
            {
                var base64 = Convert.ToBase64String(book.Thumb);
                var imgSrc = String.Format($"data:image/gif;base64,{base64}");
                ViewBag.ImgSrc = imgSrc;
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books
        public async Task<IActionResult> Csv()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Csv(IFormFile importFile)
        {
            try
            {
                List<Book> bookList;
                using (var stream = importFile.OpenReadStream())
                {
                    bookList = GetDataFromCSVFile(stream);
                }

                // Now you can do something with the bookList, like saving it to the database
                foreach (var book in bookList)
                {
                    _context.Books.Add(book);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }

        // Read CSV file
        private List<Book> GetDataFromCSVFile(Stream stream)
        {
            var bookList = new List<Book>();
            try
            {
                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Skip the header row
                    if (!parser.EndOfData)
                    {
                        parser.ReadLine();
                    }

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields != null && fields.Length > 0)
                        {

                            Book book = new Book()
                                {
                                    BookID = Guid.NewGuid(),
                                    FirstName = fields[1],
                                    LastName = fields[2],
                                    Title = fields[3],
                                    LiteraryType = fields[4],
                                    BookLocation = fields[5],
                                    Content = fields[6],
                                    BookCollection = fields[7],
                                    Photo = new byte[0],
                                    Thumb = new byte[0],
                                    DateInserted = DateTime.Now
                                };

                                bookList.Add(book); // Add the book to the bookList    
                            
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return bookList;
        }

        // SEARCH CONTROLER
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchResults(string searchTerm)
        {

            
            var books = await _context.Books.Select(x => new BookSearch() { BookID = x.BookID, FirstName = x.FirstName, LastName = x.LastName, Title = x.Title })
                .Where(book => book.Title.Contains(searchTerm) || book.LastName.Contains(searchTerm) || book.FirstName.Contains(searchTerm))
                .ToListAsync();

            return View("SearchResults", books);
        }

        private bool BookExists(Guid id)
        {
          return (_context.Books?.Any(e => e.BookID == id)).GetValueOrDefault();
        }




        
    }


}
