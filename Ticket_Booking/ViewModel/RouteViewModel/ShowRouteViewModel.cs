namespace Ticket_Booking.ViewModel.RouteViewModel
{
    public class ShowRouteViewModel
    {
            public int Id { get; set; }
            public string BusNumber { get; set; }
            public string StartCity { get; set; }
            public string DestinationCity { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime ReachedTime { get; set; }
            public List<StopViewModel> Stops { get; set; }

            public class StopViewModel
            {
                public string StopCity { get; set; }
                public DateTime StopTime { get; set; }
            }
        
    }
}
