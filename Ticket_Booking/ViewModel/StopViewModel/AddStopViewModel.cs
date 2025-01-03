using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.StopViewModel
{
    public class AddStopViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bus Number")]
        [Required(ErrorMessage = "Bus Number is required")]
        public string BusNumber { get; set; }

        [Display(Name = "From City")]
        [Required(ErrorMessage = "Start From City is required")]
        public string StartCity { get; set; }

        [Display(Name = "To City")]
        [Required(ErrorMessage = "To City is required")]
        public string DestinationCity { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Start Time is required")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Reached Time")]
        [Required(ErrorMessage = "Reached Time is required")]
        public DateTime ReachedTime { get; set; }

        [Display(Name = "Stop City")]
        [Required(ErrorMessage = "Stop City is required")]
        public int StopCity { get; set; }

        [Display(Name = "Stop Time")]
        [Required(ErrorMessage = "Stop Time is required")]
        public DateTime StopTime { get; set; }

        public IEnumerable<SelectListItem> City { get; set; }
        public List<StopViewModel> Stops { get; set; }

        public AddStopViewModel()
        {
            City = new List<SelectListItem>();
            Stops = new List<StopViewModel>();
        }
    }

    public class StopViewModel
    {
        public string CityName { get; set; }
        public DateTime StopTime { get; set; }
    }
}



