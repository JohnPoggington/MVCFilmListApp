using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVCFilmLists.Data;
using MVCFilmLists.Models;

namespace MVCFilmLists.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var applicationDbContext = _context.Movie.Include(m => m.Director).Include(m => m.Genre);
            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = _context.Movie.Where(s => s.Title.ToLower().Contains(searchString.ToLower())
                || s.Description.ToLower().Contains(searchString.ToLower()))
                    .Include(m => m.Director)
                    .Include(m => m.Genre);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    applicationDbContext = applicationDbContext.OrderByDescending(s => s.Title).Include(m => m.Director).Include(m => m.Genre);
                    break;
                case "Date":
                    applicationDbContext = applicationDbContext.OrderBy(s => s.ReleaseDate).Include(m => m.Director).Include(m => m.Genre);
                    break;
                case "date_desc":
                    applicationDbContext = applicationDbContext.OrderByDescending(s => s.ReleaseDate).Include(m => m.Director).Include(m => m.Genre);
                    break;
                default:
                    applicationDbContext = applicationDbContext.OrderBy(s => s.Title).Include(m => m.Director).Include(m => m.Genre);
                    break;
            }
            int pageSize = 3;

            return View(await PaginatedList<Movie>.CreateAsync(applicationDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            
            if (User.Identity.Name != null)
            {
                var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var curUserId = user.Id;

                var userLists = _context.MovieLists.Where(l => l.ApplicationUserId == curUserId).ToList();

                ViewData["UserLists"] = new SelectList(userLists, "Id", "Name");
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["DirectorId"] = new SelectList(_context.Director, "Id", "FirstAndLastName");
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Runtime,ReleaseDate,GenreId,DirectorId")] Movie movie)
        {
            ModelState.Remove("Director");
            ModelState.Remove("Genre");
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DirectorId"] = new SelectList(_context.Director, "Id", "FirstAndLastName", movie.DirectorId);
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", movie.GenreId);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["DirectorId"] = new SelectList(_context.Director, "Id", "FirstAndLastName", movie.DirectorId);
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", movie.GenreId);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Runtime,ReleaseDate,GenreId,DirectorId")] Movie movie)
        {
            
            if (id != movie.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Director");
            ModelState.Remove("Genre");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DirectorId"] = new SelectList(_context.Director, "Id", "Id", movie.DirectorId);
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Id", movie.GenreId);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Director)
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
