using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.BookViewModel
{
    public class GetAllTicketViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Passenger Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Seat to Book")]
        [Range(1, int.MaxValue, ErrorMessage = "Seats to book must be at least 1.")]
        public int Seats { get; set; }

        [Display(Name = "Bus Number(Route)")]
        public int BueRouteId { get; set; }

        [Display(Name = "Bus Number")]
        public string BusId { get; set; }

        [Display(Name = "Pick City")]
        public string StartCity { get; set; }

        [Display(Name = "Drop City")]
        public string DestinationCity { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Reached Time")]
        public DateTime ReachedTime { get; set; }

            
    }
}
