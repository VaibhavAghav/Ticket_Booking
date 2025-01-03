using Ticket_Model;

namespace Ticket_DataAccess
{
    public interface ICityRepository
    {
        City AddCity(City city);
        IEnumerable<City> GetAllCities();
        City GetCity(int Id);
        City UpdateCity(City city);
        City DeleteCity(int Id);
    }
}