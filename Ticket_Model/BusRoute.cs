using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Model
{
    public class BusRoute
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Bus")]
        public int BusId { get; set; }
        public virtual Bus Bus { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime ReachedTime { get; set; }


        [ForeignKey("StartCity")]
        public int StartCityId { get; set; }
        public virtual City StartCity { get; set; }

        [ForeignKey("DestinationCity")]
        public int DestinationCityId { get; set; }
        public virtual City DestinationCity { get; set; }

    }
}
