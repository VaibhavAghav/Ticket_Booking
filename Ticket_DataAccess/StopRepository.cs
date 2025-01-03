using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket_Model;

namespace Ticket_DataAccess
{
    public class StopRepository : IStopRepository
    {
        private List<BusStop> busStopList;

        public BusStop AddStop(BusStop busStop)
        {
            busStop.Id = busStopList.Max(x => x.Id) + 1;
            busStopList.Add(busStop);
            return busStop;
        }

        public IEnumerable<BusStop> GetAllStops()
        {
            return busStopList;
        }

        public BusStop GetStop(int Id)
        {
            BusStop busStop = busStopList.FirstOrDefault(x => x.Id == Id);
            return busStop;
        }

        public BusStop UpdateStop(BusStop busStop)
        {
            BusStop updatedbusStop = busStopList.FirstOrDefault(x => x.Id == busStop.Id);
            if (updatedbusStop != null)
            {
                updatedbusStop.AddCityId = busStop.AddCityId;
                updatedbusStop.StopTime = busStop.StopTime;
                updatedbusStop.BusRoutId = busStop.BusRoutId;
            }

            return updatedbusStop;
        }

        public BusStop DeleteStop(int Id)
        {
            BusStop busStop = busStopList.FirstOrDefault(x => x.Id == Id);
            busStopList.Remove(busStop);
            return busStop;
        }

       
    }
}
