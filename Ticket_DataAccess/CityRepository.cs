using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket_Model;

namespace Ticket_DataAccess
{
    public class CityRepository : ICityRepository
    {
        private List<City> cityList;

        public City AddCity(City city)
        {
            city.Id = cityList.Max(x => x.Id)+1;
            cityList.Add(city);
            return city;
        }

        public IEnumerable<City> GetAllCities()
        {
            return cityList;
        }

        public City GetCity(int Id)
        {
            City city = cityList.FirstOrDefault(x => x.Id == Id);
            return city;
        }

        public City UpdateCity(City city)
        {
            City updatedCity = cityList.FirstOrDefault(x => x.Id == city.Id);
            if(updatedCity != null)
            {
                updatedCity.CityName = city.CityName;
            }

            return updatedCity;
        }

        public City DeleteCity(int Id)
        {
            City city = cityList.FirstOrDefault(x => x.Id ==Id);
            cityList.Remove(city);
            return city;
        }

    }
}
