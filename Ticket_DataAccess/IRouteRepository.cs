using Ticket_Model;

namespace Ticket_DataAccess
{
    public interface IRouteRepository
    {
        BusRoute AddRoute(BusRoute bus);
        IEnumerable<BusRoute> GetAllRoutes();
        BusRoute GetRoute(int Id);
        BusRoute UpdateRoute(BusRoute bus);
        BusRoute DeleteRoute(int Id);
    }
}