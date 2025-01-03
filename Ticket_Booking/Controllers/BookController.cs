using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Ticket_Booking.ViewModel.BookViewModel;
using Ticket_Model;
using Ticket_DataAccess;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;

namespace Ticket_Booking.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IBusRepository _busRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IBookingRepository _bookRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public BookController(AppDbContext appDbContext, ICityRepository cityRepository,
                              IRouteRepository routeRepository, IBookingRepository bookRepository,
                              IBusRepository busRepository, UserManager<IdentityUser> userManager, 
                              SignInManager<IdentityUser> signInManager)
        {
            _busRepository = busRepository;
            _cityRepository = cityRepository;
            _routeRepository = routeRepository;
            _appDbContext = appDbContext;
            _bookRepository = bookRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        #region OWN TICKET

        public IActionResult OldTicket()
        {
            var user = User.Identity.Name;
            var tickets = from booking in _appDbContext.BusBooking
                          join br in _appDbContext.BusRoute on booking.BusRouteId equals br.Id
                          join b in _appDbContext.Buses on br.BusId equals b.Id
                          join sc in _appDbContext.City on booking.StartCityId equals sc.Id
                          join ec in _appDbContext.City on booking.DestinationCityId equals ec.Id
                          where(booking.PassengerName.StartsWith(user))
                          select new GetAllTicketViewModel
                          {
                              Id = booking.Id,
                              Name = booking.PassengerName.Substring(user.Length).Trim(),
                              Seats = booking.NoSeatBooked,
                              BueRouteId = booking.BusRouteId,
                              BusId = b.BusNumber,
                              StartCity = sc.CityName,
                              DestinationCity = ec.CityName,
                              StartTime = booking.BookingDateTime,
                              ReachedTime = booking.ReachingDateTime
                          };

            var viewModel = tickets.ToList();
            return View(viewModel);
        }

        #endregion


        #region ALL TICKET

        public IActionResult ShowTicket()
        {
            try
            {
                var user = User.Identity.Name;
                var tickets = from booking in _appDbContext.BusBooking
                          join br in _appDbContext.BusRoute on booking.BusRouteId equals br.Id
                          join b in _appDbContext.Buses on br.BusId equals b.Id
                          join sc in _appDbContext.City on booking.StartCityId equals sc.Id
                          join ec in _appDbContext.City on booking.DestinationCityId equals ec.Id
                          select new GetAllTicketViewModel
                          {
                              Id = booking.Id,
                              Name = booking.PassengerName.Substring(user.Length).Trim(),
                              Seats = booking.NoSeatBooked,
                              BueRouteId = booking.BusRouteId,
                              BusId = b.BusNumber,
                              StartCity = sc.CityName,
                              DestinationCity = ec.CityName,
                              StartTime = booking.BookingDateTime,
                              ReachedTime = booking.ReachingDateTime
                          };

            var viewModel = tickets.ToList();
            return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion

        
        #region SEARCH BUS
        [HttpGet]
        public IActionResult BookTicket()
        {
            try
            {
                var cities = _cityRepository.GetAllCities()
               .Select(c => new SelectListItem
               {
                   Value = c.Id.ToString(),
                   Text = c.CityName
               }).ToList();

                var model = new SearchBookTicketViewModel
                {
                    Cities = cities
                };

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        [HttpPost]
        public IActionResult SearchBusRoutes(int fromCityId, int toCityId, DateTime bookingDateTime)
        {
            try
            {
                var startCity = _cityRepository.GetCity(fromCityId);
                var endCity = _cityRepository.GetCity(toCityId);

                DateTime currentDateTime = DateTime.Now;
                DateTime currentDate = currentDateTime.Date;

                var routes = (from br in _appDbContext.BusRoute
                              join b in _appDbContext.Buses on br.BusId equals b.Id
                              join sc in _appDbContext.City on br.StartCityId equals sc.Id
                              join ec in _appDbContext.City on br.DestinationCityId equals ec.Id
                              join sbs in _appDbContext.BusStop on br.Id equals sbs.BusRoutId
                              join ebs in _appDbContext.BusStop on br.Id equals ebs.BusRoutId
                              where fromCityId != toCityId &&
                                    (((sbs.AddCityId == fromCityId && ebs.AddCityId == toCityId) && sbs.StopTime < ebs.StopTime) ||
                                     (br.StartCityId == fromCityId && br.DestinationCityId == toCityId) ||
                                     (br.StartCityId == fromCityId && ebs.AddCityId == toCityId) ||
                                     (sbs.AddCityId == fromCityId && br.DestinationCityId == toCityId))
                              select new
                              {
                                  br.Id,
                                  BusNumber = b.BusNumber,
                                  StartTime = ((sbs.AddCityId == fromCityId && ebs.AddCityId == toCityId) && sbs.StopTime < ebs.StopTime ? sbs.StopTime :
                                              (br.StartCityId == fromCityId && br.DestinationCityId == toCityId) ? br.StartTime :
                                              (br.StartCityId == fromCityId && ebs.AddCityId == toCityId) ? br.StartTime :
                                              (sbs.AddCityId == fromCityId && br.DestinationCityId == toCityId) ? sbs.StopTime :
                                              DateTime.MinValue),
                                  ReachedTime = ((sbs.AddCityId == fromCityId && ebs.AddCityId == toCityId) && sbs.StopTime < ebs.StopTime ? ebs.StopTime :
                                                (br.StartCityId == fromCityId && br.DestinationCityId == toCityId) ? br.ReachedTime :
                                                (br.StartCityId == fromCityId && ebs.AddCityId == toCityId) ? ebs.StopTime :
                                                (sbs.AddCityId == fromCityId && br.DestinationCityId == toCityId) ? br.ReachedTime :
                                                DateTime.MinValue),
                                  StartCity = startCity.CityName,
                                  DestinationCity = endCity.CityName
                              }).Distinct().ToList();

                var result = routes.Select(route => new
                {
                    route.Id,
                    route.BusNumber,
                    StartTime = bookingDateTime.Date < currentDate ? route.StartTime : new DateTime(bookingDateTime.Year, bookingDateTime.Month, bookingDateTime.Day, route.StartTime.Hour, route.StartTime.Minute, route.StartTime.Second),
                    ReachedTime = bookingDateTime.Date < currentDate ? route.ReachedTime : new DateTime(bookingDateTime.Year, bookingDateTime.Month, bookingDateTime.Day, route.ReachedTime.Hour, route.ReachedTime.Minute, route.ReachedTime.Second),
                    route.StartCity,
                    route.DestinationCity,
                    IsDeparted = bookingDateTime.Date <= currentDate && currentDateTime.TimeOfDay > route.StartTime.TimeOfDay
                }).ToList();

                return Json(result);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }


        #endregion

        
        #region BOOK BUS

        #region GET METHOD
        [HttpGet]
        public IActionResult BookBus(int id, string busNumber, string startTime, string reachedTime, string startCity, string destinationCity)
        {
            try
            {
                DateTime startDateTime = DateTime.Parse(startTime);

                var scdb = _appDbContext.City.FirstOrDefault(x => x.CityName == startCity);
                var ecdb = _appDbContext.City.FirstOrDefault(x => x.CityName == destinationCity);

                var busroute = _routeRepository.GetRoute(id);

                var busStops = _appDbContext.BusStop
                     .Where(bs => bs.BusRoutId == id)
                     .OrderBy(bs => bs.StopTime)
                     .ToList();

                var startStopIndex = busStops.FindIndex(bs => bs.AddCityId == scdb.Id);
                var endStopIndex = busStops.FindIndex(bs => bs.AddCityId == ecdb.Id);

                var overlappingBookings = new List<BusBooking>();

                if (busroute.StartCityId == scdb.Id && busroute.DestinationCityId == ecdb.Id)
                {
                    overlappingBookings = _appDbContext.BusBooking
                                            .Where(sd => sd.BusRouteId == id &&
                                                   sd.BookingDateTime.Date == startDateTime.Date
                                                   ).ToList();
                }
                else if (startStopIndex != -1 && endStopIndex != -1 && startStopIndex < endStopIndex)
                {
                    var middleStops = new List<int>();
                    var beforemiddleStops = new List<int>();
                    var abovemiddleStops = new List<int>();
                    var startstopcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == scdb.Id);
                    var endstopcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == ecdb.Id);
                    foreach (var stop in busStops)
                    {
                        if (startstopcity.StopTime < stop.StopTime && endstopcity.StopTime > stop.StopTime)
                        {
                            middleStops.Add(stop.AddCityId);
                        }
                        else if (endstopcity.StopTime < stop.StopTime)
                        {
                            abovemiddleStops.Add(stop.AddCityId);
                        }
                        else if (startstopcity.StopTime > stop.StopTime)
                        {
                            beforemiddleStops.Add(stop.AddCityId);
                        }

                    }
                    overlappingBookings = _appDbContext.BusBooking
                                            .Where(sd => ((sd.BusRouteId == id) && (startDateTime.Date == sd.BookingDateTime.Date)) &&
                                            (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == busroute.DestinationCityId) ||   //pun-mum-2,
                                            (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == endstopcity.AddCityId) ||                      //pun-nas-2,
                                            (sd.StartCityId == scdb.Id && sd.DestinationCityId == busroute.DestinationCityId) ||               //  // lon-mum-2,2,2
                                            (sd.StartCityId == scdb.Id && sd.DestinationCityId == endstopcity.AddCityId) ||                                 //  //lon-nask-2,
                                            (sd.StartCityId == busroute.StartCityId && middleStops.Contains(sd.DestinationCityId)) ||
                                            (sd.StartCityId == busroute.StartCityId && abovemiddleStops.Contains(sd.DestinationCityId)) ||
                                            (sd.StartCityId == scdb.Id && middleStops.Contains(sd.DestinationCityId)) ||
                                            (sd.StartCityId == scdb.Id && abovemiddleStops.Contains(sd.DestinationCityId)) ||
                                            (middleStops.Contains(sd.StartCityId) && middleStops.Contains(sd.DestinationCityId)) ||
                                            (middleStops.Contains(sd.StartCityId) && abovemiddleStops.Contains(sd.DestinationCityId))
                                            ).ToList();
                }
                else if (startStopIndex == -1 && endStopIndex != -1)
                {

                    var middleStops = new List<int>();
                    var abovemiddleStops = new List<int>();
                    var endstopcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == ecdb.Id);

                    var allStops = busStops.Select(bs => bs.AddCityId).ToList();

                    foreach (var stop in busStops)
                    {
                        if (stop.StopTime > endstopcity.StopTime)
                        {
                            abovemiddleStops.Add(stop.AddCityId);
                        }
                        middleStops.Add(stop.AddCityId);
                    }

                    overlappingBookings = _appDbContext.BusBooking
                       .Where(sd => (sd.BusRouteId == id) && (startDateTime.Date == sd.BookingDateTime.Date))
                       .ToList();

                    overlappingBookings = overlappingBookings
                        .Where(sd =>
                            (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == busroute.DestinationCityId) ||
                            (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == endstopcity.AddCityId) ||
                            (sd.StartCityId == busroute.StartCityId && allStops.Contains(sd.DestinationCityId)) ||
                            (middleStops.Contains(sd.StartCityId) && middleStops.Contains(sd.DestinationCityId)) ||
                            (middleStops.Contains(sd.StartCityId) && sd.DestinationCityId == busroute.DestinationCityId) ||
                            (middleStops.Contains(sd.StartCityId) && abovemiddleStops.Contains(sd.DestinationCityId)))
                        .ToList();

                }
                else if (startStopIndex != -1 && endStopIndex == -1)
                {

                    var middleStops = new List<int>();
                    var allStops = busStops.Select(bs => bs.AddCityId).ToList();
                    var stopstartcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == scdb.Id);

                    foreach (var stop in busStops)
                    {
                        if (stop.StopTime > stopstartcity.StopTime)
                        {
                            middleStops.Add(stop.AddCityId);
                        }
                    }

                    overlappingBookings = _appDbContext.BusBooking
                       .Where(sd => ((sd.BusRouteId == id) && (startDateTime.Date == sd.BookingDateTime.Date)))
                       .ToList();

                    overlappingBookings = overlappingBookings
                        .Where(sd =>
                                (sd.StartCityId == scdb.Id && sd.DestinationCityId == busroute.DestinationCityId) ||
                                (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == busroute.DestinationCityId) ||
                                (allStops.Contains(sd.StartCityId) && middleStops.Contains(sd.DestinationCityId)) ||
                                (sd.StartCityId == busroute.StartCityId && middleStops.Contains(sd.DestinationCityId)) ||
                                (allStops.Contains(sd.StartCityId) && sd.DestinationCityId == busroute.DestinationCityId)
                                ).ToList();

                }
                else
                {
                    overlappingBookings = _appDbContext.BusBooking
                                           .Where(sd => ((sd.BusRouteId == id) && (startDateTime.Date == sd.BookingDateTime.Date)) &&
                                                sd.BookingDateTime.Date == startDateTime.Date &&
                                                (sd.StartCityId == scdb.Id && ecdb.Id == sd.DestinationCityId ||
                                                (sd.StartCityId == busroute.StartCityId && busroute.DestinationCityId == sd.DestinationCityId)))
                                                .ToList();
                }

                int numSeatsBooked = overlappingBookings.Sum(sd => sd.NoSeatBooked);

                var route = _routeRepository.GetRoute(id);
                var bus = _busRepository.GetBus(route.BusId);

                int remainingSeats = bus.SeatCapacity - numSeatsBooked;

                var model = new BookTicketViewModel
                {
                    BueRouteId = id,
                    BusId = busNumber,
                    StartTime = DateTime.Parse(startTime),
                    ReachedTime = DateTime.Parse(reachedTime),
                    StartCity = startCity,
                    DestinationCity = destinationCity
                };

                ViewBag.Remain = remainingSeats;
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion

        #region POST

        [HttpPost]
        public IActionResult BookBus(BookTicketViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var scdb = _appDbContext.City.FirstOrDefault(x => x.CityName == model.StartCity);
                    var ecdb = _appDbContext.City.FirstOrDefault(x => x.CityName == model.DestinationCity);

                    var busroute = _routeRepository.GetRoute(model.BueRouteId);

                    var busStops = _appDbContext.BusStop
                         .Where(bs => bs.BusRoutId == model.BueRouteId)
                         .OrderBy(bs => bs.StopTime)
                         .ToList();

                    var startStopIndex = busStops.FindIndex(bs => bs.AddCityId == scdb.Id);
                    var endStopIndex = busStops.FindIndex(bs => bs.AddCityId == ecdb.Id);

                    var overlappingBookings = new List<BusBooking>();

                    if (busroute.StartCityId == scdb.Id && busroute.DestinationCityId == ecdb.Id)
                    {
                        overlappingBookings = _appDbContext.BusBooking
                                                .Where(sd => sd.BusRouteId == model.BueRouteId &&
                                                       sd.BookingDateTime.Date == model.StartTime.Date
                                                       ).ToList();
                    }
                    else if (startStopIndex != -1 && endStopIndex != -1 && startStopIndex < endStopIndex)
                    {
                        var middleStops = new List<int>();
                        var beforemiddleStops = new List<int>();
                        var abovemiddleStops = new List<int>();
                        var startstopcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == scdb.Id);
                        var endstopcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == ecdb.Id);
                        foreach (var stop in busStops)
                        {
                            if (startstopcity.StopTime < stop.StopTime && endstopcity.StopTime > stop.StopTime)
                            {
                                middleStops.Add(stop.AddCityId);
                            }
                            else if (endstopcity.StopTime < stop.StopTime)
                            {
                                abovemiddleStops.Add(stop.AddCityId);
                            }
                            else if (startstopcity.StopTime > stop.StopTime)
                            {
                                beforemiddleStops.Add(stop.AddCityId);
                            }

                        }
                        overlappingBookings = _appDbContext.BusBooking
                                                .Where(sd => ((sd.BusRouteId == model.BueRouteId) && (model.StartTime.Date == sd.BookingDateTime.Date)) &&
                                                (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == busroute.DestinationCityId) ||   //pun-mum-2,
                                                (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == endstopcity.AddCityId) ||                      //pun-nas-2,
                                                (sd.StartCityId == scdb.Id && sd.DestinationCityId == busroute.DestinationCityId) ||               //  // lon-mum-2,2,2
                                                (sd.StartCityId == scdb.Id && sd.DestinationCityId == endstopcity.AddCityId) ||                                 //  //lon-nask-2,
                                                (sd.StartCityId == busroute.StartCityId && middleStops.Contains(sd.DestinationCityId)) ||
                                                (sd.StartCityId == busroute.StartCityId && abovemiddleStops.Contains(sd.DestinationCityId)) ||
                                                (sd.StartCityId == scdb.Id && middleStops.Contains(sd.DestinationCityId)) ||
                                                (sd.StartCityId == scdb.Id && abovemiddleStops.Contains(sd.DestinationCityId)) ||
                                                (middleStops.Contains(sd.StartCityId) && middleStops.Contains(sd.DestinationCityId)) ||
                                                (middleStops.Contains(sd.StartCityId) && abovemiddleStops.Contains(sd.DestinationCityId))
                                                ).ToList();
                    }
                    else if (startStopIndex == -1 && endStopIndex != -1)
                    {

                        var middleStops = new List<int>();
                        var abovemiddleStops = new List<int>();
                        var endstopcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == ecdb.Id);

                        var allStops = busStops.Select(bs => bs.AddCityId).ToList();

                        foreach (var stop in busStops)
                        {
                            if (stop.StopTime > endstopcity.StopTime)
                            {
                                abovemiddleStops.Add(stop.AddCityId);
                            }
                            middleStops.Add(stop.AddCityId);
                        }

                        overlappingBookings = _appDbContext.BusBooking
                           .Where(sd => (sd.BusRouteId == model.BueRouteId) && (model.StartTime.Date == sd.BookingDateTime.Date))
                           .ToList();

                        overlappingBookings = overlappingBookings
                            .Where(sd =>
                                (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == busroute.DestinationCityId) ||
                                (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == endstopcity.AddCityId) ||
                                (sd.StartCityId == busroute.StartCityId && allStops.Contains(sd.DestinationCityId)) ||
                                (middleStops.Contains(sd.StartCityId) && middleStops.Contains(sd.DestinationCityId)) ||
                                (middleStops.Contains(sd.StartCityId) && sd.DestinationCityId == busroute.DestinationCityId) ||
                                (middleStops.Contains(sd.StartCityId) && abovemiddleStops.Contains(sd.DestinationCityId)))
                            .ToList();


                    }
                    else if (startStopIndex != -1 && endStopIndex == -1)
                    {

                        var middleStops = new List<int>();
                        var allStops = busStops.Select(bs => bs.AddCityId).ToList();
                        var stopstartcity = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == scdb.Id);

                        foreach (var stop in busStops)
                        {
                            if (stop.StopTime > stopstartcity.StopTime)
                            {
                                middleStops.Add(stop.AddCityId);
                            }
                        }

                        overlappingBookings = _appDbContext.BusBooking
                           .Where(sd => ((sd.BusRouteId == model.BueRouteId) && (model.StartTime.Date == sd.BookingDateTime.Date)))
                           .ToList();

                        overlappingBookings = overlappingBookings
                            .Where(sd =>
                                    (sd.StartCityId == scdb.Id && sd.DestinationCityId == busroute.DestinationCityId) ||
                                    (sd.StartCityId == busroute.StartCityId && sd.DestinationCityId == busroute.DestinationCityId) ||
                                    (allStops.Contains(sd.StartCityId) && middleStops.Contains(sd.DestinationCityId)) ||
                                    (sd.StartCityId == busroute.StartCityId && middleStops.Contains(sd.DestinationCityId)) ||
                                    (allStops.Contains(sd.StartCityId) && sd.DestinationCityId == busroute.DestinationCityId)
                                    ).ToList();

                    }
                    else
                    {
                        overlappingBookings = _appDbContext.BusBooking
                                               .Where(sd => ((sd.BusRouteId == model.BueRouteId) && (model.StartTime.Date == sd.BookingDateTime.Date)) &&
                                                    sd.BookingDateTime.Date == model.StartTime.Date &&
                                                    (sd.StartCityId == scdb.Id && ecdb.Id == sd.DestinationCityId ||
                                                    (sd.StartCityId == busroute.StartCityId && busroute.DestinationCityId == sd.DestinationCityId)))
                                                    .ToList();
                    }

                    int numSeatsBooked = overlappingBookings.Sum(sd => sd.NoSeatBooked);

                    var route = _routeRepository.GetRoute(model.BueRouteId);
                    var bus = _busRepository.GetBus(route.BusId);

                    int remainingSeats = bus.SeatCapacity - numSeatsBooked;
                    DateTime bookingDate = model.StartTime.Date;
                    var segmentAvailability = _appDbContext.BusSeatAvailablity
                        .FirstOrDefault(bd => bd.BusRouteId == model.BueRouteId &&
                                              bd.Date.Date == bookingDate &&
                                              bd.StartCityId == scdb.Id && bd.DestinationCityId == ecdb.Id);

                    if (model.Seats > remainingSeats)
                    {
                        ModelState.AddModelError("Seats", $"Remaining seats from {model.StartCity} to {model.DestinationCity}: {remainingSeats}");
                        return View(model);
                    }

                    string namePattern = @"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$";
                    if (!Regex.IsMatch(model.Name, namePattern))
                    {
                        ModelState.AddModelError("Name", "Please fill in a valid name.");
                        return View(model);
                    }

                    var busBooking = new BusBooking
                    {
                        BusRouteId = model.BueRouteId,
                        StartCityId = scdb.Id,
                        DestinationCityId = ecdb.Id,
                        NoSeatBooked = model.Seats,
                        PassengerName = $"{User.Identity.Name} {model.Name}",
                        BookingDateTime = model.StartTime,
                        ReachingDateTime = model.ReachedTime
                    };

                    if (segmentAvailability == null)
                    {
                        segmentAvailability = new BusDaily
                        {
                            BusRouteId = model.BueRouteId,
                            Date = bookingDate,
                            StartCityId = scdb.Id,
                            DestinationCityId = ecdb.Id,
                            AvailableSeats = bus.SeatCapacity
                        };

                        _appDbContext.BusSeatAvailablity.Add(segmentAvailability);

                    }

                    _bookRepository.AddBusBook(busBooking);
                    segmentAvailability.AvailableSeats -= model.Seats;
                    _appDbContext.BusSeatAvailablity.Update(segmentAvailability);
                    _appDbContext.SaveChanges();

                    return RedirectToAction("BookTicket");
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        #endregion

        #endregion


        #region DELETE TICKET

        [HttpPost]
        public IActionResult DeleteTicket(int id)
        {
            try
            {
                var ticket = _appDbContext.BusBooking.FirstOrDefault(b => b.Id == id);

                if (ticket != null)
                {
                    var currentTime = DateTime.Now;
                    var bookingTime = ticket.BookingDateTime;

                    if ((bookingTime - currentTime).TotalMinutes <= 60)
                    {
                        return RedirectToAction("Error", "Bus", new { message = "Cannot delete booking within 1 hour of the departure time." });
                    }

                    _appDbContext.BusBooking.Remove(ticket);
                    _appDbContext.SaveChanges();
                }

                return RedirectToAction("ShowTicket");

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

        }

        #endregion



    }
}
