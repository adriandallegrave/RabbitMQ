using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExploreCalifornia.WebApp.Pages
{
    public class BookingCanceledModel : PageModel
    {
        public string TourName { get; set; }
        public string Name { get; set; }

        public void OnGet()
        {
            TourName = Request.Query["tourname"];
            Name = Request.Query["name"];
        }
    }
}
