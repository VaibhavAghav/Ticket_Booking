using System.ComponentModel.DataAnnotations;

namespace Ticket_Model
{
    public class City
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "City Name")]
        public string CityName { get; set; }
    }
}
