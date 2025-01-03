using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket_Model
{
    public class BusStop
    {
        public int Id { get; set; }

        [ForeignKey("BusRoute")]
        public int BusRoutId { get; set; }
        public virtual BusRoute BusRoute { get; set; }

        public DateTime StopTime { get; set; }


        [ForeignKey("AddCity")]
        public int AddCityId { get; set; }
        public virtual City AddCity { get; set; }
    }
}
