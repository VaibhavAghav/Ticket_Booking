using System.ComponentModel.DataAnnotations;

namespace Ticket_Booking.ViewModel.AccountViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmed Password")]
        [Compare("Password",ErrorMessage ="Password and confirmation password does not match")]
        public string ConfirmPassword { get; set;}

    }
}
