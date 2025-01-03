using Ticket_Model;

namespace Ticket_DataAccess
{
    public interface IStopRepository
    {
        BusStop AddStop(BusStop busStop);
        IEnumerable<BusStop> GetAllStops();
        BusStop GetStop(int Id);
        BusStop UpdateStop(BusStop busStop);
        BusStop DeleteStop(int Id);

    }
}