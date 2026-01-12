using Panopticon.Models;
using Panopticon.Shared.Models;
using System.Text.Json;

namespace Panopticon.Services
{
    public class FREDService
    {
        private HttpClient _client { get; set; }
        private string? FREDApiKey { get; set; }
        public FREDService(HttpClient client)
        {
            _client = client;
            FREDApiKey = Environment.GetEnvironmentVariable("FRED_API_KEY");
        }
        public async Task<List<SahmRuleObservation>> GetSahmRuleObservations(DateTime? startDate = null)
        {
            List<SahmRuleObservation> observations = new List<SahmRuleObservation>();
            string query = $"https://api.stlouisfed.org/fred/series/observations?series_id=SAHMREALTIME&api_key={FREDApiKey}&file_type=json";
            if (startDate != null)
            {
                DateTime date = (DateTime)startDate;
                query += $"&observation_start={date.Year}-{date.Month}-{date.Day}";
            }

            using(HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, query))
            {
                using(HttpResponseMessage resp = await _client.SendAsync(req))
                {
                    FREDObservationResponse? observationResponse = JsonSerializer.Deserialize<FREDObservationResponse>(resp.Content.ReadAsStringAsync().Result);
                    if(observationResponse != null)
                    {
                        foreach(Observation observation in observationResponse.observations)
                        {
                            try
                            {
                                observations.Add(new SahmRuleObservation(DateTime.Parse(observation.date),
                                    Double.Parse(observation.value)));
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("Encountered an error parsing observation");
                                Console.WriteLine(JsonSerializer.Serialize(observation));
                            }
                        }
                    }
                }
            }

            return observations;
        }
    }
}
