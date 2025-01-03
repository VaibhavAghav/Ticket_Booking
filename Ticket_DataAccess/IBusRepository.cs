using Ticket_Model;

namespace Ticket_DataAccess
{
    public interface IBusRepository
    {
        Bus AddBus(Bus bus);
        IEnumerable<Bus> GetAllBuses();
        Bus GetBus(int Id);
        Bus UpdateBus(Bus bus);
        Bus DeleteBus(int Id);
    }
}