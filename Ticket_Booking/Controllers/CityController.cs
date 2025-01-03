using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Ticket_Booking.ViewModel.BusViewModel;
using Ticket_Booking.ViewModel.CityViewModel;
using Ticket_DataAccess;
using Ticket_Model;

namespace Ticket_Booking.Controllers
{

    public class CityController : Controller
    {
        private ICityRepository _cityRepository;
        private AppDbContext _appDbContext;

        public CityController(ICityRepository cityRepository, AppDbContext appDbContext)
        {
            _cityRepository = cityRepository;
            _appDbContext = appDbContext;
        }


        #region GET ALL CITY
        public IActionResult GetAllCity()
        {
            try
            {
                var city = _cityRepository.GetAllCities();
                return View(city);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion

        #region CREATE CITY

        [HttpGet]
        public IActionResult CreateCity()
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

        [HttpPost]
        public IActionResult CreateCity(CreateCityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    string pattern = @"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$";
                    Regex regex = new Regex(pattern);
                    bool isMatch = Regex.IsMatch(model.CityName, pattern);
                    if (!isMatch)
                    {
                        ModelState.AddModelError("CityName", "City name must be in string ");
                        return View();
                    }
                    var allcity = _cityRepository.GetAllCities();
                    foreach (var i in allcity)
                    {
                        if (i.CityName.ToLower() == model.CityName.ToLower())
                        {
                            ModelState.AddModelError("CityName", "City name can not be repeated ");
                            return View();
                        }
                    }
                    City city = new City
                    {
                        CityName = model.CityName
                    };
                    _cityRepository.AddCity(city);
                    return RedirectToAction("GetAllCity");
                }

                return View();

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }

            

        }

        #endregion

        #region EDIT CITY

        [HttpGet]
        public IActionResult EditCity(int id)
        {
            try
            {
                City editCity = _cityRepository.GetCity(id);

                if (editCity == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This city does not exist." });
                }

                EditCityViewModel model = new EditCityViewModel
                {
                    CityName = editCity.CityName
                };

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
           
        }

        [HttpPost]
        public IActionResult EditCity(EditCityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string pattern = @"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$";
                    Regex regex = new Regex(pattern);
                    bool isMatch = Regex.IsMatch(model.CityName, pattern);
                    if (!isMatch)
                    {
                        ModelState.AddModelError("CityName", "City name must be in string ");
                        return View(model);
                    }

                    City city = _cityRepository.GetCity(model.Id);
                    city.CityName = model.CityName;

                    _cityRepository.UpdateCity(city);
                    return RedirectToAction("GetAllCity");
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }
        #endregion

        #region DELETE CITY
        [HttpGet]
        public IActionResult DeleteCity(int id)
        {
            try
            {
                City bus = _cityRepository.GetCity(id);
                if (bus == null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "This city does not exist." });
                }
                DeleteCityViewModel model = new DeleteCityViewModel
                {
                    CityName = bus.CityName
                };
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        [HttpPost]
        public IActionResult DeleteCity(DeleteCityViewModel model)
        {
            try
            {
                var cityUsedInRoute = _appDbContext.BusRoute.FirstOrDefault(x => (x.StartCityId == model.Id || x.DestinationCityId == model.Id));
                var cityUsedInStop = _appDbContext.BusStop.FirstOrDefault(x => x.AddCityId == model.Id);
                if (cityUsedInRoute != null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "Cannot delete this city because city used in busroute." });
                }
                if (cityUsedInStop != null)
                {
                    return RedirectToAction("ErrorPage", "Bus", new { message = "Cannot delete this city because city used in busroutestop." });
                }
                _cityRepository.DeleteCity(model.Id);
                return RedirectToAction("GetAllCity");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Bus", new { message = ex.Message });
            }
            
        }

        #endregion


    }
}
