using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Ticket_Booking.ViewModel.RouteViewModel
{
    public class DeleteRouteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bus Number")]
        [Required(ErrorMessage = "Bus Number is required")]
        public int BusId { get; set; }

        [Display(Name = "From City")]
        [Required(ErrorMessage = "Start From City is required")]
        public int StartCityId { get; set; }

        [Display(Name = "To Time")]
        [Required(ErrorMessage = "To City is required")]
        public int DestinationCityId { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Start Time is required")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Reached Time")]
        [Required(ErrorMessage = "Reached Time is required")]
        public DateTime ReachedTime { get; set; }

        public IEnumerable<SelectListItem> Buses { get; set; }
        public IEnumerable<SelectListItem> City { get; set; }

        public List<StopViewModel> Stops { get; set; } // Add this property for stops

        public DeleteRouteViewModel()
        {
            Buses = new List<SelectListItem>();
            City = new List<SelectListItem>();
            Stops = new List<StopViewModel>();
        }
    }

    public class StopViewModel
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public DateTime StopTime { get; set; }
    }
}



//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.ComponentModel.DataAnnotations;

//namespace Ticket_Booking.ViewModel.RouteViewModel
//{
//    public class DeleteRouteViewModel
//    {
//        public int Id { get; set; }

//        [Display(Name = "Bus Number")]

//        [Required(ErrorMessage = "Bus Number is required")]
//        public int BusId { get; set; }

//        [Display(Name = "From City")]
//        [Required(ErrorMessage = "Start FRom City is required")]
//        public int StartCityId { get; set; }

//        [Display(Name = "To Time")]
//        [Required(ErrorMessage = "To City is required")]
//        public int DestinationCityId { get; set; }

//        [Display(Name = "Start Time")]
//        [Required(ErrorMessage = "Start Time is required")]
//        public DateTime StartTime { get; set; }

//        [Display(Name = "Reached Time")]
//        [Required(ErrorMessage = "Reached Time is required")]
//        public DateTime ReachedTime { get; set; }

//        public IEnumerable<SelectListItem> Buses { get; set; }
//        public IEnumerable<SelectListItem> City { get; set; }

//        public DeleteRouteViewModel()
//        {
//            Buses = new List<SelectListItem>();
//            City = new List<SelectListItem>();
//        }
//    }
//}
