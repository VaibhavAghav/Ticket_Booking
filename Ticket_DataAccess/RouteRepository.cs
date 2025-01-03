using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket_Model;

namespace Ticket_DataAccess
{
    public class RouteRepository : IRouteRepository
    {
        private List<BusRoute> busRouteList;

        public BusRoute AddRoute(BusRoute busRoute)
        {
            busRoute.Id = busRouteList.Max(x => x.Id) + 1;
            busRouteList.Add(busRoute);
            return busRoute;
        }

        public IEnumerable<BusRoute> GetAllRoutes()
        {
            return busRouteList;
        }

        public BusRoute GetRoute(int Id)
        {
            BusRoute busRoute = busRouteList.FirstOrDefault(x => x.Id == Id);
            return busRoute;
        }

        public BusRoute UpdateRoute(BusRoute busRoute)
        {
            BusRoute updatedbusRoute = busRouteList.FirstOrDefault(x => x.Id == busRoute.Id);
            if (updatedbusRoute == null)
            {
                updatedbusRoute.BusId = busRoute.BusId;
                updatedbusRoute.StartCityId = busRoute.StartCityId;
                updatedbusRoute.DestinationCityId = busRoute.DestinationCityId;
                updatedbusRoute.StartTime = busRoute.StartTime; 
                updatedbusRoute.ReachedTime = busRoute.ReachedTime;
            }
            return updatedbusRoute;
        }

        public BusRoute DeleteRoute(int Id)
        {
            BusRoute busRoute = busRouteList.FirstOrDefault(x => x.Id == Id);
            busRouteList.Remove(busRoute);
            return busRoute;
        }
    }
}
