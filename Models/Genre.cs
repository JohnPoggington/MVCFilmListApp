using System.ComponentModel.DataAnnotations;

namespace MVCFilmLists.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        public String Name { get; set; }

        public ICollection<Movie> Movies { get; } = new List<Movie>();
    }
}
