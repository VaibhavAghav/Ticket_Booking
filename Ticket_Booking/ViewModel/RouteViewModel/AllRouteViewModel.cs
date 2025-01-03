using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.RouteViewModel
{
    public class AllRouteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bus Number")]
        public string BusNumber { get; set; }

        [Display(Name = "From City")]
        public string StartCity { get; set; }

        [Display(Name = "To City")]
        public string EndCity { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Reached Time")]
        public DateTime ReachedTime { get; set; }
    }
}
