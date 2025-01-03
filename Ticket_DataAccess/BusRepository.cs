using Ticket_Model;

namespace Ticket_DataAccess
{
    public class BusRepository : IBusRepository
    {
        private List<Bus> busList;

        public IEnumerable<Bus> GetAllBuses()
        {
            return busList;
        }

        public Bus AddBus(Bus bus)
        {
            bus.Id = busList.Max(x => x.Id) + 1;
            busList.Add(bus);
            return bus;
        }

        public Bus GetBus(int Id)
        {
            return busList.FirstOrDefault(x => x.Id == Id);
        }

        public Bus UpdateBus(Bus busupdate)
        {
            Bus bus = busList.FirstOrDefault(e => e.Id == busupdate.Id);
            if (bus != null)
            {
                bus.BusNumber = busupdate.BusNumber;
                bus.SeatCapacity = busupdate.SeatCapacity;
            }
            return bus;

        }

        public Bus DeleteBus(int Id)
        {
            Bus bus = busList.FirstOrDefault(e => e.Id == Id);
            busList.Remove(bus);
            return bus;
        }
    }
}
