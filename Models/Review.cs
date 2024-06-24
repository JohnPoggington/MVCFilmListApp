using System.ComponentModel.DataAnnotations;

namespace MVCFilmLists.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 20)]
        public string Content { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [Display(Name = "Author")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
