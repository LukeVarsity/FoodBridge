using FoodBridge.Web.Models;
using FoodBridge.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodBridge.Web.Controllers
{
    public class ListingsController : Controller
    {
        private readonly ListingApiClient _api;

        public ListingsController(ListingApiClient api)
        {
            _api = api;
        }

        // GET /Listings  or  /Listings?status=Available
        public async Task<IActionResult> Index(string? status)
        {
            ViewBag.Status = status;

            try
            {
                var listings = await _api.GetListingsAsync(status);
                return View(listings);
            }
            catch (HttpRequestException)
            {
                // happens if the api container is down, or the free render service is still waking up
                ViewBag.Error = "Couldn't reach the FoodBridge API. If this is the live site the API may be waking up, give it a minute and refresh.";
                return View(new List<FoodListing>());
            }
        }

        // GET /Listings/Create
        public IActionResult Create()
        {
            // default the collect by time to 4 hours from now
            var listing = new FoodListing { CollectBy = DateTime.Now.AddHours(4) };
            return View(listing);
        }

        // POST /Listings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodListing listing)
        {
            if (!ModelState.IsValid)
            {
                return View(listing);
            }

            var error = await _api.CreateListingAsync(listing);

            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(listing);
            }

            TempData["Message"] = "Your listing is live. NGOs can see it and claim it now.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Listings/Claim/5 - shows the listing and asks for the NGO name
        public async Task<IActionResult> Claim(int id)
        {
            var listing = await _api.GetListingAsync(id);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // POST /Listings/Claim/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Claim(int id, string? claimedBy)
        {
            if (string.IsNullOrWhiteSpace(claimedBy))
            {
                ModelState.AddModelError("claimedBy", "Enter the name of your organisation.");
                var listing = await _api.GetListingAsync(id);
                return listing == null ? NotFound() : View(listing);
            }

            var error = await _api.ClaimListingAsync(id, claimedBy!.Trim());
            TempData["Message"] = error ?? "Claimed. Please collect the food before the collect by time.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Listings/Collect/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Collect(int id)
        {
            var error = await _api.CollectListingAsync(id);
            TempData["Message"] = error ?? "Marked as collected. Thanks for keeping food out of landfill.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Listings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _api.DeleteListingAsync(id);
            TempData["Message"] = error ?? "Listing removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
