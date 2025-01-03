using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.BookViewModel
{
    public class SearchBookTicketViewModel
    {
        [Display(Name = "From City")]
        public int FromCityId { get; set; }

        [Display(Name = "To City")]
        public int ToCityId { get; set; }

        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public List<SelectListItem> Cities { get; set; }
    }
}



//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.ComponentModel.DataAnnotations;

//namespace Ticket_Booking.ViewModel.BookViewModel
//{
//    public class SearchBookTicketViewModel
//    {
//        [Display(Name = "From City")]
//        public int FromCityId { get; set; }

//        [Display(Name = "To City")]
//        public int ToCityId { get; set; }

//        public List<SelectListItem> Cities { get; set; }
//    }
//}
