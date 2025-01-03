using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.BusViewModel
{
    public class DeleteBusViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }


        [Display(Name = "Seat Capacity")]
        public int SeatCapacity { get; set; }
    }
}
