using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket_Model
{
    public class Bus
    {

        [Key]
        public int Id { get; set; }
        public string BusNumber { get; set; }
        public int SeatCapacity { get; set; }
    }
}
