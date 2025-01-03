using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.AdministrationViewModel
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
