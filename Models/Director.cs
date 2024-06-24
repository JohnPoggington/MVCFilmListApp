using System.ComponentModel.DataAnnotations;

namespace MVCFilmLists.Models
{
    public class Director
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string FirstAndLastName { get =>  FirstName + " " + LastName; }

        public ICollection<Movie> Movies { get; } = new List<Movie>();
    }
}
