using FoodBridge.Web.Models;
using FoodBridge.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodBridge.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ListingApiClient _api;

        public HomeController(ListingApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var listings = await _api.GetListingsAsync(null);

                ViewBag.Available = listings.Count(l => l.Status == ListingStatus.Available);
                ViewBag.Claimed = listings.Count(l => l.Status == ListingStatus.Claimed);
                ViewBag.Collected = listings.Count(l => l.Status == ListingStatus.Collected);
            }
            catch (HttpRequestException)
            {
                // the page still loads if the api is down, just without the numbers
                ViewBag.ApiDown = true;
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
