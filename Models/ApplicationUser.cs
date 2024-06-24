using Microsoft.AspNetCore.Identity;

namespace MVCFilmLists.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }

        public ICollection<MovieList> MovieLists { get; set; } = new List<MovieList>();
    }
}
