using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ticket_Booking.ViewModel.RouteViewModel;
using Ticket_DataAccess;
using Ticket_Model;

namespace Ticket_Booking.Controllers
{
 

    public class RouteController : Controller
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IBusRepository _busRepository;
        private readonly ICityRepository _cityRepository;
        private readonly AppDbContext _appDbContext;
        private readonly IStopRepository _stopRepository;

        public RouteController(IRouteRepository routeRepository , ICityRepository cityRepository,
                               IBusRepository busRepository, AppDbContext appDbContext,
                               IStopRepository stopRepository)
        {
            _stopRepository = stopRepository;
            _routeRepository = routeRepository;
            _busRepository = busRepository;
            _cityRepository = cityRepository;
            _appDbContext = appDbContext;

        }

        #region GET ALL ROUTE

        public IActionResult GetAllRoute(int? fromCityId, int? toCityId)
        {
            try
            {
                var query = from br in _appDbContext.BusRoute
                        join b in _appDbContext.Buses on br.BusId equals b.Id
                        join sc in _appDbContext.City on br.StartCityId equals sc.Id
                        join ec in _appDbContext.City on br.DestinationCityId equals ec.Id
                        select new
                        {
                            Id = br.Id,
                            BusId = b.BusNumber,
                            StartCity = sc.CityName,
                            EndCity = ec.CityName,
                            StartTime = br.StartTime,
                            ReachedTime = br.ReachedTime,
                            StartCityId = sc.Id,
                            EndCityId = ec.Id
                        };

                if (fromCityId.HasValue)
                {
                query = query.Where(r => r.StartCityId == fromCityId.Value);
                }
                if (toCityId.HasValue)
                {
                    query = query.Where(r => r.EndCityId == toCityId.Value);
                }

                var results = query.ToList();
                var viewRouteList = results.Select(r => new AllRouteViewModel
                {
                    Id = r.Id,
                    BusNumber = r.BusId,
                    StartCity = r.StartCity,
                    EndCity = r.EndCity,
                    StartTime = r.StartTime,
                    ReachedTime = r.ReachedTime
                }).ToList();
                    
                var cities = _appDbContext.City.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CityName
                }).ToList();
                    
                ViewBag.Cities = cities;

                return View(viewRouteList);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        #endregion


        #region CREATE BUS-ROUTE

        [HttpGet]
        public IActionResult CreateRoute()
        {
            try
            {
                var allCity = _cityRepository.GetAllCities();
                var allbus = _busRepository.GetAllBuses();
                var viewModel = new CreateRouteViewModel
                {
                    City = allCity.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CityName
                    }),
                    Buses = allbus.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.BusNumber
                    })
                };
                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        [HttpPost]
        public IActionResult CreateRoute(CreateRouteViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dbbuses = _appDbContext.BusRoute.Where(br => br.BusId == viewModel.BusId).ToList();

                    if (viewModel.DestinationCityId == viewModel.StartCityId)
                    {
                        ModelState.AddModelError("DestinationCityId", "Destination City cannot be the same as Start City.");
                    }
                    if (viewModel.StartTime >= viewModel.ReachedTime)
                    {
                        ModelState.AddModelError("ReachedTime", "Reached Time must be after Start Time.");
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

                    if (dbbuses != null)
                    {
                        int count = 0;
                        foreach (var dbbus in dbbuses)
                        {
                            if (viewModel.StartTime > dbbus.ReachedTime || viewModel.ReachedTime < dbbus.StartTime)
                            {
                                count++;
                            }
                        }

                        if (count == dbbuses.Count)
                        {
                            var busRoute = new BusRoute
                            {
                                StartTime = viewModel.StartTime,
                                ReachedTime = viewModel.ReachedTime,
                                StartCityId = viewModel.StartCityId,
                                DestinationCityId = viewModel.DestinationCityId,
                                BusId = viewModel.BusId
                            };

                            _routeRepository.AddRoute(busRoute);
                            return RedirectToAction("GetAllRoute");
                        }
                        else
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

                            ViewBag.Dbbuses = dbbuses;
                            ViewBag.ErrorMessage = "Bus is allocated at another route for this time";
                            return View(viewModel);
                        }

                    }

                    var busRoute1 = new BusRoute
                    {
                        StartTime = viewModel.StartTime,
                        ReachedTime = viewModel.ReachedTime,
                        StartCityId = viewModel.StartCityId,
                        DestinationCityId = viewModel.DestinationCityId,
                        BusId = viewModel.BusId
                    };

                    _routeRepository.AddRoute(busRoute1);
                    return RedirectToAction("GetAllRoute");

                }

                var cities1 = _cityRepository.GetAllCities();
                var buses1 = _busRepository.GetAllBuses();
                viewModel.City = cities1.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CityName
                });
                viewModel.Buses = buses1.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.BusNumber
                });

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        #endregion


        #region EDIT BUS-ROUTE

        [HttpGet]
        public IActionResult EditRoute(int Id)
        {
            try
            {
                var busRoute = _routeRepository.GetRoute(Id);
                if (busRoute == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This route does not exist." });
                }
                var cities = _cityRepository.GetAllCities();
                var buses = _busRepository.GetAllBuses();

                var viewModel = new EditRouteViewModel
                {
                    Id = busRoute.Id,
                    StartTime = busRoute.StartTime,
                    ReachedTime = busRoute.ReachedTime,
                    StartCityId = busRoute.StartCityId,
                    DestinationCityId = busRoute.DestinationCityId,
                    BusId = busRoute.BusId,
                    City = cities.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CityName,
                        Selected = c.Id == busRoute.StartCityId
                    }),
                    Buses = buses.Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.BusNumber,
                        Selected = b.Id == busRoute.BusId
                    })
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        [HttpPost]
        public IActionResult EditRoute(EditRouteViewModel viewModel)
        {
            try
            {
                var cities = _cityRepository.GetAllCities();
                var buses = _busRepository.GetAllBuses();

                if (ModelState.IsValid)
                {
                    if (viewModel.DestinationCityId == viewModel.StartCityId)
                    {
                        ModelState.AddModelError("DestinationCityId", "Destination City cannot be the same as Start City.");
                    }
                    if (viewModel.StartTime >= viewModel.ReachedTime)
                    {
                        ModelState.AddModelError("ReachedTime", "Reached Time must be after Start Time.");
                    }

                    if (!ModelState.IsValid)
                    {
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

                    var busRoute = _routeRepository.GetRoute(viewModel.Id);

                    busRoute.StartTime = viewModel.StartTime;
                    busRoute.ReachedTime = viewModel.ReachedTime;
                    busRoute.StartCityId = viewModel.StartCityId;
                    busRoute.DestinationCityId = viewModel.DestinationCityId;
                    busRoute.BusId = viewModel.BusId;

                    _routeRepository.UpdateRoute(busRoute);
                    return RedirectToAction("GetAllRoute");
                }

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
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        #endregion


        #region VIEW ROUTE
        public IActionResult ViewRoute(int id)
        {
            try
            {
                var busRoute = _routeRepository.GetRoute(id);
                if (busRoute == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This route does not exist." });

                }

                var bus = _busRepository.GetBus(busRoute.BusId);
                var startCity = _cityRepository.GetCity(busRoute.StartCityId);
                var endCity = _cityRepository.GetCity(busRoute.DestinationCityId);
                var stops = _stopRepository.GetAllStops().Where(s => s.BusRoutId == id)
                                    .Select(s => new ShowRouteViewModel.StopViewModel
                                    {
                                        StopCity = _cityRepository.GetCity(s.AddCityId).CityName,
                                        StopTime = s.StopTime
                                    }).ToList();

                var viewModel = new ShowRouteViewModel
                {
                    Id = busRoute.Id,
                    BusNumber = bus.BusNumber,
                    StartCity = startCity.CityName,
                    DestinationCity = endCity.CityName,
                    StartTime = busRoute.StartTime,
                    ReachedTime = busRoute.ReachedTime,
                    Stops = stops
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            
        }

        #endregion


        #region DELETE BUS-ROUTE

        [HttpGet]
        public IActionResult DeleteRoute(int id)
        {
            try
            {
                var busRoute = _routeRepository.GetRoute(id);
                var cities = _cityRepository.GetAllCities();
                var buses = _busRepository.GetAllBuses();

                if (busRoute == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This route does not exist." });
                }


                var stops = _appDbContext.BusStop
                    .Where(stop => stop.BusRoutId == id)
                    .Select(stop => new StopViewModel
                    {
                        Id = stop.Id,
                        CityName = _cityRepository.GetCity(stop.AddCityId).CityName,
                        StopTime = stop.StopTime
                    })
                    .ToList();
                var viewModel = new DeleteRouteViewModel
                {
                    Id = busRoute.Id,
                    StartTime = busRoute.StartTime,
                    ReachedTime = busRoute.ReachedTime,
                    StartCityId = busRoute.StartCityId,
                    DestinationCityId = busRoute.DestinationCityId,
                    BusId = busRoute.BusId,
                    City = cities.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CityName,
                        Selected = c.Id == busRoute.StartCityId
                    }),
                    Buses = buses.Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.BusNumber,
                        Selected = b.Id == busRoute.BusId
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
        public IActionResult DeleteRoute(DeleteRouteViewModel viewModel)
        {
            try
            {
                var stopsinRoute = _appDbContext.BusStop.Where(x => x.BusRoutId == viewModel.Id).ToList();
                if (stopsinRoute.Count != 0)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "Cannot delete this route because related route has stop delete it first." });
                }
                _routeRepository.DeleteRoute(viewModel.Id);
                return RedirectToAction("GetAllRoute");

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

        }

        #endregion

    }
}
