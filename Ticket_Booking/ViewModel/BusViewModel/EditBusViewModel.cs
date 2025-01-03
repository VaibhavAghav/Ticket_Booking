using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.BusViewModel
{
    public class EditBusViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bus number is required")]
        [RegularExpression(@"^[A-Z]{2}\s\d{2}\s[A-Z]{2}\s\d{4}$", ErrorMessage = "Bus number must be in the format WW NN WW NNNN where W is an uppercase letter and N is a digit.")]
        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }

        [Required(ErrorMessage = "Seat capacity is required")]
        [Display(Name = "Seat Capacity")]
        [Range(10, 25, ErrorMessage = "Seat capacity must be greater than 10 and less than 25")]
        public int SeatCapacity { get; set; }
    }
}
