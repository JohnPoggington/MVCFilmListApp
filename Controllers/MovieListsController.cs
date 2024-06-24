using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCFilmLists.Data;
using MVCFilmLists.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MVCFilmLists.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MVCFilmLists.Controllers
{
    public class MovieListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public MovieListsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MovieLists
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var lists = await _context.MovieLists.Include(l => l.ApplicationUser).ToListAsync();

            switch(sortOrder)
            {
                case "name_desc":
                    lists = await _context.MovieLists.OrderByDescending(s => s.Name).Include(l => l.ApplicationUser).ToListAsync();
                    break;
                case "Date":
                    lists = await _context.MovieLists.OrderBy(s => s.CreationDate).Include(l => l.ApplicationUser).ToListAsync();
                    break;
                case "date_desc":
                    lists = await _context.MovieLists.OrderByDescending(s => s.CreationDate).Include(l => l.ApplicationUser).ToListAsync();
                    break;
                default:
                    lists = await _context.MovieLists.OrderBy(s => s.Name).Include(l => l.ApplicationUser).ToListAsync();
                    break;

            }
            if (!String.IsNullOrEmpty(searchString))
            {
                lists = lists.Where(l => l.Name.Contains(searchString)).ToList();
            }
            return View(lists);
        }

        // GET: MovieLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var movieList = await _context.MovieLists
            //    .Include(l => l.Entries).ThenInclude(e => e.Movie)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var movieList = new ListDetailsData();
            movieList.MovieList = await _context.MovieLists
                .Include(l => l.Movies).ThenInclude(m => m.Director)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            
            if (movieList == null)
            {

                return NotFound();
            }
            //movieList.Movies = movieList.MovieList.Entries.Select(e => e.Movie).ToList();
            return View(movieList);
        }

        // GET: MovieLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MovieLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] MovieList movieList)
        {
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("CreationDate");
            ModelState.Remove("ApplicationUserId");
            ModelState.Remove("Movies");
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            movieList.ApplicationUserId = user.Id;
            ModelState.ClearValidationState(nameof(MovieList));
            if (ModelState.IsValid)
            {
                _context.Add(movieList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
                await Console.Out.WriteLineAsync(message);
            }
            return View(movieList);
        }

        // GET: MovieLists/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var pom = await _context.MovieLists.Include(m => m.Entries).ThenInclude(entry => entry.MovieId).FirstOrDefault(m => m.Id == id);

            //var movieList = await  _context.MovieLists.Include(l => l.Movies).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

            var movieList = new ListViewModel();

            movieList.MovieList = await _context.MovieLists.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Id == id);
            movieList.Movies = await _context.Movie.Include(m => m.Director).ToListAsync();

            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (movieList.MovieList.ApplicationUserId != user.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (movieList == null)
            {
                return NotFound();
            }
            return View(movieList);
        }

        private void PopulateAssignedMovies(MovieList movieList)
        {
            var allMovies = _context.Movie;
            var moviesInList = new HashSet<int>(movieList.Movies.Select(e => e.Id));
            var viewModel = new List<AssignedMovieData>();

            foreach (var movie in allMovies)
            {
                viewModel.Add(new AssignedMovieData { MovieId = movie.Id,
                Title = movie.Title,
                Assigned = moviesInList.Contains(movie.Id)});
            }
            ViewData["MoviesInList"] = viewModel;
        }

        // POST: MovieLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(ListViewModel model, int[] movieIds)
        {
            var list = await _context.MovieLists.Include(l => l.Movies).FirstOrDefaultAsync(x => x.Id == model.MovieList.Id);

            if (list == null) { return NotFound(); }
            
            if (list != null)
            {
                list.Name = model.MovieList.Name;
                list.Description = model.MovieList.Description;

                list.Movies.Clear();

                if (movieIds != null)
                {
                    foreach (var movieId in movieIds)
                    {
                        var movie = await _context.Movie.FindAsync(movieId);
                        if (movie != null)
                        {
                            list.Movies.Add(movie);
                        }
                    }
                }
                _context.Update(list);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));

                
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description, CreationDate")] MovieList movieList, string[] selectedMovies)
        //{

        //    if (id != movieList.Id)
        //    {
        //        return NotFound();
        //    }
        //    ModelState.Remove("ApplicationUser");
        //    ModelState.Remove("CreationDate");
        //    ModelState.Remove("ApplicationUserId");
        //    var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
        //    movieList.ApplicationUserId = user.Id;
        //    movieList.Id = id;

        //    //var listToUpdate = await _context.MovieLists
        //    //    .Include(l => l.Entries)
        //    //    .ThenInclude(e => e.Movie)
        //    //    .FirstOrDefaultAsync(m => m.Id == id);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            Console.ForegroundColor = ConsoleColor.Green;
        //            string pom = "";
        //            foreach(var item in selectedMovies)
        //            {
        //                pom += item;
        //            }
        //            await Console.Out.WriteLineAsync(selectedMovies.Length.ToString());
        //            await Console.Out.WriteLineAsync(pom);
        //            Console.ResetColor();

        //            UpdateListEntries(selectedMovies, movieList);
        //            _context.Update(movieList);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MovieListExists(movieList.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(movieList);
        //}

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> ModifyEntry(int? listId, int? movieId)
        //{
        //    var listToUpdate = await _context.MovieLists.Include(l => l.Entries).ThenInclude(e => e.Movie).SingleOrDefaultAsync(m => m.Id == listId);

        //    var selectedMovie = _context.Movie.SingleOrDefaultAsync(m => m.Id == movieId);


        //    var movies = new HashSet<int>(listToUpdate.Entries.Select(e => e.Movie.Id));

        //    foreach (var movie in _context.Movie)
        //    {
               
        //        if (movie.Id == movieId)
        //        {
        //            if (!movies.Contains(movie.Id))
        //            {
        //                listToUpdate.Entries.Add(new ListEntry { MovieListId = listToUpdate.Id, MovieId = movie.Id });
        //            }
        //        }
        //        else
        //        {
        //            if (movies.Contains(movie.Id))
        //            {
        //                ListEntry entryToRemove = listToUpdate.Entries.FirstOrDefault(i => i.MovieId == movie.Id);
        //                _context.Remove(entryToRemove);
        //            }
        //        }
        //    }
        //    _context.Update(listToUpdate);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        //public async Task<IActionResult> EditEntries(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //var pom = await _context.MovieLists.Include(m => m.Entries).ThenInclude(entry => entry.MovieId).FirstOrDefault(m => m.Id == id);

        //    var movieList = await _context.MovieLists.Include(l => l.Entries).ThenInclude(e => e.Movie).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

        //    if (movieList == null)
        //    {
        //        return NotFound();
        //    }
        //    PopulateAssignedMovies(movieList);
        //    return View(movieList);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditEntries(int? id, string[] selectedCourses)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var instructorToUpdate = await _context.MovieLists
        //        .Include(i => i.Entries)
        //            .ThenInclude(i => i.Movie)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    await Console.Out.WriteLineAsync(selectedCourses.ToString());
        //    Console.ResetColor();

        //    if (await TryUpdateModelAsync<MovieList>(
        //        instructorToUpdate))
        //    {
                
        //        UpdateListEntries(selectedCourses, instructorToUpdate);
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateException /* ex */)
        //        {
        //            //Log the error (uncomment ex variable name and write a log.)
        //            ModelState.AddModelError("", "Unable to save changes. " +
        //                "Try again, and if the problem persists, " +
        //                "see your system administrator.");
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    UpdateListEntries(selectedCourses, instructorToUpdate);
        //    PopulateAssignedMovies(instructorToUpdate);
        //    return RedirectToAction(nameof(Details));
        //}

        private void UpdateListEntries(string[] selectedMovies, MovieList ListToUpdate)
        {
            if (selectedMovies == null)
            {
                ListToUpdate.Movies = new List<Movie>();
                return;
            }

            var selectedMoviesHS = new HashSet<string>(selectedMovies);
            var movies = new HashSet<int>(ListToUpdate.Movies.Select(e => e.Id));

            foreach (var movie in _context.Movie)
            {
                if (selectedMoviesHS.Contains(movie.Id.ToString()))
                {
                    if (!movies.Contains(movie.Id))
                    {
                        ListToUpdate.Movies.Add(_context.Movie.SingleOrDefault(m => m.Id == movie.Id));
                    }
                }
                else
                {
                    if (movies.Contains(movie.Id))
                    {
                        Movie entryToRemove = ListToUpdate.Movies.FirstOrDefault(i => i.Id == movie.Id);
                        _context.Remove(entryToRemove);
                    }
                }
            }
        }

        // GET: MovieLists/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieList = await _context.MovieLists
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (movieList.ApplicationUserId != user.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (movieList == null)
            {
                return NotFound();
            }

            return View(movieList);
        }

        // POST: MovieLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieList = await _context.MovieLists.FindAsync(id);
            if (movieList != null)
            {
                _context.MovieLists.Remove(movieList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddMovie(int movieId, int listId)
        {
            return RedirectToAction(nameof(Index));
        }

        private bool MovieListExists(int id)
        {
            return _context.MovieLists.Any(e => e.Id == id);
        }
    }
}
