using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.StopViewModel
{
    public class CreateStopViewModel
    {
        public int Id { get; set; }


        [Display(Name = "Bus Number")]
        public int BusRouteId { get; set; }

        [Display(Name = "Stop")]
        public int Addcity { get; set; }

        public DateTime StopTime { get; set; }

        public IEnumerable<SelectListItem> Buses { get; set; }
        public IEnumerable<SelectListItem> City { get; set; }

        public CreateStopViewModel()
        {
            City = new List<SelectListItem>();
            Buses = new List<SelectListItem>();
        }

    }
}
