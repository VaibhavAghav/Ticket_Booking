using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticket_Model
{
    public class BusDaily
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BusRoute")]
        public int BusRouteId { get; set; }
        public virtual BusRoute BusRoute { get; set; }
        public DateTime Date { get; set; }
        public int AvailableSeats { get; set; }

        [ForeignKey("StartCity")]
        public int StartCityId { get; set; }
        public virtual City StartCity { get; set; }

        [ForeignKey("DestinationCity")]
        public int DestinationCityId { get; set; }
        public virtual City DestinationCity { get; set; }

    }
}

