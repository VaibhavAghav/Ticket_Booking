using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Ticket_Booking.Models;
using Ticket_Booking.ViewModel.BusViewModel;
using Ticket_DataAccess;
using Ticket_Model;

namespace Ticket_Booking.Controllers
{


    public class BusController : Controller
    {
        private readonly ILogger<BusController> _logger;
        private readonly IBusRepository _busRepository;
        private readonly AppDbContext _appDbContext;

        public BusController(ILogger<BusController> logger, IBusRepository busRepository, AppDbContext appDbContext)
        {
            _busRepository = busRepository;
            _logger = logger;
            _appDbContext = appDbContext;
        }


        #region GETALL BUSES
        public IActionResult GetAllBus()
        {
            try
            {
                var buses = _busRepository.GetAllBuses();
                return View(buses);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion

        #region CREATEBUS

        [HttpGet]
        public IActionResult CreateBus()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        [HttpPost]
        public IActionResult CreateBus(CreateBusViewModel model)
        {
            _logger.LogInformation("model ", model.BusNumber);
            try
            {
                if (ModelState.IsValid)
                {
                    var allbuses = _busRepository.GetAllBuses();
                    var doublebus = allbuses.FirstOrDefault(x => x.BusNumber == model.BusNumber);
                    if (doublebus != null)
                    {
                        ModelState.AddModelError("BusNumber", "Bus Number cannot be Repeated");
                        return View(model);
                    }
                    string pattern = @"^[A-Z]{2}\s\d{2}\s[A-Z]{2}\s\d{4}$";
                    bool isMatch = Regex.IsMatch(model.BusNumber, pattern);
                    if (model.SeatCapacity < 10 || !isMatch || model.SeatCapacity > 25)
                    {
                        ModelState.AddModelError("BusNumber", "Bus number must be in the format WW NN WW NNNN where W is an uppercase letter and N is a digit.");
                        ModelState.AddModelError("SeatCapacity", "Seat capacity Can be 10 to 25.");
                        return View(model);
                    }
                    Bus newbus = new Bus
                    {
                        BusNumber = model.BusNumber,
                        SeatCapacity = model.SeatCapacity
                    };

                    _busRepository.AddBus(newbus);
                    return RedirectToAction("GetAllBus");
                }
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion

        #region EDIT BUS

        [HttpGet]
        public IActionResult EditBus(int id)
        {
            try
            {
                Bus editBus = _busRepository.GetBus(id);

                if (editBus == null)
                {
                    return RedirectToAction("ErrorPage", new { message = "This bus does not exist." });
                }

                EditBusViewModel model = new EditBusViewModel
                {
                    BusNumber = editBus.BusNumber,
                    SeatCapacity = editBus.SeatCapacity
                };


                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        [HttpPost]
        public IActionResult EditBus(EditBusViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string pattern = @"^[A-Z]{2}\s\d{2}\s[A-Z]{2}\s\d{4}$";
                    bool isMatch = Regex.IsMatch(model.BusNumber, pattern);
                    if (model.SeatCapacity < 10 || !isMatch || model.SeatCapacity > 25)
                    {
                        ModelState.AddModelError("BusNumber", "Bus number must be in the format WW NN WW NNNN where W is an uppercase letter and N is a digit.");
                        ModelState.AddModelError("SeatCapacity", "Seat capacity Can be 10 to 25.");
                        return View(model);
                    }

                    Bus editBus = _busRepository.GetBus(model.Id);
                    editBus.BusNumber = model.BusNumber;
                    editBus.SeatCapacity = model.SeatCapacity;


                    _busRepository.UpdateBus(editBus);
                    return RedirectToAction("GetAllBus");

                }
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }
        #endregion

        #region DELETE BUS
        [HttpGet]
        public IActionResult DeleteBus(int id)
        {
            try
            {
                Bus bus = _busRepository.GetBus(id);
                if (bus == null)
                {
                    return RedirectToAction("ErrorPage", new { message = "This bus does not exist." });
                }
                DeleteBusViewModel model = new DeleteBusViewModel
                {
                    BusNumber = bus.BusNumber,
                    SeatCapacity = bus.SeatCapacity
                };
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        [HttpPost]
        public IActionResult DeleteBus(DeleteBusViewModel model)
        {
            try
            {
                var busUsedInRoute = _appDbContext.BusRoute.FirstOrDefault(x => x.BusId == model.Id);
                if (busUsedInRoute != null)
                {
                    return RedirectToAction("ErrorPage", new { message = "Cannot delete this bus because related route exist." });
                }
                _busRepository.DeleteBus(model.Id);
                return RedirectToAction("GetAllBus");

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion

        #region LOGO INFO
        [HttpGet]
        public IActionResult BusInformation()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }
        #endregion

        #region ERROR PAGE
        public IActionResult Privacy()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        public IActionResult ErrorPage(string message)
        {
            ViewBag.Message = message;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion

    }
}
