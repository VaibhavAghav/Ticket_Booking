using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Ticket_Booking.ViewModel.RouteViewModel;
using Ticket_Booking.ViewModel.StopViewModel;
using Ticket_DataAccess;
using Ticket_Model;

namespace Ticket_Booking.Controllers
{
 

    public class StopController : Controller
    {
        private readonly IStopRepository _stopRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IBusRepository _busRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly AppDbContext _appDbContext;
        public StopController(IStopRepository stopRepository, ICityRepository cityRepository, 
                              IBusRepository busRepository, IRouteRepository routeRepository,
                              AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _stopRepository = stopRepository;
            _cityRepository = cityRepository;
            _busRepository = busRepository;
            _routeRepository = routeRepository;

        }


        #region ALL STOP
        public IActionResult GetAllStop(string busNumber)
        {
            try
            {
                var query = from br in _appDbContext.BusStop
                            join b in _appDbContext.BusRoute on br.BusRoutId equals b.Id
                            join d in _appDbContext.Buses on b.BusId equals d.Id
                            join ec in _appDbContext.City on br.AddCityId equals ec.Id
                            select new
                            {
                                Id = br.Id,
                                BusId = d.BusNumber,
                                Stop = ec.CityName,
                                StopTime = br.StopTime
                            };

                if (!string.IsNullOrEmpty(busNumber))
                {
                    query = query.Where(r => r.BusId.ToLower().Contains(busNumber.ToLower()));
                }

                var results = query.ToList();

                var showRouteList = results.Select(r => new GetAllStopViewModel
                {
                    Id = r.Id,
                    BusNumber = r.BusId,
                    StopCity = r.Stop,
                    StopTime = r.StopTime
                }).ToList();

                return View(showRouteList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage","Bus", new { message = ex.Message });
            }
        }


        #endregion


        #region ADD STOP BY BUSROUTE

        [HttpGet]
        public ActionResult AddStop(int Id)
        {
            try
            {
                var busRoute = _routeRepository.GetRoute(Id);
                var cities = _cityRepository.GetAllCities();
                var allStops = _stopRepository.GetAllStops();
                var addedStops = allStops.Where(s => s.BusRoutId == Id).Select(s => s.AddCityId).ToList();
                var bus = _busRepository.GetBus(busRoute.BusId);
                var sCity = _cityRepository.GetCity(busRoute.StartCityId);
                var dCity = _cityRepository.GetCity(busRoute.DestinationCityId);

                var stops = allStops.Where(s => s.BusRoutId == Id).Select(s => new ViewModel.StopViewModel.StopViewModel
                {
                    CityName = _cityRepository.GetCity(s.AddCityId).CityName,
                    StopTime = s.StopTime
                }).ToList();

                var viewModel = new AddStopViewModel
                {
                    Id = busRoute.Id,
                    BusNumber = bus.BusNumber,
                    StartCity = sCity.CityName,
                    DestinationCity = dCity.CityName,
                    StartTime = busRoute.StartTime,
                    ReachedTime = busRoute.ReachedTime,
                    City = cities.Where(c => c.Id != busRoute.StartCityId && c.Id != busRoute.DestinationCityId && !addedStops.Contains(c.Id))
                                 .Select(c => new SelectListItem
                                 {
                                     Value = c.Id.ToString(),
                                     Text = c.CityName
                                 }),
                    Stops = stops
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

        }
               

        [HttpPost]
        public IActionResult AddStop(AddStopViewModel model)
        {
            try
            {
                int Id = model.Id;
                var busRoute = _routeRepository.GetRoute(Id);
                var cities = _cityRepository.GetAllCities();
                var allStops = _stopRepository.GetAllStops();
                var addedStops = allStops.Where(s => s.BusRoutId == Id).Select(s => s.AddCityId).ToList();
                var bus = _busRepository.GetBus(busRoute.BusId);
                var sCity = _cityRepository.GetCity(busRoute.StartCityId);
                var dCity = _cityRepository.GetCity(busRoute.DestinationCityId);

                var stops = allStops.Where(s => s.BusRoutId == Id).Select(s => new ViewModel.StopViewModel.StopViewModel
                {
                    CityName = _cityRepository.GetCity(s.AddCityId).CityName,
                    StopTime = s.StopTime
                }).ToList();

                var viewModel = new AddStopViewModel
                {
                    Id = busRoute.Id,
                    BusNumber = bus.BusNumber,
                    StartCity = sCity.CityName,
                    DestinationCity = dCity.CityName,
                    StartTime = busRoute.StartTime,
                    ReachedTime = busRoute.ReachedTime,
                    City = cities.Where(c => c.Id != busRoute.StartCityId && c.Id != busRoute.DestinationCityId && !addedStops.Contains(c.Id))
                                 .Select(c => new SelectListItem
                                 {
                                     Value = c.Id.ToString(),
                                     Text = c.CityName
                                 }),
                    Stops = stops
                };

                if (ModelState.IsValid)
                {
                    var previousStop = _appDbContext.BusStop.Where(x => x.BusRoutId == model.Id);
                    foreach (var stop in previousStop)
                    {
                        if (model.StopTime < stop.StopTime)
                        {
                            var lessTimeStop = _cityRepository.GetCity(stop.AddCityId);
                            ModelState.AddModelError("StopTime", $"this stop must have more time than stop {lessTimeStop.CityName} {stop.StopTime}");
                            break;
                        }
                    }

                    if (model.StartTime > model.StopTime)
                    {
                        ModelState.AddModelError("StopTime", "Stop time cannot be before the start time of the bus.");
                    }

                    if (model.ReachedTime < model.StopTime)
                    {
                        ModelState.AddModelError("StopTime", "Stop time cannot be after the reached time of the bus.");
                    }

                    if (ModelState.IsValid)
                    {
                        var busStop = new BusStop
                        {
                            BusRoutId = model.Id,
                            AddCityId = model.StopCity,
                            StopTime = model.StopTime,
                        };

                        _stopRepository.AddStop(busStop);
                        return RedirectToAction("GetAllStop");
                    }
                }

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });

            }
            
        }

        #endregion


        #region CREATE STOP
        [HttpGet]
        public IActionResult CreateStop()
        {
            try
            {
                var allCity = _cityRepository.GetAllCities();
                var allbus = _busRepository.GetAllBuses();
                var allroute = _routeRepository.GetAllRoutes();
                List<int> presentroute = new List<int>();

                foreach (var route in allroute)
                {
                    foreach (var bus in allbus)
                    {
                        if (bus.Id == route.BusId)
                        {
                            presentroute.Add(bus.Id);
                        }
                    }
                }

                var viewModel = new CreateStopViewModel
                {
                    City = allCity.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CityName
                    }).ToList(),
                    Buses = allbus
                        .Where(bus => presentroute.Contains(bus.Id))
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.BusNumber
                        }).ToList()
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }



        [HttpPost]
        public IActionResult CreateStop(CreateStopViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var busRoute = _appDbContext.BusRoute.FirstOrDefault(x => x.BusId == viewModel.BusRouteId);
                    var beforestop = _appDbContext.BusStop.Where(x => x.BusRoutId == busRoute.Id);
                    var city = _cityRepository.GetCity(viewModel.Addcity);


                    if (viewModel.StopTime < busRoute.StartTime || viewModel.StopTime > busRoute.ReachedTime)
                    {
                        ModelState.AddModelError("StopTime", "Stop time must be in between start and reached time");
                    }
                    if (viewModel.Addcity == busRoute.StartCityId || viewModel.Addcity == busRoute.DestinationCityId)
                    {
                        ModelState.AddModelError("Addcity", "Stop city must not be start and destination city");
                    }

                    foreach (var i in beforestop)
                    {
                        if (i.AddCityId == viewModel.Addcity)
                        {
                            ModelState.AddModelError("Addcity", " " + city.CityName + " is already added in stop");
                            break;
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        var cities = _cityRepository.GetAllCities();
                        var buses = _busRepository.GetAllBuses();

                        viewModel.City = cities.Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.CityName
                        });
                        viewModel.Buses = buses.Select(b => new SelectListItem
                        {
                            Value = b.Id.ToString(),
                            Text = b.BusNumber
                        });

                        return View(viewModel);
                    }


                    BusStop busStop = new BusStop
                    {
                        AddCityId = viewModel.Addcity,
                        BusRoutId = busRoute.Id,
                        StopTime = viewModel.StopTime
                    };

                    _stopRepository.AddStop(busStop);
                    return RedirectToAction("GetAllStop");
                }
                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion


        #region EDIT STOP

        #region GET METHOD
        [HttpGet]
        public IActionResult EditStop(int id)
        {
            try
            {
                var stop = _stopRepository.GetStop(id);
                if (stop == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This stop does not exist." });
                }

                var busRoute = _routeRepository.GetRoute(stop.BusRoutId);
                if (busRoute == null)
                {
                    return NotFound();
                }


                var presentCityIds = _appDbContext.BusStop
                    .Where(s => s.BusRoutId == busRoute.Id)
                    .Select(s => s.AddCityId)
                    .Union(_appDbContext.BusRoute
                        .Where(br => br.Id == busRoute.Id)
                        .Select(br => br.StartCityId)
                        .Union(_appDbContext.BusRoute
                            .Where(br => br.Id == busRoute.Id)
                            .Select(br => br.DestinationCityId)))
                    .Distinct()
                    .ToList();


                var availableCities = _appDbContext.City
                    .Where(c => !presentCityIds.Contains(c.Id))
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CityName,
                        Selected = false
                    })
                    .ToList();


                var currentCity = _appDbContext.City
                    .Where(c => c.Id == stop.AddCityId)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CityName,
                        Selected = true
                    })
                    .FirstOrDefault();

                if (currentCity != null)
                {
                    availableCities.Add(currentCity);
                }


                var presentRoute = _appDbContext.BusRoute
                    .Join(_appDbContext.Buses,
                          br => br.BusId,
                          b => b.Id,
                          (br, b) => new SelectListItem
                          {
                              Value = br.Id.ToString(),
                              Text = b.BusNumber,
                              Selected = br.Id == stop.BusRoutId
                          })
                    .ToList();

                var viewModel = new CreateStopViewModel
                {
                    Id = stop.Id,
                    StopTime = stop.StopTime,
                    Addcity = stop.AddCityId,
                    BusRouteId = stop.BusRoutId,
                    City = availableCities.OrderBy(x => x.Text).ToList(),
                    Buses = presentRoute
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        #endregion

        [HttpPost]
        public IActionResult EditStop(CreateStopViewModel model)
        {
            try
            {
                var busRoute1 = _routeRepository.GetRoute(model.BusRouteId);
                if (ModelState.IsValid)
                {
                    if (busRoute1 == null)
                    {
                        return RedirectToAction("ErrorPage", "Bus", new { message = "This stop does not exist." });
                    }

                    if (model.Addcity == busRoute1.StartCityId || model.Addcity == busRoute1.DestinationCityId)
                    {
                        ModelState.AddModelError("AddCity", "City is already in the route");
                    }

                    var existingStop = _appDbContext.BusStop.FirstOrDefault(x => x.BusRoutId == busRoute1.Id && x.AddCityId == model.Addcity);
                    if (existingStop != null)
                    {
                        ModelState.AddModelError("AddCity", "City is already in the stops");
                    }

                    if (model.StopTime < busRoute1.StartTime || model.StopTime > busRoute1.ReachedTime)
                    {
                        ModelState.AddModelError("StopTime", "Time must be between start and reached time");
                    }

                    if (ModelState.IsValid)
                    {
                        var editBusStop = _stopRepository.GetStop(model.Id);
                        editBusStop.StopTime = model.StopTime;
                        editBusStop.AddCityId = model.Addcity;
                        editBusStop.BusRoutId = model.BusRouteId;

                        _stopRepository.UpdateStop(editBusStop);
                        return RedirectToAction("GetAllStop");

                    }
                }

                var stop = _stopRepository.GetStop(model.Id);
                if (stop == null)
                {
                    return NotFound();
                }

                var busRoute = _routeRepository.GetRoute(stop.BusRoutId);
                if (busRoute == null)
                {
                    return NotFound();
                }

                var presentCityIds = _appDbContext.BusStop
                    .Where(s => s.BusRoutId == busRoute.Id)
                    .Select(s => s.AddCityId)
                    .Union(_appDbContext.BusRoute
                        .Where(br => br.Id == busRoute.Id)
                        .Select(br => br.StartCityId)
                        .Union(_appDbContext.BusRoute
                            .Where(br => br.Id == busRoute.Id)
                            .Select(br => br.DestinationCityId)))
                    .Distinct()
                    .ToList();

                var availableCities = _appDbContext.City
                    .Where(c => !presentCityIds.Contains(c.Id))
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CityName,
                        Selected = c.Id == stop.AddCityId
                    })
                    .ToList();

                var presentRoute = _appDbContext.BusRoute
                    .Join(_appDbContext.Buses,
                          br => br.BusId,
                          b => b.Id,
                          (br, b) => new SelectListItem
                          {
                              Value = br.Id.ToString(),
                              Text = b.BusNumber,
                              Selected = br.Id == stop.BusRoutId
                          })
                    .ToList();


                var viewModel = new CreateStopViewModel
                {
                    Id = stop.Id,
                    StopTime = stop.StopTime,
                    Addcity = stop.AddCityId,
                    BusRouteId = stop.BusRoutId,
                    City = availableCities.OrderBy(x => x.Text).ToList(),
                    Buses = presentRoute
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

        }

        #endregion


        #region DELETE STOP
        [HttpGet]
        public IActionResult DeleteStop(int id)
        {
            try
            {
                BusStop busStop = _stopRepository.GetStop(id);

                if (busStop == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This stop does not exist." });
                }

                var busRoute = _routeRepository.GetRoute(busStop.BusRoutId);
                var bus = _busRepository.GetBus(busRoute.BusId);
                var city = _cityRepository.GetCity(busStop.AddCityId);

                var model = new DeleteStopViewModel
                {
                    Id = id,
                    BusNumber = bus.BusNumber,
                    City = city.CityName,
                    StopTime = busStop.StopTime
                };

                return View(model);


            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
          
        }
        
        [HttpPost]
        public IActionResult DeleteStop(DeleteStopViewModel model)
        {
            try
            {
                var busStop = _stopRepository.GetStop(model.Id);
                if (busStop == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This stop does not exist." });
                }

                _stopRepository.DeleteStop(model.Id);
                return RedirectToAction("GetAllStop");
            }
            catch(Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion


    }
}

