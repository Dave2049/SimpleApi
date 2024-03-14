namespace SimpleApi.Repository;

class GoogleMapsRepo
{
    private string GetGoogleMapsApiKey()
    {
        return Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY");
    }

    protected async Task<string> GetGeocodingData(string address)
        {
            var apiKey =  GetGoogleMapsApiKey();
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve geocoding data: {response.StatusCode}");
            }

            // Here you might want to deserialize the JSON content to a specific object.
            // For simplicity, we're returning the JSON string directly.
            return content;
        }

}