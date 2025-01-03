using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket_Model;

namespace Ticket_DataAccess
{
    public class SQLRepository:IBusRepository , ICityRepository , IRouteRepository,IStopRepository, IBookingRepository
    {
        private readonly AppDbContext applicationDbContext;
        public SQLRepository(AppDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }


        #region BUS 
        public Bus AddBus(Bus bus)
        {
            applicationDbContext.Buses.Add(bus);
            applicationDbContext.SaveChanges();
            return bus;
        }

        public IEnumerable<Bus> GetAllBuses()
        {
            return applicationDbContext.Buses;
        }

        public Bus GetBus(int Id)
        {
            Bus bus = applicationDbContext.Buses.FirstOrDefault(e => e.Id == Id);
            return bus;
        }

        public Bus UpdateBus(Bus busupdate)
        {
            applicationDbContext.Buses.Attach(busupdate);
            applicationDbContext.SaveChanges();
            return busupdate;
        }

        public Bus DeleteBus(int Id)
        {
            Bus bus = applicationDbContext.Buses.FirstOrDefault(e => e.Id == Id);
            applicationDbContext.Buses.Remove(bus);
            applicationDbContext.SaveChanges();
            return bus;

        }

        #endregion

        #region CITY

        public City AddCity(City city)
        {
            applicationDbContext.City.Add(city);
            applicationDbContext.SaveChanges();
            return city;
        }

        public IEnumerable<City> GetAllCities()
        {
            return applicationDbContext.City;
        }

        public City GetCity(int Id)
        {
            City city = applicationDbContext.City.FirstOrDefault(e => e.Id == Id);
            return city;
        }

        public City UpdateCity(City city)
        {
            applicationDbContext.City.FirstOrDefault(x => x.Id == city.Id);
            applicationDbContext.SaveChanges();
            return city;
        }

        public City DeleteCity(int Id)
        {
            City city = applicationDbContext.City.FirstOrDefault(x => x.Id == Id);
            applicationDbContext.City.Remove(city);
            applicationDbContext.SaveChanges();
            return city;
        }


        #endregion

        #region BUS ROUTE

        public BusRoute AddRoute(BusRoute busRoute)
        {
            applicationDbContext.BusRoute.Add(busRoute);
            applicationDbContext.SaveChanges();
            return busRoute;
        }

        public IEnumerable<BusRoute> GetAllRoutes()
        {
            return applicationDbContext.BusRoute;
        }

        public BusRoute GetRoute(int Id)
        {
            BusRoute route = applicationDbContext.BusRoute.FirstOrDefault(x => x.Id == Id);
            return route;
        }

        public BusRoute UpdateRoute(BusRoute busRoute)
        {
            applicationDbContext.BusRoute.FirstOrDefault(x => x.Id == busRoute.Id);
            applicationDbContext.SaveChanges();
            return busRoute;
        }

        public BusRoute DeleteRoute(int Id)
        {
            BusRoute busRoute = applicationDbContext.BusRoute.FirstOrDefault(x => x.Id == Id);
            applicationDbContext.BusRoute.Remove(busRoute);
            applicationDbContext.SaveChanges();
            return busRoute;
        }


        #endregion

        #region BUS STOP

        public BusStop AddStop(BusStop busStop)
        {
            applicationDbContext.BusStop.Add(busStop);
            applicationDbContext.SaveChanges();
            return busStop;
        }

        public IEnumerable<BusStop> GetAllStops()
        {
            return applicationDbContext.BusStop;
        }

        public BusStop GetStop(int Id)
        {
            BusStop busStop = applicationDbContext.BusStop.FirstOrDefault(x => x.Id == Id);
            return busStop;
        }

        public BusStop UpdateStop(BusStop busStop)
        {
            applicationDbContext.BusStop.FirstOrDefault(x => x.Id == busStop.Id);
            applicationDbContext.SaveChanges();
            return busStop;
        }

        public BusStop DeleteStop(int Id)
        {
            BusStop busStop = applicationDbContext.BusStop.FirstOrDefault(x => x.Id == Id);
            applicationDbContext.BusStop.Remove(busStop);
            applicationDbContext.SaveChanges();
            return busStop;
        }


        #endregion

        #region BUS BOOKING
        public BusBooking AddBusBook(BusBooking busBooking)
        {
            applicationDbContext.BusBooking.Add(busBooking);
            applicationDbContext.SaveChanges();
            return busBooking;
        }

        public IEnumerable<BusBooking> GetAllBusBooking()
        {
            return applicationDbContext.BusBooking ;
        }

        public BusBooking GetBusBooking(int Id)
        {
            BusBooking busBooking = applicationDbContext.BusBooking.FirstOrDefault(x => x.Id == Id);
            return busBooking;
        }

        public BusBooking DeleteBusBooking(int Id)
        {
            BusBooking busBooking = applicationDbContext.BusBooking.FirstOrDefault(x => x.Id == Id);
            applicationDbContext.BusBooking.Remove(busBooking);
            applicationDbContext.SaveChanges();
            return busBooking;
        }

        #endregion

    }
}
