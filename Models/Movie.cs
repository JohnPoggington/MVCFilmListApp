using System.ComponentModel.DataAnnotations;

namespace MVCFilmLists.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        [DisplayFormat(NullDisplayText = "No description")]
        public string? Description { get; set; }
        [Range(5, int.MaxValue)]
        public int Runtime { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        [Display(Name = "Director")]
        public int DirectorId { get; set; }
        public Director Director { get; set; } = null!;

        public ICollection<MovieList> movieLists { get; set; }

        //public List<ListEntry> Entries { get; } = [];
    }
}
