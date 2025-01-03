using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket_Model;

namespace Ticket_DataAccess
{
    public class BookingRepository : IBookingRepository
    {
        private List<BusBooking> _bookingList;

        public BusBooking AddBusBook(BusBooking booking)
        {
            booking.Id = _bookingList.Max(x => x.Id);
            _bookingList.Add(booking);
            return booking;
        }

        public BusBooking GetBusBooking(int id)
        {
            BusBooking busBooking = _bookingList.FirstOrDefault(x => x.Id == id);
            return busBooking;
        }
        
        public IEnumerable<BusBooking> GetAllBusBooking()
        {
            return _bookingList;
        }

        public BusBooking DeleteBusBooking(int Id)
        {
            BusBooking booking = _bookingList.FirstOrDefault(x => x.Id == Id);
            _bookingList.Remove(booking);
            return booking;
        }


    }
}
