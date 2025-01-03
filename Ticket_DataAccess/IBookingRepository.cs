using Ticket_Model;

namespace Ticket_DataAccess
{
    public interface IBookingRepository
    {
        BusBooking AddBusBook(BusBooking busBooking);
        IEnumerable<BusBooking> GetAllBusBooking();
        BusBooking GetBusBooking(int Id);
        BusBooking DeleteBusBooking(int Id);
    }
}