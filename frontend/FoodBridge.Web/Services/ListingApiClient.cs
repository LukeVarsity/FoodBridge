using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoodBridge.Web.Models;

namespace FoodBridge.Web.Services
{
    // every call to the backend api goes through here so the controllers stay small
    public class ListingApiClient
    {
        private readonly HttpClient _http;

        // the api sends the status as text so this has to read it the same way
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public ListingApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FoodListing>> GetListingsAsync(string? status)
        {
            var url = "api/listings";

            if (!string.IsNullOrEmpty(status))
            {
                url += "?status=" + status;
            }

            var listings = await _http.GetFromJsonAsync<List<FoodListing>>(url, _jsonOptions);
            return listings ?? new List<FoodListing>();
        }

        public async Task<FoodListing?> GetListingAsync(int id)
        {
            var response = await _http.GetAsync($"api/listings/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<FoodListing>(_jsonOptions);
        }

        // these return null if it worked, otherwise the message the api sent back
        public async Task<string?> CreateListingAsync(FoodListing listing)
        {
            var response = await _http.PostAsJsonAsync("api/listings", listing, _jsonOptions);
            return await GetError(response);
        }

        public async Task<string?> ClaimListingAsync(int id, string claimedBy)
        {
            var response = await _http.PutAsJsonAsync($"api/listings/{id}/claim", new { claimedBy }, _jsonOptions);
            return await GetError(response);
        }

        public async Task<string?> CollectListingAsync(int id)
        {
            var response = await _http.PutAsync($"api/listings/{id}/collect", null);
            return await GetError(response);
        }

        public async Task<string?> DeleteListingAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/listings/{id}");
            return await GetError(response);
        }

        private static async Task<string?> GetError(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            var message = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(message))
            {
                return $"The API returned {(int)response.StatusCode}.";
            }

            return message;
        }
    }
}
