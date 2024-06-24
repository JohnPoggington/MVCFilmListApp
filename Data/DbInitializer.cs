using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MVCFilmLists.Models;

namespace MVCFilmLists.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            
            if (context.Movie.Any())
            {
                return;
            }

            var genres = new Genre[]
            {
                new Genre {Name = "Action"},
                new Genre {Name = "Drama"},
                new Genre {Name = "Comedy"},
            };

            foreach(Genre g in genres)
            {
                context.Genre.Add(g);
            }
            context.SaveChanges();

            var directors = new Director[]
            {
                new Director { FirstName = "Martin", LastName = "Scorcese" },
                new Director { FirstName = "Quentin", LastName = "Tarantino" },
                new Director { FirstName = "Cristopher", LastName = "Nolan" },

                new Director { FirstName = "Ridley", LastName = "Scott" },

            };

            foreach (Director director in directors) { context.Director.Add(director);}
            context.SaveChanges();

            var movies = new Movie[] 
            {
                new Movie {Title = "The Wolf of Wall Street", Description = "Based on the true story of Jordan Belfort, from his rise to a wealthy stock-broker living the high life to his fall involving crime, corruption and the federal government.",
                Runtime = 180, ReleaseDate = new DateTime(2014, 12, 17), GenreId = genres.Single(g => g.Name == "Drama").Id, DirectorId = directors.Single(d => d.LastName == "Scorcese").Id},
                new Movie {Title = "Django Unchained", Description = "With the help of a German bounty-hunter, a freed slave sets out to rescue his wife from a brutal plantation owner in Mississippi.",
                Runtime = 165, ReleaseDate = new DateTime(2012, 12, 11), GenreId = genres.Single(g => g.Name == "Drama").Id, DirectorId = directors.Single(d => d.LastName == "Tarantino").Id},
                new Movie {Title = "Tenet", Description = "Armed with only the word \"Tenet,\" and fighting for the survival of the entire world, CIA operative, The Protagonist, journeys through a twilight world of international espionage on a global mission that unfolds beyond real time.",
                Runtime = 150, ReleaseDate = new DateTime(2012, 12, 11), GenreId = genres.Single(g => g.Name == "Action").Id, DirectorId = directors.Single(d => d.LastName == "Nolan").Id},
                new Movie {Title = "The Fall Guy", Description = "A down-and-out stuntman must find the missing star of his ex-girlfriend's blockbuster film.",
                Runtime = 150, ReleaseDate = new DateTime(2024, 3, 12), GenreId = genres.Single(g => g.Name == "Comedy").Id, DirectorId = directors.Single(d => d.LastName == "Nolan").Id},
                new Movie {Title = "Gladiator", Description = "A former Roman General sets out to exact vengeance against the corrupt emperor who murdered his family and sent him into slavery.",
                Runtime = 150, ReleaseDate = new DateTime(2000, 5, 1), GenreId = genres.Single(g => g.Name == "Action").Id, DirectorId = directors.Single(d => d.LastName == "Scott").Id},
            };

            foreach(Movie movie in movies)
            {
                context.Movie.Add(movie);
            }
            context.SaveChanges();

            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            ApplicationUser admin = new ApplicationUser();
            admin.Email = "admin@example.com";
            admin.UserName = "admin@example.com";
            IdentityResult aresult = userManager.CreateAsync(admin, "Admin1!").Result;
            if (aresult.Succeeded)
            {
                userManager.AddToRoleAsync(admin, "Admin").Wait();
            }

            ApplicationUser user = new ApplicationUser();
            admin.Email = "user@example.com";
            admin.UserName = "user@example.com";
            IdentityResult uresult = userManager.CreateAsync(admin, "Test12!").Result;
            if (uresult.Succeeded)
            {
                userManager.AddToRoleAsync(user, "User").Wait();
            }
        }
    }
}
