using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.StopViewModel
{
    public class DeleteStopViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }

        [Display(Name = "Stop City")]
        public string City { get; set; }
        public DateTime StopTime { get; set; }
    }
}
