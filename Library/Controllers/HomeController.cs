using Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibraryDbContext _context;

        public HomeController(ILogger<HomeController> logger, LibraryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Books.Select(x => new BookHome() { BookID = x.BookID, FirstName = x.FirstName, LastName = x.LastName, Title = x.Title, Photo = x.Photo }).ToListAsync();
            
            return View(list);
        }

        private List<BookHome> GetRandomSubset(List<BookHome> sourceList, int count)
        {
            List<BookHome> randomSubset = new List<BookHome>();

            if (sourceList.Count <= count)
            {
                randomSubset.AddRange(sourceList);
            }
            else
            {
                Random random = new Random();
                HashSet<int> selectedIndices = new HashSet<int>();

                while (randomSubset.Count < count)
                {
                    int randomIndex = random.Next(sourceList.Count);

                    if (selectedIndices.Add(randomIndex))
                    {
                        randomSubset.Add(sourceList[randomIndex]);
                    }
                }
            }

            return randomSubset;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}