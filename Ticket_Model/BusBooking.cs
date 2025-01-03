using System.ComponentModel.DataAnnotations.Schema;

namespace Ticket_Model
{
    public class BusBooking
    {
        public int Id { get; set; }

        [ForeignKey("BusRoute")]
        public int BusRouteId { get; set; }
        public virtual BusRoute BusRoute { get; set; }

        public string PassengerName { get; set; }

        public int NoSeatBooked { get; set; }

        public DateTime BookingDateTime { get; set; }
        public DateTime ReachingDateTime { get; set; }


        [ForeignKey("StartCity")]
        public int StartCityId { get; set; }
        public virtual City StartCity { get; set; }

        [ForeignKey("DestinationCity")]
        public int DestinationCityId { get; set; }
        public virtual City DestinationCity { get; set; }

    }
}
