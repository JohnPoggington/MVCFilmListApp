using System.ComponentModel.DataAnnotations;

namespace MVCFilmLists.Models
{
    public class MovieList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        //public List<ListEntry> Entries { get; set; } = [];

        public ICollection<Movie> Movies { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;


    }
}
