using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Library.Models
{
    public class Book
    {
        [Key]
        public Guid BookID { get; set; }

        [Display(Name = "Ime avtorja")]
        public string? FirstName { get; set; }
        [Display(Name = "Priimek avtorja")]
        public string? LastName { get; set; }

        [Display(Name = "Naslov dela")]
        [Required(ErrorMessage = "Naslov morate obvezno vnesti!")]
        public string Title { get; set; }

        [Display(Name = "Literarna zvrst")]
        public string? LiteraryType { get; set; }

        [Display(Name = "Lokacija")]
        public string? BookLocation { get; set; }

        [Display(Name = "Vsebina")]
        public string? Content { get; set; }

        [Display(Name = "Zbirka")]
        public string? BookCollection { get; set; }

        public byte[]? Photo { get; set; }

        public byte[]? Thumb { get; set; }

        [Display(Name = "Datum vnosa")]
        public DateTime DateInserted { get; set; }
    }

    public class BookIndex
    {
        [Key]
        public Guid BookID { get; set; }
        [Display(Name = "Ime avtorja")]
        public string? FirstName { get; set; }
        [Display(Name = "Priimek avtorja")]
        public string? LastName { get; set; }

        [Display(Name = "Naslov dela")] 
        [Required(ErrorMessage = "Naslov morate obvezno vnesti!")] 
        public string Title { get; set; }

        [Display(Name = "Lokacija")]
        public string? BookLocation { get; set; }
    }

    public class BookHome
    {
        [Key]   
        public Guid BookID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Title { get; set; }
        public byte[]? Photo { get; set; }

    }
    public class BookSearch
    {
        [Key]
        public Guid BookID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Title { get; set; }
    }
}
